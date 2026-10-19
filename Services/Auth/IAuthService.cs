using FirstBackendApp.Contracts.Auth.Requests;
using FirstBackendApp.Contracts.Auth.Responses;

namespace FirstBackendApp.Services.Auth;

// Autentifikatsiya (ro'yxatdan o'tish va kirish) servisi interfeysi
public interface IAuthService
{
    // Yangi foydalanuvchini ro'yxatdan o'tkazish
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    // Tizimga kirish va JWT token olish
    Task<AuthResponse> LoginAsync(LoginRequest request);
}