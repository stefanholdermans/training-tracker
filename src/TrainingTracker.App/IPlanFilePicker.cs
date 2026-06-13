namespace TrainingTracker.App;

/// <summary>
/// Presents a native file picker for choosing a training plan file.
/// </summary>
public interface IPlanFilePicker
{
    /// <summary>
    /// Shows the picker and returns a readable local path to the chosen file,
    /// or <c>null</c> if the runner cancelled.
    /// </summary>
    Task<string?> PickAsync();
}
