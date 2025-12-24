using T21.Core.Options;

namespace T21.Core.Jobs;

/// <summary>
/// Interface for cloud sync job execution
/// </summary>
public interface ICloudSyncJob
{
    /// <summary>
    /// Execute sync operation for a specific task
    /// </summary>
    /// <param name="syncTask">The sync task configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExecuteSyncAsync(SyncTask syncTask, CancellationToken cancellationToken = default);
}
