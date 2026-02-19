namespace TrainingTracker.Domain;

/// <summary>
/// A training session: its type and planned distance.
/// </summary>
public record TrainingSession(TrainingType Type, decimal DistanceKm);
