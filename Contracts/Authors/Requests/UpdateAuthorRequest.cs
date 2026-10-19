namespace FirstBackendApp.Contracts.Authors.Requests;

// Muallif ma'lumotlarini to'liq yangilash so'rovi modeli (PUT)
public class UpdateAuthorRequest
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