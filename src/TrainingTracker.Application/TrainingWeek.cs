namespace TrainingTracker.Application;

/// <summary>
/// A week in the training calendar, spanning Monday to Sunday.
/// </summary>
public record TrainingWeek(DateOnly StartDate, IReadOnlyList<TrainingDay> Days)
{
    public decimal TotalDistanceKm =>
        Days.Sum(d => d.Session?.DistanceKm ?? 0);
}
