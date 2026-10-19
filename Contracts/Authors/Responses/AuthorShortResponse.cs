namespace FirstBackendApp.Contracts.Authors.Responses;

// Kitobga biriktirilgan muallif haqidagi qisqa ma'lumot (DTO)
public class AuthorShortResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateOnly? BirthDate { get; set; }
}