using Microsoft.Extensions.Options;

namespace T21.Core.Options;

/// <summary>
/// Validates CloudSync configuration using IValidateOptions pattern
/// </summary>
public class ValidateCloudSyncOptions : IValidateOptions<CloudSyncOptions>
{
    public ValidateOptionsResult Validate(string? name, CloudSyncOptions options)
    {
        var errors = new List<string>();

        if (options.SyncTasks.Count == 0)
        {
            // Empty tasks list is valid
            return ValidateOptionsResult.Success;
        }

        foreach (var task in options.SyncTasks)
        {
            if (string.IsNullOrWhiteSpace(task.Name))
            {
                errors.Add("Task must have a name");
            }

            if (string.IsNullOrWhiteSpace(task.Source))
            {
                errors.Add($"Task '{task.Name}' must have a source");
            }

            if (string.IsNullOrWhiteSpace(task.Destination))
            {
                errors.Add($"Task '{task.Name}' must have a destination");
            }

            if (string.IsNullOrWhiteSpace(task.Schedule))
            {
                errors.Add($"Task '{task.Name}' must have a schedule");
            }
        }

        return errors.Count != 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}