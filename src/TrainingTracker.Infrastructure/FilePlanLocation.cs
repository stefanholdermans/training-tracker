using TrainingTracker.Application;

namespace TrainingTracker.Infrastructure;

/// <summary>
/// Remembers the runner's plan file by recording its path in a small pointer
/// file, so the same plan is found again after a restart. This is the
/// structural twin of the security-scoped bookmark used inside the macOS
/// sandbox, and serves any ordinary filesystem.
/// </summary>
public sealed class FilePlanLocation(string pointerFilePath) : IPlanLocation
{
    public string? FilePath =>
        File.Exists(pointerFilePath) ? File.ReadAllText(pointerFilePath) : null;

    public void Remember(string filePath) =>
        File.WriteAllText(pointerFilePath, filePath);
}
