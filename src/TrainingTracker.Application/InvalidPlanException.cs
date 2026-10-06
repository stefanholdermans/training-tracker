namespace TrainingTracker.Application;

/// <summary>
/// Thrown when a training plan file is structurally valid but contains content
/// the app does not understand — for example, an unrecognised session type.
/// </summary>
public class InvalidPlanException : Exception
{
    public InvalidPlanException() { }

    public InvalidPlanException(string message) : base(message) { }

    public InvalidPlanException(string message, Exception innerException)
        : base(message, innerException) { }
}
