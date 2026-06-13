using FluentAssertions;
using TrainingTracker.Domain;
using TrainingTracker.Infrastructure;

namespace TrainingTracker.Infrastructure.Tests;

/// <summary>
/// Given a training plan stored as a JSON file on disk.
/// </summary>
public class JsonTrainingPlanRepositoryTests
{
    [Fact]
    public void ReturnsSessionsParsedFromJsonFile()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0 }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);
            IReadOnlyList<ScheduledSession> sessions = repository.GetAll();

            sessions.Should().HaveCount(1);
            sessions[0].Date.Should().Be(new DateOnly(2026, 3, 2));
            sessions[0].Session.Type.Should().Be(TrainingType.EasyRun);
            sessions[0].Session.DistanceKm.Should().Be(5.0m);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void AdoptsThePlanAtTheGivenPathAsTheActivePlan()
    {
        string activePath = Path.GetTempFileName();
        string newPlanPath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(activePath, """
                {
                  "sessions": [
                    { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0 }
                  ]
                }
                """);
            File.WriteAllText(newPlanPath, """
                {
                  "sessions": [
                    { "date": "2026-09-07", "type": "LongRun", "distanceKm": 22.0 }
                  ]
                }
                """);

            var repository = new JsonTrainingPlanRepository(activePath);
            repository.Load(newPlanPath);
            IReadOnlyList<ScheduledSession> sessions = repository.GetAll();

            sessions.Should().HaveCount(1);
            sessions[0].Date.Should().Be(new DateOnly(2026, 9, 7));
            sessions[0].Session.Type.Should().Be(TrainingType.LongRun);
            sessions[0].Session.DistanceKm.Should().Be(22.0m);
        }
        finally
        {
            File.Delete(activePath);
            File.Delete(newPlanPath);
        }
    }
}
