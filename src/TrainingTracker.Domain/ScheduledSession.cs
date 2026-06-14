namespace TrainingTracker.Domain;

/// <summary>
/// A training session scheduled on a specific date, and whether the runner has
/// completed it.
/// </summary>
public record ScheduledSession(
    DateOnly Date, TrainingSession Session, bool Completed = false);
