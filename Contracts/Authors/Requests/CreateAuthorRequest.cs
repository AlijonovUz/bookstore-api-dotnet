namespace FirstBackendApp.Contracts.Authors.Requests;

// Yangi muallif qo'shish so'rovi modeli (POST)
public class CreateAuthorRequest
{
    // Muallif ismi (majburiy)
    public string FirstName { get; set; } = string.Empty;

    // Muallif familiyasi (majburiy)
    public string LastName { get; set; } = string.Empty;

    // Yashash manzili (ixtiyoriy)
    public string? Address { get; set; }

    // Tug'ilgan sanasi (ixtiyoriy)
    public DateOnly? BirthDate { get; set; }
}