using Quartz;

namespace T21.Core.Jobs;

[DisallowConcurrentExecution]
public sealed class CloudSyncJob(ICloudSyncJob cloudSyncJob) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var syncTask = new Options.SyncTask
        {
            Name = dataMap.GetString("Name") ?? string.Empty,
            Source = dataMap.GetString("Source") ?? string.Empty,
            Destination = dataMap.GetString("Destination") ?? string.Empty,
            Schedule = dataMap.GetString("Schedule") ?? string.Empty,
            Enabled = dataMap.GetBooleanValue("Enabled")
        };

        return cloudSyncJob.ExecuteSyncAsync(syncTask, context.CancellationToken);
    }
}
