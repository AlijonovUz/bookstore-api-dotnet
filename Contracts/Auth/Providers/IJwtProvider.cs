using FirstBackendApp.Entities;

namespace FirstBackendApp.Contracts.Auth;

// JWT token generatsiya qilish interfeysi
public interface IJwtProvider
{
    // Foydalanuvchi uchun yangi JWT token yaratish
    string Generate(User user);
}