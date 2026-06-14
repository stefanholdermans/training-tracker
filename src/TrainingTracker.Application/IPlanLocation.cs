namespace TrainingTracker.Application;

/// <summary>
/// Remembers which file holds the runner's training plan, so the same file is
/// read and written across app restarts. The runner's own file is the single
/// source of truth; the app keeps no private copy of the plan.
/// </summary>
public interface IPlanLocation
{
    /// <summary>
    /// The path of the runner's plan file to read and write, or <c>null</c>
    /// when no plan has been chosen yet.
    /// </summary>
    string? FilePath { get; }

    /// <summary>
    /// Remembers the runner's chosen file as the plan to read and write from
    /// now on, including after the app restarts.
    /// </summary>
    void Remember(string filePath);
}
