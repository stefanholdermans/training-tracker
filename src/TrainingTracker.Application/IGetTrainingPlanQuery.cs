namespace TrainingTracker.Application;

/// <summary>
/// Returns the training plan organised into calendar weeks.
/// </summary>
public interface IGetTrainingPlanQuery
{
    TrainingCalendar Execute();
}
