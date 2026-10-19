using Microsoft.EntityFrameworkCore;
using Mapster;
using FirstBackendApp.Contracts.Books.Parameters;
using FirstBackendApp.Contracts.Books.Requests;
using FirstBackendApp.Contracts.Books.Responses;
using FirstBackendApp.Contracts.Common.Responses;
using FirstBackendApp.Data;
using FirstBackendApp.Entities;
using FirstBackendApp.Extensions;
using FirstBackendApp.Services.Common;
using Microsoft.Extensions.Caching.Memory;

namespace FirstBackendApp.Services.Books;

// Kitoblar bilan ishlash biznes mantiqi
public class BookService : IBookService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ICurrentUserService _currentUser;

    public BookService(AppDbContext context, IMemoryCache cache, ICurrentUserService currentUser)
    {
        _context = context;
        _cache = cache;
        _currentUser = currentUser;
    }

    // 1. Kitoblarni qidirish, filtrlash va sahifalash orqali olish
    public async Task<PagedResponse<BookResponse>> GetAllAsync(BookFilterParameters parameters)
    {
        var cacheKey = $"books_p{parameters.PageNumber}_s{parameters.PageSize}_" +
                       $"q:{parameters.Search?.Trim().ToLower()}_" +
                       $"a:{parameters.AuthorId}_" +
                       $"min:{parameters.MinPrice}_" +
                       $"max:{parameters.MaxPrice}";
        
        if (_cache.TryGetValue(cacheKey, out PagedResponse<BookResponse>? cachedResult) && cachedResult is not null)
        {
            return cachedResult;
        }
        
        var search = parameters.Search?.Trim().ToLower();

        var query = _context.Books.AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(search), b =>
                b.Title.ToLower().Contains(search!) || b.Description.ToLower().Contains(search!))
            .WhereIf(parameters.AuthorId.HasValue, b => 
                b.AuthorId == parameters.AuthorId)
            .WhereIf(parameters.MinPrice.HasValue, b => 
                b.Price >= parameters.MinPrice!.Value)
            .WhereIf(parameters.MaxPrice.HasValue, b => 
                b.Price <= parameters.MaxPrice!.Value)
            .OrderByDescending(b => b.Id);
    
        var result = await query
            .ProjectToType<BookResponse>()
            .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(TimeSpan.FromSeconds(30));

        _cache.Set(cacheKey, result, cacheOptions);

        return result;
    }

    // 2. Kitobni ID bo'yicha topish
    public async Task<BookResponse?> GetByIdAsync(int id)
    {
        var cacheKey = $"book_{id}";

        if (_cache.TryGetValue(cacheKey, out BookResponse? cachedBook) && cachedBook is not null)
        {
            return cachedBook;
        }

        var book = await _context.Books
            .Where(b => b.Id == id)
            .ProjectToType<BookResponse>()
            .FirstOrDefaultAsync();

        if (book is not null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, book, cacheOptions);
        }

        return book;
    }
    // 3. Yangi kitob yaratish va uning rasmini saqlash
    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        await EnsureAuthorExistsAsync(request.AuthorId);
        await EnsureIsbnIsUniqueAsync(request.Isbn);

        var book = request.Adapt<Book>();

        if (request.Image is not null)
        {
            book.ImageUrl = await SaveImageAsync(request.Image);
        }

        book.CreatedById = _currentUser.UserId;
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        
        return (await GetByIdAsync(book.Id))!;
    }

    // 4. Kitob ma'lumotlarini to'liq yangilash (PUT)
    public async Task<BookResponse?> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await GetEntityByIdAsync(id);
        if (book is null) return null;
        
        if (!_currentUser.IsAdmin && book.CreatedById != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("Siz faqat o'zingiz yaratgan kitoblarni o'zgartira olasiz.");
        }
        
        await EnsureAuthorExistsAsync(request.AuthorId);
        await EnsureIsbnIsUniqueAsync(request.Isbn, id);
        
        request.Adapt(book);
        
        if (request.Image is not null)
        {
            book.ImageUrl = await SaveImageAsync(request.Image, book.ImageUrl);
        }

        await _context.SaveChangesAsync();
        
        _cache.Remove($"book_{id}");
        
        return (await GetByIdAsync(book.Id))!;
    }

    // 5. Kitob ma'lumotlarini qisman yangilash (PATCH)
    public async Task<BookResponse?> PatchAsync(int id, PatchBookRequest request)
    {
        var book = await GetEntityByIdAsync(id);
        if (book is null) return null;
        
        if (!_currentUser.IsAdmin && book.CreatedById != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("Siz faqat o'zingiz yaratgan kitoblarni o'zgartira olasiz.");
        }

        if (request.AuthorId.HasValue)
        {
            await EnsureAuthorExistsAsync(request.AuthorId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Isbn))
        {
            await EnsureIsbnIsUniqueAsync(request.Isbn, id);
        }
        
        request.Adapt(book, TypeAdapterConfig.GlobalSettings.Clone()
            .NewConfig<PatchBookRequest, Book>()
            .IgnoreNullValues(true)
            .Config
        );
        
        if (request.Image is not null)
        {
            book.ImageUrl = await SaveImageAsync(request.Image, book.ImageUrl);
        }
        
        await _context.SaveChangesAsync();
        
        _cache.Remove($"book_{id}");
        
        return (await GetByIdAsync(book.Id))!;
    }

    // 6. Kitobni va unga biriktirilgan rasmni o'chirish (DELETE)
    public async Task<bool> DeleteAsync(int id)
    {
        var book = await GetEntityByIdAsync(id);
        if (book is null) return false;
        
        DeleteImageFile(book.ImageUrl);
        
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        
        _cache.Remove($"book_{id}");
        
        return true;
    }
    
    // --- Yordamchi (Private Helper) metodlar ---

    // Bazadan kitobning asl Entity modelini topuvchi yordamchi metod
    private Task<Book?> GetEntityByIdAsync(int id)
    {
        return _context.Books
            .Include(b => b.CreatedBy)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // Kitobga biriktirilayotgan muallif bazada mavjudligini tekshiruvchi metod
    private async Task EnsureAuthorExistsAsync(int id)
    {
        var exists = await _context.Authors.AnyAsync(a => a.Id == id);
        if (!exists)
        {
            throw new ArgumentException($"ID: {id} bo'lgan muallif topilmadi.");
        }
    }
    
    // Bazada bunday ISBN bor-yo'qligini tekshiruvchi asinxron yordamchi metod
    private async Task EnsureIsbnIsUniqueAsync(string isbn, int? currentBookId = null)
    {
        var exists = await _context.Books.AnyAsync(b => b.Isbn == isbn && (currentBookId == null || b.Id != currentBookId));
        if (exists)
        {
            throw new ArgumentException($"Ushbu ISBN ({isbn}) raqamiga ega kitob tizimda allaqachon mavjud.");
        }
    }
    // Rasm faylini diskka (wwwroot/uploads/books/) yuklash va eski rasmni almashtirish metodi
    private async Task<string?> SaveImageAsync(IFormFile? imageFile, string? oldImageUrl = null)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return null;
        }
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension = Path.GetExtension(imageFile.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }
        
        if (!string.IsNullOrWhiteSpace(oldImageUrl))
        {
            DeleteImageFile(oldImageUrl);
        }

        return $"/uploads/books/{uniqueFileName}";
    }

    // Server diskidagi rasmni jismoniy o'chirish metodi
    private void DeleteImageFile(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        var relativePath = imageUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}