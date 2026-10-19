using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FirstBackendApp.Commands.Cli;
using FirstBackendApp.Contracts.Auth;
using FirstBackendApp.Data;
using FirstBackendApp.Hubs;
using FirstBackendApp.Interceptors;
using FirstBackendApp.Services.Auth;
using FirstBackendApp.Services.Authors;
using FirstBackendApp.Services.Books;
using FirstBackendApp.Services.Common;
using FirstBackendApp.Workers;

namespace FirstBackendApp.Extensions;

// Servislarni ro'yxatdan o'tkazish uchun kengaytiruvchi (extension) metodlar
public static class DependencyInjection
{
    // Ma'lumotlar bazasi (SQLite) va interceptorlarni ulash
    public static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<BookNotificationInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            // .env yoki konfiguratsiyadan DefaultConnection ulanish qatorini o'qish
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                   ?? throw new InvalidOperationException("Baza ulanishi (DefaultConnection) topilmadi.");
            options.UseSqlite(connectionString);

            var interceptor = sp.GetRequiredService<BookNotificationInterceptor>();
            options.AddInterceptors(interceptor);
        });

        return services;
    }

    // Infratuzilma servislari (SignalR, Xotira keshi, Fon xizmati)
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSignalR();
        services.AddHostedService<BookAnalyticsWorker>();

        return services;
    }

    // 1. Rate Limiter xizmatini sozlash
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("fixed-by-ip", httpContext =>
            {
                var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: clientIp,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromSeconds(30),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }

    // 2. Health Checks xizmatini sozlash (Baza holati bilan birga)
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        return services;
    }

    // JWT Bearer autentifikatsiyasini sozlash
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                          ?? throw new InvalidOperationException("JwtSettings topilmadi.");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    // Biznes servislarni DI konteynerga qo'shish
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        return services;
    }

    // Swagger uchun JWT Bearer avtorizatsiya maydonini qo'shish
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", 
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    // CLI buyruqlarini boshqarish (--manage-users)
    public static async Task<bool> HandleCliCommandsAsync(this WebApplication app, string[] args)
    {
        if (args.Contains("--manage-users"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await AdminCliManager.RunAsync(context);
            return true;
        }

        return false; 
    }
}