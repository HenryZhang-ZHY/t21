using Microsoft.Extensions.Logging;
using T21.Core.Options;

namespace T21.Core.Jobs;

/// <summary>
/// Fake implementation of cloud sync job for testing without calling rclone
/// </summary>
public class FakeCloudSyncJob(ILogger<FakeCloudSyncJob> logger) : ICloudSyncJob
{
    public async Task ExecuteSyncAsync(SyncTask syncTask, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[Fake] Starting sync from {Source} to {Destination}",
            syncTask.Source, syncTask.Destination);

        // Simulate sync operation
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        logger.LogInformation("[Fake] Transferred: 100 files, 500 MB");
        logger.LogInformation("[Fake] Sync completed successfully for task: {TaskName}", syncTask.Name);
    }
}