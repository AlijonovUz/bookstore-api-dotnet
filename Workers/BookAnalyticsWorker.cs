using Microsoft.EntityFrameworkCore;
using FirstBackendApp.Data;

namespace FirstBackendApp.Workers;

// Fondagi kitoblar tahlilini yurituvchi fon xizmati (BackgroundService)
public class BookAnalyticsWorker : BackgroundService
{
    private readonly ILogger<BookAnalyticsWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BookAnalyticsWorker(ILogger<BookAnalyticsWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    // Har 30 soniyada fonda kitoblar tahlilini hisoblash va logga chiqarish
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var totalBooks = await context.Books.CountAsync(stoppingToken);
                var mostExpensiveBook = await context.Books
                    .OrderByDescending(b => b.Price)
                    .Select(b => new { b.Title, b.Price })
                    .FirstOrDefaultAsync(stoppingToken);

                if (mostExpensiveBook is not null)
                {
                    _logger.LogInformation(
                        "📊 Tahlil: Jami kitoblar: {Count} ta | Eng qimmat kitob: '{Title}' ({Price} so'm)",
                        totalBooks, mostExpensiveBook.Title, mostExpensiveBook.Price);
                }
                else
                {
                    _logger.LogInformation("📊 Tahlil: Hozircha bazada kitoblar mavjud emas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tahlil jarayonida xatolik yuz berdi!");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("Kitoblar tahlilchisi to'xtatildi.");
    }
}