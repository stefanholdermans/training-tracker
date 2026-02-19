namespace TrainingTracker.Application;

/// <summary>
/// The complete training plan, organised as a sequence of weeks.
/// </summary>
public record TrainingPlan(IReadOnlyList<TrainingWeek> Weeks);
