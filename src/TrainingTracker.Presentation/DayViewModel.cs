namespace TrainingTracker.Presentation;

/// <summary>
/// Represents a single day in the training calendar.
/// </summary>
public class DayViewModel
{
    public required DateOnly Date { get; init; }

    public SessionViewModel? Session { get; init; }
}
