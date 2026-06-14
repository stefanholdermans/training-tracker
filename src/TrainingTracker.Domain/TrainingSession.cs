namespace TrainingTracker.Domain;

/// <summary>
/// A training session: its type, planned distance, and an optional number of
/// strides (short accelerations of roughly 100 m) tacked onto the run.
/// </summary>
public record TrainingSession(
    TrainingType Type, decimal DistanceKm, int? Strides = null);
