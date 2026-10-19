using Microsoft.EntityFrameworkCore;
using Mapster;
using FirstBackendApp.Contracts.Auth;
using FirstBackendApp.Contracts.Auth.Requests;
using FirstBackendApp.Contracts.Auth.Responses;
using FirstBackendApp.Data;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Services.Auth;

// Autentifikatsiya (ro'yxatdan o'tish va kirish) biznes mantiqi
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(AppDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    // Yangi foydalanuvchini ro'yxatdan o'tkazish
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Username bandligini tekshirish
        var usernameExists = await _context.Users
            .AnyAsync(x => x.Username.ToLower() == request.Username.ToLower());
        
        if (usernameExists)
        {
            throw new BadHttpRequestException("Ushbu foydalanuvchi nomi allaqachon band.");
        }

        // Email bandligini tekshirish (agar kiritilgan bo'lsa)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _context.Users
                .AnyAsync(x => x.Email != null && x.Email.ToLower() == request.Email.ToLower());

            if (emailExists)
            {
                throw new BadHttpRequestException("Ushbu elektron pochta manzili allaqachon band.");
            }
        }

        // Parolni BCrypt bilan xesh qilish
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = request.Adapt<User>();
        user.PasswordHash = passwordHash;
        user.Role = UserRole.User;
        user.CreatedAt = DateTime.UtcNow;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // JWT token generatsiya qilish
        var token = _jwtProvider.Generate(user);
        
        var response = user.Adapt<AuthResponse>();
        response.Token = token;
        
        return response;
    }

    // Tizimga kirish va parolni tekshirish
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username.ToLower());

        // Foydalanuvchi yoki parol noto'g'ri bo'lsa
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new BadHttpRequestException("Foydalanuvchi nomi yoki parol noto'g'ri.");
        }

        // Yangi JWT token yaratish
        var token = _jwtProvider.Generate(user);

        var response = user.Adapt<AuthResponse>();
        response.Token = token;
        
        return response;
    }
}