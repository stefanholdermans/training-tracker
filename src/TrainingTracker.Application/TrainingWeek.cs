namespace TrainingTracker.Application;

/// <summary>
/// A week in the training calendar, spanning Monday to Sunday.
/// </summary>
public record TrainingWeek(DateOnly StartDate, IReadOnlyList<TrainingDay> Days)
{
    public decimal TotalDistanceKm =>
        Days.Sum(d => d.Session?.DistanceKm ?? 0);

    /// <summary>
    /// The volume the runner has completed this week: the planned sum
    /// restricted to the sessions that have been checked off.
    /// </summary>
    public decimal CompletedDistanceKm =>
        Days.Sum(d => d.Completed ? d.Session?.DistanceKm ?? 0 : 0);
}
