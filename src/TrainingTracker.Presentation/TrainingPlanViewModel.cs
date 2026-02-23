using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Presentation;

/// <summary>
/// Exposes the training plan as a sequence of calendar weeks for display.
/// </summary>
public class TrainingPlanViewModel(IGetTrainingPlanQuery query)
{
    public IReadOnlyList<WeekViewModel> Weeks { get; } = MapWeeks(query.Execute());

    private static IReadOnlyList<WeekViewModel> MapWeeks(TrainingCalendar plan) =>
        [..plan.Weeks.Select(MapWeek)];

    private static WeekViewModel MapWeek(TrainingWeek week) =>
        new()
        {
            StartDate = week.StartDate,
            Days = [..week.Days.Select(MapDay)],
            TotalDistanceKm = 0
        };

    private static DayViewModel MapDay(TrainingDay day) =>
        new()
        {
            Date = day.Date,
            Session = day.Session is { } session ? MapSession(session) : null
        };

    private static SessionViewModel MapSession(TrainingSession session) =>
        new()
        {
            DisplayName = session.Type switch
            {
                TrainingType.EasyRun => "Easy Run",
                TrainingType.ThresholdRun => "Threshold Run",
                TrainingType.Repetitions => "Repetitions",
                TrainingType.Intervals => "Intervals",
                TrainingType.LongRun => "Long Run",
                TrainingType.Race => "Race",
                _ => session.Type.ToString()
            },
            Color = session.Type switch
            {
                TrainingType.EasyRun => "#4CAF80",
                TrainingType.ThresholdRun => "#E07820",
                TrainingType.Repetitions => "#C04040",
                TrainingType.Intervals => "#7050C0",
                TrainingType.LongRun => "#4080C0",
                TrainingType.Race => "#C09020",
                _ => "#808080"
            },
            DistanceKm = session.DistanceKm
        };
}
