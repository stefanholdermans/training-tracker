namespace TrainingTracker.Application;

/// <summary>
/// Marks the session scheduled on a given date as uncompleted, undoing a
/// check-off the runner made by mistake.
/// </summary>
public interface IMarkSessionUncompletedCommand
{
    void Execute(DateOnly scheduledDate);
}
