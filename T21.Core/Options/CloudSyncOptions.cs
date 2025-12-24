namespace T21.Core.Options;

/// <summary>
/// CloudSync application configuration model
/// </summary>
public class CloudSyncOptions
{
  public const string SectionName = "CloudSync";
  
  /// <summary>
  /// Sync tasks configuration
  /// </summary>
  public List<SyncTask> SyncTasks { get; set; } = new();
}

/// <summary>
/// Individual sync task configuration
/// </summary>
public class SyncTask
{
  /// <summary>
  /// Task display name
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Source path (rclone remote format: remote:path)
  /// </summary>
  public string Source { get; set; } = string.Empty;

  /// <summary>
  /// Destination path (rclone remote format: remote:path)
  /// </summary>
  public string Destination { get; set; } = string.Empty;

  /// <summary>
  /// Cron expression for scheduling (e.g., "0 0 * * * ?" for daily at midnight)
  /// </summary>
  public string Schedule { get; set; } = string.Empty;

  /// <summary>
  /// Whether this task is enabled
  /// </summary>
  public bool Enabled { get; set; } = true;
}
