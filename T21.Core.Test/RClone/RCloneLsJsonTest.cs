using Microsoft.Extensions.Logging;
using Moq;
using T21.Core.Cli;
using T21.Core.RClone;

namespace T21.Core.Test.RClone;

public class RCloneLsJsonTest
{
    private readonly Mock<ICliRunner> _mockCliRunner;
    private readonly Mock<ILogger<RCloneCommand>> _mockLogger;
    private readonly RCloneCommand _command;

    public RCloneLsJsonTest()
    {
        _mockCliRunner = new Mock<ICliRunner>();
        _mockLogger = new Mock<ILogger<RCloneCommand>>();
        _command = new RCloneCommand(_mockCliRunner.Object, _mockLogger.Object);
    }

    private void SetupCliRunnerResponse(int exitCode, string output, string error = "")
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
    public async Task RCloneCommand_SyncAsync_ShouldThrowOnFailure()
    {
        SetupCliRunnerResponse(1, "", "Error occurred");

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _command.SyncAsync("/source", "/destination", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RCloneCommand_LsJsonAsync_ShouldReturnResult()
    {
        SetupCliRunnerResponse(0, """{"IsDir":true}""");

        var result = await _command.LsJsonAsync("/test/path", TestContext.Current.CancellationToken);

        Assert.True(result.IsDir);
        _mockCliRunner.Verify(x => x.RunAsync(
            "rclone",
            It.Is<string>(s => s.Contains("lsjson") && s.Contains("--stat") && s.Contains("/test/path")),
            null,
            It.IsAny<CancellationToken>(),
            null), Times.Once);
    }

    [Fact]
    public async Task RCloneCommand_LsJsonAsync_ShouldParseFileResult()
    {
        SetupCliRunnerResponse(0, """{"IsDir":false}""");

        var result = await _command.LsJsonAsync("/test/file.txt", TestContext.Current.CancellationToken);

        Assert.False(result.IsDir);
    }

    [Fact]
    public async Task RCloneCommand_LsJsonAsync_ShouldParseUnknownProperties()
    {
        SetupCliRunnerResponse(0, """{"Path":"","Name":"","Size":-1,"MimeType":"inode/directory","ModTime":"2025-12-12T08:32:40Z","IsDir":true}""");

        var result = await _command.LsJsonAsync("/test/unknown", TestContext.Current.CancellationToken);

        Assert.True(result.IsDir);
    }
}
