using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Presentation;

/// <summary>
/// Exposes the training plan as a sequence of calendar weeks for display.
/// </summary>
public class TrainingPlanViewModel(IGetTrainingPlanQuery query)
{
    /// <summary>
    /// Smallest visible fraction, so the lowest active week still reads as
    /// effort and stays distinct from a rest week's empty bar.
    /// </summary>
    private const double MinVisibleFraction = 0.15;

    private const string NeutralColor = "#C8C8C8";
    private const string LowLoadColor = "#4DB6AC";
    private const string PeakLoadColor = "#00695C";

    public IReadOnlyList<WeekViewModel> Weeks { get; } =
        MapWeeks(query.Execute());

    private static IReadOnlyList<WeekViewModel> MapWeeks(
        TrainingCalendar plan) =>
        [..plan.Weeks.Select(week => MapWeek(
            week,
            plan.PeakWeeklyDistanceKm,
            plan.LowestActiveWeeklyDistanceKm))];

    private static WeekViewModel MapWeek(
        TrainingWeek week, decimal peak, decimal? lowestActive)
    {
        double fraction = IntensityFraction(
            week.TotalDistanceKm, peak, lowestActive);
        return new()
        {
            StartDate = week.StartDate,
            Days = [..week.Days.Select(MapDay)],
            TotalDistanceKm = week.TotalDistanceKm,
            IntensityFraction = fraction,
            IntensityColor = IntensityColor(fraction, week.TotalDistanceKm)
        };
    }

    private static double IntensityFraction(
        decimal total, decimal peak, decimal? lowestActive)
    {
        if (total <= 0 || lowestActive is not { } lowest)
        {
            return 0.0;
        }

        if (peak == lowest)
        {
            return 1.0;
        }

        double spread = (double)(total - lowest) / (double)(peak - lowest);
        return MinVisibleFraction + (1.0 - MinVisibleFraction) * spread;
    }

    private static string IntensityColor(double fraction, decimal total)
    {
        if (total <= 0)
        {
            return NeutralColor;
        }

        return Lerp(LowLoadColor, PeakLoadColor, fraction);
    }

    private static string Lerp(string from, string to, double fraction) =>
        $"#{Channel(from, to, fraction, 1):X2}" +
        $"{Channel(from, to, fraction, 3):X2}" +
        $"{Channel(from, to, fraction, 5):X2}";

    private static int Channel(
        string from, string to, double fraction, int offset)
    {
        int start = Convert.ToInt32(from.Substring(offset, 2), 16);
        int end = Convert.ToInt32(to.Substring(offset, 2), 16);
        return (int)Math.Round(start + (end - start) * fraction);
    }

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
