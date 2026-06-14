using TrainingTracker.Domain;

namespace TrainingTracker.Application;

/// <summary>
/// Retrieves the training plan, grouping sessions into calendar weeks and
/// stamping the calendar with the current date.
/// </summary>
public class GetTrainingPlanQuery(
    ITrainingPlanRepository repository, IClock clock)
    : IGetTrainingPlanQuery
{
    /// <summary>
    /// Reads the plan against the machine's local date. A convenience for
    /// callers that have no need to control "today".
    /// </summary>
    public GetTrainingPlanQuery(ITrainingPlanRepository repository)
        : this(repository, new SystemClock())
    {
    }

    public TrainingCalendar Execute()
    {
        var sessions = repository.GetAll();

        if (sessions.Count == 0)
            return new([]) { Today = clock.Today };

        var firstMonday = StartOfWeek(sessions.Min(s => s.Date));
        var lastMonday = StartOfWeek(sessions.Max(s => s.Date));
        var scheduledByDate = sessions.ToDictionary(s => s.Date);

        var weeks = new List<TrainingWeek>();
        for (var monday = firstMonday;
             monday <= lastMonday;
             monday = monday.AddDays(7))
        {
            var days = Enumerable.Range(0, 7)
                .Select(i => monday.AddDays(i))
                .Select(date => MapDay(
                    date, scheduledByDate.GetValueOrDefault(date)))
                .ToList();
            weeks.Add(new TrainingWeek(monday, days));
        }

        return new TrainingCalendar(weeks) { Today = clock.Today };
    }

    private static TrainingDay MapDay(DateOnly date, ScheduledSession? scheduled)
        => scheduled is null
            ? new TrainingDay(date, null)
            : new TrainingDay(date, scheduled.Session, scheduled.Completed);

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.AddDays(-daysFromMonday);
    }
}
