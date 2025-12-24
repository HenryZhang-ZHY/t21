using Microsoft.Extensions.Logging;
using T21.Core.Options;
using T21.Core.RClone;

namespace T21.Core.Jobs;

/// <summary>
/// Real implementation of cloud sync job using rclone
/// </summary>
public class RCloneCloudSyncJob(IRCloneCommand rCloneCommand, ILogger<RCloneCloudSyncJob> logger) : ICloudSyncJob
{
    public async Task ExecuteSyncAsync(SyncTask syncTask, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting sync task: {TaskName} from {Source} to {Destination}",
            syncTask.Name,
            syncTask.Source, syncTask.Destination);

        try
        {
            await ExecuteSyncCoreAsync(syncTask, cancellationToken);

            logger.LogInformation("Sync task completed successfully: {TaskName}", syncTask.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing sync task {TaskName}", syncTask.Name);

            throw;
        }
    }

    async Task ExecuteSyncCoreAsync(SyncTask syncTask, CancellationToken cancellationToken)
    {
        var lsJsonResult = await rCloneCommand.LsJsonAsync(syncTask.Source);
        if (lsJsonResult.IsDir)
        {
            logger.LogInformation("Source {Source} is a directory. Performing directory sync.", syncTask.Source);

            await rCloneCommand.SyncAsync(syncTask.Source, syncTask.Destination, cancellationToken);
        }
        else
        {
            logger.LogInformation("Source {Source} is a file. Performing file copy.", syncTask.Source);
            
            await rCloneCommand.CopyToAsync(syncTask.Source, syncTask.Destination, cancellationToken);
        }
    }
}