using Microsoft.Extensions.Logging;
using Moq;
using T21.Core.Cli;
using T21.Core.RClone;

namespace T21.Core.Test.RClone;

public class RCloneCopyToTest
{
    private readonly Mock<ICliRunner> _mockCliRunner;
    private readonly Mock<ILogger<RCloneCommand>> _mockLogger;
    private readonly RCloneCommand _command;

    public RCloneCopyToTest()
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
    public async Task CopyToAsync_ShouldSucceedOnValidPaths()
    {
        SetupCliRunnerResponse(0);

        await _command.CopyToAsync("/source/file.txt", "/destination/file.txt", TestContext.Current.CancellationToken);

        _mockCliRunner.Verify(x => x.RunAsync(
            "rclone",
            "copyto --log-level ERROR \"/source/file.txt\" \"/destination/file.txt\"",
            null,
            It.IsAny<CancellationToken>(),
            null), Times.Once);
    }

    [Fact]
    public async Task CopyToAsync_ShouldThrowOnFailure()
    {
        SetupCliRunnerResponse(1, "", "CopyTo error occurred");

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _command.CopyToAsync("/source/file.txt", "/destination/file.txt", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CopyToAsync_ShouldQuotePathsWithSpaces()
    {
        SetupCliRunnerResponse(0);

        await _command.CopyToAsync("/source/file with spaces.txt", "/destination/file with spaces.txt", TestContext.Current.CancellationToken);

        _mockCliRunner.Verify(x => x.RunAsync(
            "rclone",
            "copyto --log-level ERROR \"/source/file with spaces.txt\" \"/destination/file with spaces.txt\"",
            null,
            It.IsAny<CancellationToken>(),
            null), Times.Once);
    }
}
