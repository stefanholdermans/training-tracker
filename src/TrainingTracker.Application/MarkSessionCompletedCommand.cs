namespace TrainingTracker.Application;

/// <summary>
/// Marks the session scheduled on a given date as completed and persists the
/// updated plan.
/// </summary>
public class MarkSessionCompletedCommand(ITrainingPlanRepository repository)
    : IMarkSessionCompletedCommand
{
    public void Execute(DateOnly scheduledDate)
    {
        // Pending implementation: driven by unit tests next. See PLAN.md.
        _ = repository;
        _ = scheduledDate;
        throw new NotImplementedException();
    }
}
