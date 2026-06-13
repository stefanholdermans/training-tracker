using System.Globalization;
using System.Text.Json;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Infrastructure;

/// <summary>
/// Reads the training plan from a JSON file.
/// </summary>
public class JsonTrainingPlanRepository(string filePath)
    : ITrainingPlanRepository
{
    public IReadOnlyList<ScheduledSession> GetAll()
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        using var stream = File.OpenRead(filePath);
        using var document = JsonDocument.Parse(stream);

        return [..document.RootElement
            .GetProperty("sessions")
            .EnumerateArray()
            .Select(ParseSession)];
    }

    public void Load(string sourceFilePath) =>
        File.Copy(sourceFilePath, filePath, overwrite: true);

    private static ScheduledSession ParseSession(JsonElement element)
    {
        var date = element.GetProperty("date").GetString()
            ?? throw new InvalidOperationException("Session 'date' is null.");
        var type = element.GetProperty("type").GetString()
            ?? throw new InvalidOperationException("Session 'type' is null.");
        var distanceKm = element.GetProperty("distanceKm").GetDecimal();

        return new ScheduledSession(
            DateOnly.Parse(date, CultureInfo.InvariantCulture),
            new TrainingSession(Enum.Parse<TrainingType>(type), distanceKm));
    }
}
