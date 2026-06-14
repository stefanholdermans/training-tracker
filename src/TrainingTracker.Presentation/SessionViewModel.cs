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

    /// <summary>
    /// The distance shown in the calendar: whole kilometres, with the strides
    /// appended when there are some, as in "6K + 8 ST".
    /// </summary>
    public string Distance => Strides is { } strides
        ? $"{DistanceKm:0}K + {strides} ST"
        : $"{DistanceKm:0}K";
}
