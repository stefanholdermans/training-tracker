using TrainingTracker.Application;

namespace TrainingTracker.Presentation;

/// <summary>
/// Exposes the training plan as a sequence of calendar weeks for display.
/// </summary>
public class TrainingPlanViewModel(IGetTrainingPlanQuery query)
{
    public IReadOnlyList<WeekViewModel> Weeks { get; } = MapWeeks(query.Execute());

    private static IReadOnlyList<WeekViewModel> MapWeeks(TrainingCalendar plan)
    {
        _ = plan;  // Stub; will map training weeks to ViewModels.
        return [];
    }
}
