using FluentValidation;
using FluentValidation.AspNetCore;
using FirstBackendApp.Contracts.Auth;
using FirstBackendApp.Extensions;
using FirstBackendApp.Hubs;

// .env faylidagi muhit o'zgaruvchilarini tizim xotirasiga yuklash
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Asosiy Web API va HTTP servislari
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Marshrut sozlamalari (kichik harflarda)
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Ma'lumotlar bazasi va interceptorlar
builder.Services.AddDatabase(builder.Configuration);

// Ilova servislari va avtomatik validatsiya
builder.Services.AddApplicationServices();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Infratuzilma servislari (SignalR, Xotira keshi, Fon xizmati)
builder.Services.AddInfrastructureServices();

// Rate Limiting va Health Checks
builder.Services.AddAppRateLimiting();
builder.Services.AddAppHealthChecks();

// Swagger va JWT hujjatlashtirish
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CamelCaseParameterFilter>();
});

// JWT parametrlarini bog'lash va sozlash
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// CLI buyruqlarini tekshirish (--manage-users)
if (await app.HandleCliCommandsAsync(args))
{
    return; 
}

// HTTP so'rovlar quvuri (Pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Xatoliklarni qayta ishlash va xavfsizlik
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseStaticFiles();

// Autentifikatsiya, avtorizatsiya va cheklovlar
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Endpointlar va Hublar xaritasi
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");
app.MapHealthChecks("/health");

app.Run();