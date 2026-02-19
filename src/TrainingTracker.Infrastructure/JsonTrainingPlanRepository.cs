using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Infrastructure;

/// <summary>
/// Reads the training plan from a JSON file.
/// </summary>
public class JsonTrainingPlanRepository(string filePath) : ITrainingPlanRepository
{
    public IReadOnlyList<ScheduledSession> GetAll()
    {
        _ = filePath;  // Stub; will read and parse the JSON file.
        return [];
    }
}
