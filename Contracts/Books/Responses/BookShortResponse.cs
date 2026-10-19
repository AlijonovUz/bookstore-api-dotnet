namespace FirstBackendApp.Contracts.Books.Responses;

// Muallif ma'lumotlariga biriktirilgan kitoblarning qisqartirilgan modeli (DTO)
public class BookShortResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int PageCount { get; set; }
    public bool InStock { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}