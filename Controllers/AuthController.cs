using Microsoft.AspNetCore.Mvc;
using FirstBackendApp.Contracts.Auth.Requests;
using FirstBackendApp.Contracts.Auth.Responses;
using FirstBackendApp.Services.Auth;

namespace FirstBackendApp.Controllers;

// Autentifikatsiya (ro'yxatdan o'tish va kirish) controlleri
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // 1. POST: api/auth/register - Yangi foydalanuvchini ro'yxatdan o'tkazish
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    // 2. POST: api/auth/login - Tizimga kirish va token olish
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}