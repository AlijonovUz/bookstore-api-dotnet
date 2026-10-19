namespace FirstBackendApp.Contracts.Auth.Requests;

// Yangi foydalanuvchini ro'yxatdan o'tkazish so'rovi modeli
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; } 
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
}