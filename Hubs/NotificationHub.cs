using Microsoft.AspNetCore.SignalR;

namespace FirstBackendApp.Hubs;

// Real-vaqt bildirishnomalarini tarqatuvchi SignalR hub
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    
    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    // Mijoz ulanganda ishga tushadi
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Yangi mijoz ulandi: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    // Mijoz uzilganda ishga tushadi
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Mijoz uzildi: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}