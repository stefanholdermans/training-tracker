namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a training session for display.
/// </summary>
public class SessionViewModel
{
    public required string DisplayName { get; init; }

    public required string Color { get; init; }

    public required decimal DistanceKm { get; init; }

    /// <summary>
    /// The optional number of strides tacked onto the run, or null for none.
    /// </summary>
    public int? Strides { get; init; }

    // Placeholder: the strides-aware display text follows under unit tests.
    public string Distance => $"{DistanceKm:0}K";
}
