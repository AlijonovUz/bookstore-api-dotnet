using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FirstBackendApp.Entities;
using FirstBackendApp.Hubs;

namespace FirstBackendApp.Interceptors;

// Yangi kitob qo'shilganda SignalR orqali xabar yuboruvchi EF Core interceptor
public class BookNotificationInterceptor : SaveChangesInterceptor
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private List<string> _addedBookTitles = [];

    public BookNotificationInterceptor(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Saqlashdan oldin yangi qo'shilayotgan (EntityState.Added) kitoblarni aniqlab olish
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            _addedBookTitles = eventData.Context.ChangeTracker
                .Entries<Book>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity.Title)
                .ToList();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // Baza muvaffaqiyatli saqlangach, yangi kitoblar haqida SignalR mijozlariga bildirishnoma yuborish
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_addedBookTitles.Count > 0)
        {
            foreach (var title in _addedBookTitles)
            {
                await _hubContext.Clients.All.SendAsync(
                    "ReceiveNotification",
                    $"Yangi kitob qo'shildi: {title}",
                    cancellationToken);
            }
            
            _addedBookTitles.Clear();
        }
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}