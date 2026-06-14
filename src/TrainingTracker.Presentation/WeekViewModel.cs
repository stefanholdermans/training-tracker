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

    // Placeholder: the "completed of planned" summary follows under unit tests.
    public string ProgressSummary => $"{CompletedDistanceKm:0}K";
}
