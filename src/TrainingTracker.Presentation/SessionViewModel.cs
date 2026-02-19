using TrainingTracker.Domain;

namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a training session for display.
/// </summary>
public class SessionViewModel
{
    public required TrainingType Type { get; init; }

    public required decimal DistanceKm { get; init; }
}
