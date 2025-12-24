namespace T21.Core.RClone;

public interface IRCloneCommand
{
    Task SyncAsync(
        string source,
        string destination,
        CancellationToken cancellationToken = default);

    Task<RCloneLsJsonResult> LsJsonAsync(
        string path,
        CancellationToken cancellationToken = default);

    Task CopyToAsync(
        string source,
        string destination,
        CancellationToken cancellationToken = default);
}