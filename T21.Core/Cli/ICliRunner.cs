namespace T21.Core.Cli;

public interface ICliRunner
{
    Task<CliResult> RunAsync(
        string fileName,
        string arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string?>? environmentVariables = null);
}