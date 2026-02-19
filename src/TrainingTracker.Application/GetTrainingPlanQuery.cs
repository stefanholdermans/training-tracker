using TrainingTracker.Domain;

namespace TrainingTracker.Application;

/// <summary>
/// Retrieves the training plan, grouping sessions into calendar weeks.
/// </summary>
public class GetTrainingPlanQuery(ITrainingPlanRepository repository)
    : IGetTrainingPlanQuery
{
    public TrainingCalendar Execute()
    {
        _ = repository.GetAll();  // Stub; will group sessions into weeks.
        return new([]);
    }
}
