using TrainingTracker.Application;

namespace TrainingTracker.Infrastructure;

/// <summary>
/// A plan location held in memory, pointing at a fixed file until told
/// otherwise. It does not survive a restart, so it suits a known file and
/// tests rather than the sandboxed app.
/// </summary>
internal sealed class FixedPlanLocation(string? filePath) : IPlanLocation
{
    public string? FilePath { get; private set; } = filePath;

    public void Remember(string newFilePath) => FilePath = newFilePath;
}
