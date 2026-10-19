using FirstBackendApp.Contracts.Auth.Responses;

namespace FirstBackendApp.Contracts.Authors.Responses;

// Muallif haqidagi to'liq ma'lumot javob modeli (DTO)
public class AuthorResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateOnly? BirthDate { get; set; }
    
    // Muallifni kiritgan foydalanuvchi
    public UserResponse? CreatedBy { get; set; }
    
    // Muallifga tegishli kitoblar soni
    public int BooksCount { get; set; }
}