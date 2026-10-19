namespace FirstBackendApp.Contracts.Auth.Requests;

// Tizimga kirish (login) so'rovi modeli
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}