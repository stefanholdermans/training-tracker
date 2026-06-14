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
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // The runner's own file, remembered when a plan is loaded, so completions
    // can be written through to it as well as to the active copy.
    private string? _sourceFilePath;

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

    public void Load(string sourceFilePath)
    {
        File.Copy(sourceFilePath, filePath, overwrite: true);
        _sourceFilePath = sourceFilePath;
    }

    public void Save(IReadOnlyList<ScheduledSession> sessions)
    {
        string json = Serialize(sessions);

        File.WriteAllText(filePath, json);
        if (_sourceFilePath is { } source && source != filePath)
        {
            File.WriteAllText(source, json);
        }
    }

    private static string Serialize(IReadOnlyList<ScheduledSession> sessions)
    {
        var document = new
        {
            Sessions = sessions.Select(scheduled => new
            {
                Date = scheduled.Date.ToString(
                    "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Type = scheduled.Session.Type.ToString(),
                DistanceKm = scheduled.Session.DistanceKm,
                Completed = scheduled.Completed
            })
        };

        return JsonSerializer.Serialize(document, s_serializerOptions);
    }

    private static ScheduledSession ParseSession(JsonElement element)
    {
        var date = element.GetProperty("date").GetString()
            ?? throw new InvalidOperationException("Session 'date' is null.");
        var type = element.GetProperty("type").GetString()
            ?? throw new InvalidOperationException("Session 'type' is null.");
        var distanceKm = element.GetProperty("distanceKm").GetDecimal();
        var completed = element.TryGetProperty("completed", out var flag)
            && flag.GetBoolean();

        return new ScheduledSession(
            DateOnly.Parse(date, CultureInfo.InvariantCulture),
            new TrainingSession(Enum.Parse<TrainingType>(type), distanceKm),
            completed);
    }
}
