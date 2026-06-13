namespace TrainingTracker.Application;

/// <summary>
/// Loads a training plan from a file chosen by the runner.
/// </summary>
public interface ILoadTrainingPlanCommand
{
    void Execute(string filePath);
}
