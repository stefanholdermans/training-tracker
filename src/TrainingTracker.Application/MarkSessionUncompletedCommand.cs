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
        var updated = repository.GetAll()
            .Select(session => session.Date == scheduledDate
                ? session with { Completed = false }
                : session)
            .ToList();

        repository.Save(updated);
    }
}
