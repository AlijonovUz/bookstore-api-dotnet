namespace FirstBackendApp.Contracts.Authors.Requests;

// Muallif ma'lumotlarini qisman yangilash so'rovi modeli (PATCH)
// Har bir maydon nullable bo'lib, faqat qiymat berilgan maydonlargina yangilanadi
public class PatchAuthorRequest
{
    // Muallif ismi (ixtiyoriy)
    public string? FirstName { get; set; }

    // Muallif familiyasi (ixtiyoriy)
    public string? LastName { get; set; }

    // Yashash manzili (ixtiyoriy)
    public string? Address { get; set; }

    // Tug'ilgan sanasi (ixtiyoriy)
    public DateOnly? BirthDate { get; set; }
}