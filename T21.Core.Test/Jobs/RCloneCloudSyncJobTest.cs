using Microsoft.Extensions.Logging;
using Moq;
using T21.Core.Jobs;
using T21.Core.Options;
using T21.Core.RClone;

namespace T21.Core.Test.Jobs;

public class RCloneCloudSyncJobTest
{
    private readonly Mock<IRCloneCommand> _mockRCloneCommand;
    private readonly Mock<ILogger<RCloneCloudSyncJob>> _mockLogger;
    private readonly RCloneCloudSyncJob _job;

    public RCloneCloudSyncJobTest()
    {
        _mockRCloneCommand = new Mock<IRCloneCommand>();
        _mockLogger = new Mock<ILogger<RCloneCloudSyncJob>>();
        _job = new RCloneCloudSyncJob(_mockRCloneCommand.Object, _mockLogger.Object);
    }

    private static SyncTask CreateSyncTask(string name = "TestTask", string source = "/source", string destination = "/destination")
    {
        return new SyncTask
        {
            Name = name,
            Source = source,
            Destination = destination,
            Enabled = true
        };
    }

    [Fact]
    public async Task ExecuteSyncAsync_ShouldCallSyncAsync_WhenSourceIsDirectory()
    {
        var syncTask = CreateSyncTask();
        _mockRCloneCommand
            .Setup(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RCloneLsJsonResult { IsDir = true });

        await _job.ExecuteSyncAsync(syncTask, TestContext.Current.CancellationToken);

        _mockRCloneCommand.Verify(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()), Times.Once);
        _mockRCloneCommand.Verify(x => x.SyncAsync(syncTask.Source, syncTask.Destination, It.IsAny<CancellationToken>()), Times.Once);
        _mockRCloneCommand.Verify(x => x.CopyToAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteSyncAsync_ShouldCallCopyToAsync_WhenSourceIsFile()
    {
        var syncTask = CreateSyncTask();
        _mockRCloneCommand
            .Setup(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RCloneLsJsonResult { IsDir = false });

        await _job.ExecuteSyncAsync(syncTask, TestContext.Current.CancellationToken);

        _mockRCloneCommand.Verify(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()), Times.Once);
        _mockRCloneCommand.Verify(x => x.CopyToAsync(syncTask.Source, syncTask.Destination, It.IsAny<CancellationToken>()), Times.Once);
        _mockRCloneCommand.Verify(x => x.SyncAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteSyncAsync_ShouldThrowException_WhenLsJsonAsyncFails()
    {
        var syncTask = CreateSyncTask();
        _mockRCloneCommand
            .Setup(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("lsjson failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _job.ExecuteSyncAsync(syncTask, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteSyncAsync_ShouldThrowException_WhenSyncAsyncFails()
    {
        var syncTask = CreateSyncTask();
        _mockRCloneCommand
            .Setup(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RCloneLsJsonResult { IsDir = true });
        _mockRCloneCommand
            .Setup(x => x.SyncAsync(syncTask.Source, syncTask.Destination, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("sync failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _job.ExecuteSyncAsync(syncTask, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteSyncAsync_ShouldThrowException_WhenCopyToAsyncFails()
    {
        var syncTask = CreateSyncTask();
        _mockRCloneCommand
            .Setup(x => x.LsJsonAsync(syncTask.Source, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RCloneLsJsonResult { IsDir = false });
        _mockRCloneCommand
            .Setup(x => x.CopyToAsync(syncTask.Source, syncTask.Destination, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("copyto failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _job.ExecuteSyncAsync(syncTask, TestContext.Current.CancellationToken));
    }
}
