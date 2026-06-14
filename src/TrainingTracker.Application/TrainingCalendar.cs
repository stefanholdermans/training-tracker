namespace TrainingTracker.Application;

/// <summary>
/// The training plan organised as a calendar: a sequence of weeks for display.
/// </summary>
public record TrainingCalendar(IReadOnlyList<TrainingWeek> Weeks)
{
    /// <summary>
    /// The date the calendar was assembled for, against which "today" is
    /// judged. Defaults to the BCL's zero date when no clock stamped it.
    /// </summary>
    public DateOnly Today { get; init; }

    /// <summary>
    /// The programme's title, such as "2026 Rotterdam Marathon", or
    /// <c>null</c> when the plan carries none.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The greatest weekly total across the calendar, or zero when empty.
    /// </summary>
    public decimal PeakWeeklyDistanceKm =>
        Weeks.Count == 0 ? 0 : Weeks.Max(w => w.TotalDistanceKm);

    /// <summary>
    /// The smallest weekly total among weeks that carry any load, or
    /// <c>null</c> when every week is a rest week.
    /// </summary>
    public decimal? LowestActiveWeeklyDistanceKm =>
        Weeks
            .Select(w => w.TotalDistanceKm)
            .Where(km => km > 0)
            .Cast<decimal?>()
            .Min();

    /// <summary>
    /// The number of planned sessions across the programme; rest days do not
    /// count.
    /// </summary>
    public int PlannedSessionCount =>
        Weeks.Sum(w => w.Days.Count(d => d.Session is not null));

    /// <summary>
    /// The number of planned sessions the runner has completed.
    /// </summary>
    public int CompletedSessionCount =>
        Weeks.Sum(w => w.Days.Count(d => d.Session is not null && d.Completed));
}
