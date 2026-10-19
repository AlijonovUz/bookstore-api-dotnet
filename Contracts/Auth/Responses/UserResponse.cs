using FirstBackendApp.Extensions;

namespace FirstBackendApp.Contracts.Auth.Responses;

// Foydalanuvchi haqidagi ma'lumot javob modeli (DTO)
public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Toshkent vaqtiga o'girilgan yaratilish vaqti
    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value.ToTashkentTime();
    }
}