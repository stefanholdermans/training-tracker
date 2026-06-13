namespace TrainingTracker.Application;

/// <summary>
/// The training plan organised as a calendar: a sequence of weeks for display.
/// </summary>
public record TrainingCalendar(IReadOnlyList<TrainingWeek> Weeks)
{
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
}
