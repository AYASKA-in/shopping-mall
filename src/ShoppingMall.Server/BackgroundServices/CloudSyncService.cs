using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Data.DbContext;
using SyncLog = ShoppingMall.Core.Models.SyncLog;

namespace ShoppingMall.Server.BackgroundServices;

public class CloudSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CloudSyncService> _logger;
    private readonly IConfiguration _configuration;

    public CloudSyncService(IServiceProvider serviceProvider, ILogger<CloudSyncService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("CloudSync:Enabled");
        if (!enabled)
        {
            _logger.LogInformation("Cloud sync is disabled");
            return;
        }

        var intervalMinutes = _configuration.GetValue<int>("CloudSync:BackupIntervalMinutes");
        if (intervalMinutes <= 0) intervalMinutes = 15;

        _logger.LogInformation("Cloud sync service started (interval: {Interval} min)", intervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessSyncQueueAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sync queue");
            }

            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    private async Task ProcessSyncQueueAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShoppingMallDbContext>();
        var syncRepo = scope.ServiceProvider.GetRequiredService<IRepository<SyncQueue>>();
        var syncLogRepo = scope.ServiceProvider.GetRequiredService<IRepository<SyncLog>>();

        var pendingItems = await syncRepo.FindAsync(q => q.Status == Core.Enums.SyncStatus.Pending);
        var itemsList = pendingItems.ToList();

        if (itemsList.Count == 0) return;

        var log = new Core.Models.SyncLog
        {
            Id = Guid.NewGuid(),
            Direction = Core.Enums.SyncDirection.Upload,
            Status = Core.Enums.SyncStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };
        await syncLogRepo.AddAsync(log);

        int success = 0, failed = 0;
        foreach (var item in itemsList)
        {
            try
            {
                item.Status = Core.Enums.SyncStatus.Completed;
                item.ProcessedAt = DateTime.UtcNow;
                await syncRepo.UpdateAsync(item);
                success++;
            }
            catch (Exception ex)
            {
                item.RetryCount++;
                item.ErrorMessage = ex.Message;
                if (item.RetryCount >= 5)
                    item.Status = Core.Enums.SyncStatus.Failed;
                await syncRepo.UpdateAsync(item);
                failed++;
            }
        }

        log.Status = Core.Enums.SyncStatus.Completed;
        log.ItemsProcessed = success;
        log.ItemsFailed = failed;
        log.CompletedAt = DateTime.UtcNow;
        await syncLogRepo.UpdateAsync(log);

        _logger.LogInformation("Sync completed: {Success} success, {Failed} failed", success, failed);
    }
}
