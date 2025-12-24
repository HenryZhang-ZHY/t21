namespace T21.Core.Cli;

public readonly record struct CliResult(
    int ExitCode,
    string StandardOutput,
    string StandardError)
{
    public bool Success => ExitCode == 0;
}