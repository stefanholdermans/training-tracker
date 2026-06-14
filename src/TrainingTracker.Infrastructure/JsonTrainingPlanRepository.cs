using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Infrastructure;

/// <summary>
/// Reads and writes the runner's training plan as a JSON file. The file read
/// and written is the runner's own, located via <see cref="IPlanLocation"/>;
/// the app keeps no copy of its own.
/// </summary>
public class JsonTrainingPlanRepository : ITrainingPlanRepository
{
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Optional session data, such as strides, is left out when absent.
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IPlanLocation _location;

    public JsonTrainingPlanRepository(IPlanLocation location) =>
        _location = location;

    /// <summary>
    /// Reads and writes the plan at a fixed file. A convenience for a known
    /// file that need not be remembered across restarts.
    /// </summary>
    public JsonTrainingPlanRepository(string planFilePath)
        : this(new FixedPlanLocation(planFilePath))
    {
    }

    public IReadOnlyList<ScheduledSession> GetAll()
    {
        if (_location.FilePath is not { } path || !File.Exists(path))
        {
            return [];
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        return [..document.RootElement
            .GetProperty("sessions")
            .EnumerateArray()
            .Select(ParseSession)];
    }

    public void Load(string sourceFilePath) => _location.Remember(sourceFilePath);

    public void Save(IReadOnlyList<ScheduledSession> sessions)
    {
        if (_location.FilePath is { } path)
        {
            File.WriteAllText(path, Serialize(sessions));
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
                Strides = scheduled.Session.Strides,
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
        var strides = element.TryGetProperty("strides", out var count)
            ? count.GetInt32()
            : (int?)null;
        var completed = element.TryGetProperty("completed", out var flag)
            && flag.GetBoolean();

        return new ScheduledSession(
            DateOnly.Parse(date, CultureInfo.InvariantCulture),
            new TrainingSession(
                Enum.Parse<TrainingType>(type), distanceKm, strides),
            completed);
    }
}
