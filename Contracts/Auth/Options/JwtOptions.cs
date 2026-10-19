namespace FirstBackendApp.Contracts.Auth;

// JWT konfiguratsiya parametrlari modeli (appsettings.json dan o'qiladi)
public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } 
}