namespace FirstBackendApp.Contracts.Auth.Responses;

// Autentifikatsiya (login/register) muvaffaqiyatli bo'lganda qaytadigan javob modeli (DTO)
public class AuthResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}