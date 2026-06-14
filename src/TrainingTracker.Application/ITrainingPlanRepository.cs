using TrainingTracker.Domain;

namespace TrainingTracker.Application;

/// <summary>
/// Provides access to the stored training plan sessions.
/// </summary>
public interface ITrainingPlanRepository
{
    IReadOnlyList<ScheduledSession> GetAll();

    /// <summary>
    /// The active plan's title, or <c>null</c> when it carries none.
    /// </summary>
    string? GetTitle();

    /// <summary>
    /// Adopts the training plan at the given path as the active plan, so that
    /// subsequent reads return it.
    /// </summary>
    void Load(string sourceFilePath);

    /// <summary>
    /// Persists the given sessions as the active plan, writing the changes
    /// through to the runner's own plan file when one has been loaded.
    /// </summary>
    void Save(IReadOnlyList<ScheduledSession> sessions);
}
