using Microsoft.AspNetCore.Http;

namespace FirstBackendApp.Contracts.Books.Requests;

// Yangi kitob yaratish so'rovi modeli (POST / multipart/form-data)
public class CreateBookRequest
{
    // Kitob nomi (majburiy)
    public string Title { get; set; } = string.Empty;

    // Kitob tavsifi
    public string Description { get; set; } = string.Empty;

    // Kitob narxi (0 dan katta bo'lishi kerak)
    public decimal Price { get; set; }

    // Sahifalar soni (1 dan 5000 gacha)
    public int PageCount { get; set; }

    // Omborda mavjudligi
    public bool InStock { get; set; } = true;

    // Xalqaro standart kitob raqami (ISBN)
    public string Isbn { get; set; } = string.Empty;

    // Kitob muqovasi rasmi (fayl sifatida yuklanadi)
    public IFormFile? Image { get; set; }

    // Ushbu kitob qaysi muallifga tegishliligi (Author Id)
    public int AuthorId { get; set; }
}