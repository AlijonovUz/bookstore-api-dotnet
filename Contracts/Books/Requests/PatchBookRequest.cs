using Microsoft.AspNetCore.Http;

namespace FirstBackendApp.Contracts.Books.Requests;

// Kitob ma'lumotlarini qisman yangilash so'rovi modeli (PATCH / multipart/form-data)
// Har bir maydon nullable bo'lib, faqat yuborilgan qiymatlar yangilanadi
public class PatchBookRequest
{
    // Kitob nomi (ixtiyoriy)
    public string? Title { get; set; }

    // Kitob tavsifi (ixtiyoriy)
    public string? Description { get; set; } 

    // Kitob narxi (ixtiyoriy)
    public decimal? Price { get; set; }

    // Sahifalar soni (ixtiyoriy)
    public int? PageCount { get; set; }

    // Omborda mavjudligi (ixtiyoriy)
    public bool? InStock { get; set; }

    // Xalqaro standart kitob raqami (ixtiyoriy)
    public string? Isbn { get; set; } 

    // Yangi rasm fayli (ixtiyoriy)
    public IFormFile? Image { get; set; }
    
    // Muallif identifikatori (ixtiyoriy)
    public int? AuthorId { get; set; }
}