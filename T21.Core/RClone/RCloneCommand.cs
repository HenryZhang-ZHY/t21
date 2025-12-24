using Microsoft.Extensions.Logging;
using T21.Core.Cli;

namespace T21.Core.RClone;

public class RCloneCommand(ICliRunner cliRunner, ILogger<RCloneCommand> logger) : IRCloneCommand
{
    async Task<CliResult> ExecuteAsync(
        string subcommand,
        string arguments,
        CancellationToken cancellationToken = default)
    {
        var finalArguments = $"{subcommand} --log-level ERROR {arguments}";

        logger.LogInformation("Executing: rclone {Arguments}", finalArguments);

        var cliResult = await cliRunner.RunAsync
        ("rclone",
            finalArguments,
            workingDirectory: null,
            cancellationToken
        );


        if (!cliResult.Success)
        {
            logger.LogError("rclone command failed. StdOut: {StdOut}, StdErr: {StdErr}",
                cliResult.StandardOutput, cliResult.StandardError);
        }
        else
        {
            logger.LogInformation("rclone command succeeded. StdOut: {StdOut}",
                cliResult.StandardOutput);
        }


        return cliResult;
    }

    /// <summary>
    /// https://rclone.org/commands/rclone_sync/
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SyncAsync(
        string source,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var cliResult = await ExecuteAsync("sync", $"\"{source}\" \"{destination}\"", cancellationToken);
        if (!cliResult.Success)
        {
            throw new InvalidOperationException("sync failed.");
        }
    }

    /// <summary>
    /// https://rclone.org/commands/rclone_lsjson/
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<RCloneLsJsonResult> LsJsonAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var cliResult = await ExecuteAsync("lsjson", $"--stat {path}", cancellationToken);
        if (!cliResult.Success)
        {
            throw new InvalidOperationException("lsjson failed.");
        }

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        var result = System.Text.Json.JsonSerializer.Deserialize<RCloneLsJsonResult>(
            cliResult.StandardOutput, options);

        return result;
    }

    /// <summary>
    /// https://rclone.org/commands/rclone_copyto/
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task CopyToAsync(
        string source,
        string destination,
        CancellationToken cancellationToken = default)
    {
        var cliResult = await ExecuteAsync("copyto", $"\"{source}\" \"{destination}\"", cancellationToken);
        if (!cliResult.Success)
        {
            throw new InvalidOperationException("copyto failed.");
        }
    }
}