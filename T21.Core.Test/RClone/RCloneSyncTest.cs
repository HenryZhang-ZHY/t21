using Microsoft.Extensions.Logging;
using Moq;
using T21.Core.Cli;
using T21.Core.RClone;

namespace T21.Core.Test.RClone;

public class RCloneSyncTest
{
    private readonly Mock<ICliRunner> _mockCliRunner;
    private readonly Mock<ILogger<RCloneCommand>> _mockLogger;
    private readonly RCloneCommand _command;

    public RCloneSyncTest()
    {
        _mockCliRunner = new Mock<ICliRunner>();
        _mockLogger = new Mock<ILogger<RCloneCommand>>();
        _command = new RCloneCommand(_mockCliRunner.Object, _mockLogger.Object);
    }

    private void SetupCliRunnerResponse(int exitCode, string output = "", string error = "")
    {
        _mockCliRunner
            .Setup(x => x.RunAsync(
                "rclone",
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>(),
                null))
            .ReturnsAsync(new CliResult(exitCode, output, error));
    }

    [Fact]
    public async Task SyncAsync_ShouldSucceedOnValidPaths()
    {
        SetupCliRunnerResponse(0);

        await _command.SyncAsync("/source", "/destination", TestContext.Current.CancellationToken);

        _mockCliRunner.Verify(x => x.RunAsync(
            "rclone",
            "sync --log-level ERROR \"/source\" \"/destination\"",
            null,
            It.IsAny<CancellationToken>(),
            null), Times.Once);
    }

    [Fact]
    public async Task SyncAsync_ShouldThrowOnFailure()
    {
        SetupCliRunnerResponse(1, "", "Sync error occurred");

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _command.SyncAsync("/source", "/destination", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SyncAsync_ShouldQuotePathsWithSpaces()
    {
        SetupCliRunnerResponse(0);

        await _command.SyncAsync("/source with spaces", "/destination with spaces", TestContext.Current.CancellationToken);

        _mockCliRunner.Verify(x => x.RunAsync(
            "rclone",
            "sync --log-level ERROR \"/source with spaces\" \"/destination with spaces\"",
            null,
            It.IsAny<CancellationToken>(),
            null), Times.Once);
    }
}
