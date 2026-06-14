using TrainingTracker.Domain;

namespace TrainingTracker.Application;

/// <summary>
/// A single day in the training calendar, with an optional session.
/// A null session indicates a rest day. <see cref="Completed"/> records
/// whether the runner has done the session.
/// </summary>
public record TrainingDay(
    DateOnly Date, TrainingSession? Session, bool Completed = false);
