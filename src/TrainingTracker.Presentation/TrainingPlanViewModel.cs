using System.Collections.ObjectModel;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Presentation;

/// <summary>
/// Exposes the training plan as a sequence of calendar weeks for display.
/// </summary>
public class TrainingPlanViewModel
{
    /// <summary>
    /// Smallest visible fraction, so the lowest active week still reads as
    /// effort and stays distinct from a rest week's empty bar.
    /// </summary>
    private const double MinVisibleFraction = 0.15;

    private const string NeutralColor = "#C8C8C8";
    private const string LowLoadColor = "#4DB6AC";
    private const string PeakLoadColor = "#00695C";

    private readonly IGetTrainingPlanQuery _query;
    private readonly ILoadTrainingPlanCommand _loadCommand;
    private readonly IMarkSessionCompletedCommand _markCompletedCommand;
    private readonly IMarkSessionUncompletedCommand _markUncompletedCommand;

    /// <summary>
    /// A single observable collection mutated in place, so the bound
    /// CollectionView redraws when a new plan is loaded rather than relying on
    /// the ItemsSource reference being swapped.
    /// </summary>
    private readonly ObservableCollection<WeekViewModel> _weeks = [];

    private int _plannedSessionCount;
    private int _completedSessionCount;
    private string _title = string.Empty;

    public TrainingPlanViewModel(
        IGetTrainingPlanQuery query,
        ILoadTrainingPlanCommand loadCommand,
        IMarkSessionCompletedCommand markCompletedCommand,
        IMarkSessionUncompletedCommand markUncompletedCommand)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(loadCommand);
        ArgumentNullException.ThrowIfNull(markCompletedCommand);
        ArgumentNullException.ThrowIfNull(markUncompletedCommand);

        _query = query;
        _loadCommand = loadCommand;
        _markCompletedCommand = markCompletedCommand;
        _markUncompletedCommand = markUncompletedCommand;
        Populate(query.Execute());
    }

    public IReadOnlyList<WeekViewModel> Weeks => _weeks;

    /// <summary>
    /// The programme's title for the page header; empty when the plan carries
    /// none.
    /// </summary>
    public string Title => _title;

    /// <summary>
    /// The number of planned sessions across the programme; rest days do not
    /// count.
    /// </summary>
    public int PlannedSessionCount => _plannedSessionCount;

    /// <summary>
    /// The number of planned sessions the runner has completed.
    /// </summary>
    public int CompletedSessionCount => _completedSessionCount;

    /// <summary>
    /// A short summary of adherence, such as "3 of 5 sessions completed".
    /// </summary>
    public string AdherenceSummary =>
        $"{_completedSessionCount} of {_plannedSessionCount} sessions completed";

    /// <summary>
    /// Loads the plan from the chosen file and refreshes the calendar.
    /// </summary>
    public void LoadPlan(string filePath)
    {
        _loadCommand.Execute(filePath);
        Populate(_query.Execute());
    }

    /// <summary>
    /// Marks the session on the given date as completed and refreshes the
    /// calendar.
    /// </summary>
    public void MarkCompleted(DateOnly date)
    {
        _markCompletedCommand.Execute(date);
        Populate(_query.Execute());
    }

    /// <summary>
    /// Marks the session on the given date as uncompleted and refreshes the
    /// calendar.
    /// </summary>
    public void MarkUncompleted(DateOnly date)
    {
        _markUncompletedCommand.Execute(date);
        Populate(_query.Execute());
    }

    private void Populate(TrainingCalendar plan)
    {
        _weeks.Clear();
        foreach (WeekViewModel week in MapWeeks(plan))
        {
            _weeks.Add(week);
        }

        _plannedSessionCount = plan.PlannedSessionCount;
        _completedSessionCount = plan.CompletedSessionCount;
        _title = plan.Title ?? string.Empty;
    }

    private static IReadOnlyList<WeekViewModel> MapWeeks(
        TrainingCalendar plan) =>
        [..plan.Weeks.Select(week => MapWeek(
            week,
            plan.PeakWeeklyDistanceKm,
            plan.LowestActiveWeeklyDistanceKm,
            plan.Today))];

    private static WeekViewModel MapWeek(
        TrainingWeek week, decimal peak, decimal? lowestActive, DateOnly today)
    {
        double fraction = IntensityFraction(
            week.TotalDistanceKm, peak, lowestActive);
        return new()
        {
            StartDate = week.StartDate,
            Days = [..week.Days.Select(day => MapDay(day, today))],
            TotalDistanceKm = week.TotalDistanceKm,
            CompletedDistanceKm = week.CompletedDistanceKm,
            IntensityFraction = fraction,
            IntensityColor = IntensityColor(fraction, week.TotalDistanceKm),
            OverviewHeightFraction =
                OverviewHeightFraction(week.TotalDistanceKm, peak)
        };
    }

    private static double OverviewHeightFraction(decimal total, decimal peak) =>
        peak <= 0 ? 0.0 : (double)(total / peak);

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

    private static DayViewModel MapDay(TrainingDay day, DateOnly today) =>
        new()
        {
            Date = day.Date,
            Session = day.Session is { } session ? MapSession(session) : null,
            IsCompleted = day.Completed,
            IsToday = day.Date == today,
            IsMissed =
                day.Session is not null && !day.Completed && day.Date < today
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
            DistanceKm = session.DistanceKm,
            Strides = session.Strides
        };
}
