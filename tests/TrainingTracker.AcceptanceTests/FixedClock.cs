using TrainingTracker.Application;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// A real <see cref="IClock"/> pinned to a chosen date, so scenarios that
/// depend on "today" are deterministic. It is configured with a date in the
/// same spirit as the fixture files around it, not a mock of behaviour.
/// </summary>
internal sealed class FixedClock(DateOnly today) : IClock
{
    public DateOnly Today => today;
}
