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
        var sessions = repository.GetAll();

        if (sessions.Count == 0)
            return new([]);

        var firstMonday = StartOfWeek(sessions.Min(s => s.Date));
        var lastMonday = StartOfWeek(sessions.Max(s => s.Date));
        var sessionsByDate = sessions.ToDictionary(s => s.Date, s => s.Session);

        var weeks = new List<TrainingWeek>();
        for (var monday = firstMonday; monday <= lastMonday; monday = monday.AddDays(7))
        {
            var days = Enumerable.Range(0, 7)
                .Select(i => monday.AddDays(i))
                .Select(date => new TrainingDay(date, sessionsByDate.GetValueOrDefault(date)))
                .ToList();
            weeks.Add(new TrainingWeek(monday, days));
        }

        return new TrainingCalendar(weeks);
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.AddDays(-daysFromMonday);
    }
}
