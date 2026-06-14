namespace TrainingTracker.Application;

/// <summary>
/// Marks the session scheduled on a given date as uncompleted and persists the
/// updated plan.
/// </summary>
public class MarkSessionUncompletedCommand(ITrainingPlanRepository repository)
    : IMarkSessionUncompletedCommand
{
    public void Execute(DateOnly scheduledDate)
    {
        // Pending implementation: driven by unit tests next. See PLAN.md.
        _ = repository;
        _ = scheduledDate;
        throw new NotImplementedException();
    }
}
