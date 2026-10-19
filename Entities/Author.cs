namespace FirstBackendApp.Entities;

// Muallif modeli (Database Entity)
public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateOnly? BirthDate { get; set; }
    
    // Foydalanuvchi bilan bog'liqlik (Foreign Key):
    public int? CreatedById { get; set; } 
    public User? CreatedBy { get; set; }

    // Navigatsion xususiyat: Muallifga tegishli bo'lgan kitoblar ro'yxati
    public ICollection<Book> Books { get; set; } = new List<Book>();
}