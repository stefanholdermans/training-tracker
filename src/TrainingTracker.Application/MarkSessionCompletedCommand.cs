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
        var updated = repository.GetAll()
            .Select(session => session.Date == scheduledDate
                ? session with { Completed = true }
                : session)
            .ToList();

        repository.Save(updated);
    }
}
