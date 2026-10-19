using FirstBackendApp.Contracts.Auth.Responses;
using FirstBackendApp.Contracts.Authors.Responses;

namespace FirstBackendApp.Contracts.Books.Responses;

// Kitob haqidagi to'liq ma'lumot javob modeli (DTO)
public class BookResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int PageCount { get; set; }
    public bool InStock { get; set; }

    public string Isbn { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    
    // Kitob kiritgan foydalanuvchi haqida ma'lumot
    public UserResponse? CreatedBy { get; set; }
    
    // Kitob muallifi haqidagi qisqa ma'lumot
    public AuthorShortResponse? Author { get; set; }
}