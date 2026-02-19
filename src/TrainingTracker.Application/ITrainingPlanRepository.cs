namespace TrainingTracker.Application;

/// <summary>
/// Provides access to the stored training plan sessions.
/// </summary>
public interface ITrainingPlanRepository
{
    IReadOnlyList<ScheduledSession> GetAll();
}
