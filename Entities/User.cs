namespace FirstBackendApp.Entities;

// Foydalanuvchi modeli (Database Entity)
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigatsion xususiyat: Userga tegishli bo'lgan kitoblar va Mualliflar ro'yxati
    public ICollection<Book?> Books { get; set; } = new List<Book?>();
    public ICollection<Author?> Authors { get; set; } = new List<Author?>();
}