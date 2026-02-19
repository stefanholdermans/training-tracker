namespace TrainingTracker.Application;

/// <summary>
/// A single day in the training calendar, with an optional session.
/// A null session indicates a rest day.
/// </summary>
public record TrainingDay(DateOnly Date, TrainingSession? Session);
