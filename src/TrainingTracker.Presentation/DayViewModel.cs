namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a single day in the training calendar.
/// </summary>
public class DayViewModel
{
    public required DateOnly Date { get; init; }

    public SessionViewModel? Session { get; init; }

    public bool IsRestDay => Session is null;

    /// <summary>
    /// Whether the runner has marked this day's session as completed.
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Whether this day is today, so the calendar can highlight where the
    /// runner is in the programme.
    /// </summary>
    public bool IsToday { get; init; }

    /// <summary>
    /// Whether this day's session was scheduled before today and left
    /// undone, so the calendar can flag where the runner has fallen behind.
    /// </summary>
    public bool IsMissed { get; init; }
}
