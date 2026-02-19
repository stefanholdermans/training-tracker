namespace TrainingTracker.Application;

/// <summary>
/// The training plan organised as a calendar: a sequence of weeks for display.
/// </summary>
public record TrainingCalendar(IReadOnlyList<TrainingWeek> Weeks);
