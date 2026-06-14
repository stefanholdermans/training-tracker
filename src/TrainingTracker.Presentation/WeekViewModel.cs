namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a week in the training calendar.
/// </summary>
public class WeekViewModel
{
    public required DateOnly StartDate { get; init; }

    public required IReadOnlyList<DayViewModel> Days { get; init; }

    public required decimal TotalDistanceKm { get; init; }

    /// <summary>
    /// The volume the runner has completed this week.
    /// </summary>
    public decimal CompletedDistanceKm { get; init; }

    public required double IntensityFraction { get; init; }

    public required string IntensityColor { get; init; }

    /// <summary>
    /// The week's completed volume against its planned volume, as in
    /// "5K of 13K". A rest week has no planned volume and so no progress to
    /// report, giving an empty summary.
    /// </summary>
    public string ProgressSummary =>
        TotalDistanceKm <= 0
            ? string.Empty
            : $"{CompletedDistanceKm:0}K of {TotalDistanceKm:0}K";
}
