namespace FirstBackendApp.Entities;

// Kitob modeli (Database Entity)
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int PageCount { get; set; }
    public bool InStock { get; set; }

    public string Isbn { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    
    // Foydalanuvchi bilan bog'liqlik
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    // Muallif bilan bog'liqlik
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}