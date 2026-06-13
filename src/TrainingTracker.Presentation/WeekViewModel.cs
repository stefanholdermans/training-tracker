namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a week in the training calendar.
/// </summary>
public class WeekViewModel
{
    public required DateOnly StartDate { get; init; }

    public required IReadOnlyList<DayViewModel> Days { get; init; }

    public required decimal TotalDistanceKm { get; init; }

    public double IntensityFraction { get; init; }

    public string IntensityColor { get; init; } = "#000000";
}
