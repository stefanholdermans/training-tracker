namespace TrainingTracker.Application;

/// <summary>
/// Marks the session scheduled on a given date as completed.
/// </summary>
public interface IMarkSessionCompletedCommand
{
    void Execute(DateOnly scheduledDate);
}
