namespace TrainingTracker.Application;

/// <summary>
/// An <see cref="IClock"/> backed by the machine's local date.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
