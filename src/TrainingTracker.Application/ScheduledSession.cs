namespace TrainingTracker.Application;

/// <summary>
/// A training session scheduled on a specific date.
/// </summary>
public record ScheduledSession(DateOnly Date, TrainingSession Session);
