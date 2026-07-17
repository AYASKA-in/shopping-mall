using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Server.BackgroundServices;

public class SessionCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionCleanupService> _logger;

    public SessionCleanupService(IServiceProvider serviceProvider, ILogger<SessionCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Session cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ShoppingMallDbContext>();

                var cutoff = DateTime.UtcNow.AddHours(-8);
                var expired = db.Sessions
                    .Where(s => s.IsActive && s.LoginAt < cutoff)
                    .ToList();

                if (expired.Count > 0)
                {
                    foreach (var session in expired)
                    {
                        session.IsActive = false;
                        session.LogoutAt = session.LoginAt.AddHours(8);
                    }
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Cleaned {Count} expired sessions", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Session cleanup cycle failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
