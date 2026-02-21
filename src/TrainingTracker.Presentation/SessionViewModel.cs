namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a training session for display.
/// </summary>
public class SessionViewModel
{
    public required string DisplayName { get; init; }

    public required string Color { get; init; }

    public required decimal DistanceKm { get; init; }
}
