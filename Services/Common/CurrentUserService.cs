using System.Security.Claims;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Services.Common;

// HttpContext orqali joriy foydalanuvchi ma'lumotlarini aniqlovchi servis
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // JWT tokendagi NameIdentifier claim'dan foydalanuvchi ID sini olish
    public int? UserId
    {
        get
        {
            var userClaimId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userClaimId, out var userId) ? userId : null;
        }
    }

    // Foydalanuvchida Admin roli bor-yo'qligini tekshirish
    public bool IsAdmin => 
        _httpContextAccessor.HttpContext?.User.IsInRole(nameof(UserRole.Admin)) ?? false;
}