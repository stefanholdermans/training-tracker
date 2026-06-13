namespace TrainingTracker.Application;

/// <summary>
/// Adopts the plan in the chosen file as the active training plan.
/// </summary>
public class LoadTrainingPlanCommand(ITrainingPlanRepository repository)
    : ILoadTrainingPlanCommand
{
    public void Execute(string filePath) => repository.Load(filePath);
}
