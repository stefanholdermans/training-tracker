namespace TrainingTracker.Application;

/// <summary>
/// Supplies the current date, so calendar logic that depends on "today" can
/// be exercised deterministically rather than reading the system clock.
/// </summary>
public interface IClock
{
    /// <summary>
    /// The current date.
    /// </summary>
    DateOnly Today { get; }
}
