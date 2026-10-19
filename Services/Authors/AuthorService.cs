using Microsoft.EntityFrameworkCore;
using Mapster;
using FirstBackendApp.Contracts.Authors.Parameters;
using FirstBackendApp.Contracts.Authors.Requests;
using FirstBackendApp.Contracts.Authors.Responses;
using FirstBackendApp.Contracts.Common.Responses;
using FirstBackendApp.Data;
using FirstBackendApp.Entities;
using FirstBackendApp.Extensions;
using FirstBackendApp.Services.Common;

namespace FirstBackendApp.Services.Authors;

// Mualliflar bilan ishlash biznes mantiqi
public class AuthorService : IAuthorService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuthorService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // 1. Mualliflarni qidirish, filtrlash va sahifalash orqali olish
    public async Task<PagedResponse<AuthorResponse>> GetAllAsync(AuthorFilterParameters parameters)
    {
        var search = parameters.Search?.Trim().ToLower();

        var query = _context.Authors
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(search), a => 
                a.FirstName.ToLower().Contains(search!) || a.LastName.ToLower().Contains(search!))
            .OrderByDescending(a => a.Id);
        
        return await query
            .ProjectToType<AuthorResponse>()
            .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
    }

    // 2. Muallifni ID bo'yicha topish
    public async Task<AuthorResponse?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Where(a => a.Id == id)
            .ProjectToType<AuthorResponse>()
            .FirstOrDefaultAsync();
    }

    // 3. Yangi muallif yaratish
    public async Task<AuthorResponse> CreateAsync(CreateAuthorRequest request)
    {
        var author = request.Adapt<Author>();
        
        author.CreatedById = _currentUser.UserId;

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return author.Adapt<AuthorResponse>();
    }

    // 4. Muallif ma'lumotlarini to'liq yangilash (PUT)
    public async Task<AuthorResponse?> UpdateAsync(int id, UpdateAuthorRequest request)
    {
        var author = await GetEntityByIdAsync(id);
        if (author is null) return null;

        if (!_currentUser.IsAdmin && author.CreatedById != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("Siz faqat o'zingiz yaratgan mualliflarni o'zgartira olasiz.");
        }

        request.Adapt(author);
        
        await _context.SaveChangesAsync();
        
        return author.Adapt<AuthorResponse>();
    }

    // 5. Muallif ma'lumotlarini qisman yangilash (PATCH)
    public async Task<AuthorResponse?> PatchAsync(int id, PatchAuthorRequest request)
    {
        var author = await GetEntityByIdAsync(id);
        if (author is null) return null;
        
        if (!_currentUser.IsAdmin && author.CreatedById != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("Siz faqat o'zingiz yaratgan mualliflarni o'zgartira olasiz.");
        }
        
        request.Adapt(author, TypeAdapterConfig.GlobalSettings.Clone()
            .NewConfig<PatchAuthorRequest, Author>()
            .IgnoreNullValues(true)
            .Config
        );
        
        await _context.SaveChangesAsync();
        return author.Adapt<AuthorResponse>();
    }

    // 6. Muallifni tizimdan o'chirish (DELETE)
    public async Task<bool> DeleteAsync(int id)
    {
        var author = await GetEntityByIdAsync(id);
        if (author is null) return false;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return true;
    }

    // --- Yordamchi (Private Helper) metodlar ---

    // Bazadan muallifning asl Entity modelini topuvchi yordamchi metod
    private Task<Author?> GetEntityByIdAsync(int id)
    {
        return _context.Authors
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}