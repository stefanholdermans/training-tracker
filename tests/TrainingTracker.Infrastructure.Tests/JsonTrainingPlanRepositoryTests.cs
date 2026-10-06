using FluentAssertions;
using TrainingTracker.Application;
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
    public void ThrowsInvalidPlanExceptionForAnUnknownSessionType()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "SpeedWork", "distanceKm": 8.0 }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);

            var act = () => repository.GetAll();

            act.Should().Throw<InvalidPlanException>()
                .WithMessage("*SpeedWork*");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadsAPaceRunFromTheJsonFile()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-08", "type": "PaceRun", "distanceKm": 24.0 }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);

            repository.GetAll()[0].Session.Type.Should()
                .Be(TrainingType.PaceRun);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadsTheCompletedFlagFromTheJsonFile()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0,
                  "completed": true }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);

            repository.GetAll()[0].Completed.Should().BeTrue();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ASessionWithoutACompletedFlagIsNotCompleted()
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

            repository.GetAll()[0].Completed.Should().BeFalse();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadsTheStridesCountFromTheJsonFile()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 6.0,
                  "strides": 8 }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);

            repository.GetAll()[0].Session.Strides.Should().Be(8);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ASessionWithoutAStridesFieldHasNoStrides()
    {
        string json = """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 6.0 }
              ]
            }
            """;

        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, json);

            var repository = new JsonTrainingPlanRepository(filePath);

            repository.GetAll()[0].Session.Strides.Should().BeNull();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void SaveRoundTripsASessionsStrides()
    {
        string activePath = Path.GetTempFileName();

        try
        {
            var repository = new JsonTrainingPlanRepository(activePath);
            repository.Save(
            [
                new ScheduledSession(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 6.0m, Strides: 8))
            ]);

            new JsonTrainingPlanRepository(activePath).GetAll()[0]
                .Session.Strides.Should().Be(8);
        }
        finally
        {
            File.Delete(activePath);
        }
    }

    [Fact]
    public void SaveOmitsStridesForASessionWithoutThem()
    {
        string activePath = Path.GetTempFileName();

        try
        {
            var repository = new JsonTrainingPlanRepository(activePath);
            repository.Save(
            [
                new ScheduledSession(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 6.0m))
            ]);

            File.ReadAllText(activePath).Should().NotContain("strides");
        }
        finally
        {
            File.Delete(activePath);
        }
    }

    [Fact]
    public void ReadsTheTitleFromTheJsonFile()
    {
        string json = """
            {
              "title": "2026 Rotterdam Marathon",
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

            repository.GetTitle().Should().Be("2026 Rotterdam Marathon");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void APlanWithoutATitleHasNone()
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

            repository.GetTitle().Should().BeNull();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void GetTitleIsNullWhenTheActivePlanFileDoesNotExist()
    {
        string missingPath = Path.Combine(
            Path.GetTempPath(), $"no-such-plan-{Guid.NewGuid():N}.json");

        var repository = new JsonTrainingPlanRepository(missingPath);

        repository.GetTitle().Should().BeNull();
    }

    [Fact]
    public void SavePreservesTheTitleAlreadyInTheFile()
    {
        string filePath = Path.GetTempFileName();

        try
        {
            File.WriteAllText(filePath, """
                {
                  "title": "2026 Rotterdam Marathon",
                  "sessions": [
                    { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0 }
                  ]
                }
                """);

            var repository = new JsonTrainingPlanRepository(filePath);
            repository.Save(
            [
                new ScheduledSession(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m),
                    Completed: true)
            ]);

            new JsonTrainingPlanRepository(filePath).GetTitle()
                .Should().Be("2026 Rotterdam Marathon");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReturnsNoSessionsWhenTheActivePlanFileDoesNotExist()
    {
        string missingPath = Path.Combine(
            Path.GetTempPath(), $"no-such-plan-{Guid.NewGuid():N}.json");

        var repository = new JsonTrainingPlanRepository(missingPath);

        repository.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void SaveRoundTripsThePlanIncludingCompletion()
    {
        string activePath = Path.GetTempFileName();

        try
        {
            var repository = new JsonTrainingPlanRepository(activePath);
            repository.Save(
            [
                new ScheduledSession(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m),
                    Completed: true)
            ]);

            IReadOnlyList<ScheduledSession> reloaded =
                new JsonTrainingPlanRepository(activePath).GetAll();

            reloaded.Should().ContainSingle();
            reloaded[0].Date.Should().Be(new DateOnly(2026, 3, 2));
            reloaded[0].Session.Type.Should().Be(TrainingType.EasyRun);
            reloaded[0].Session.DistanceKm.Should().Be(5.0m);
            reloaded[0].Completed.Should().BeTrue();
        }
        finally
        {
            File.Delete(activePath);
        }
    }

    [Fact]
    public void SavingWritesToTheFileTheRunnerLoaded()
    {
        string myPlanPath = Path.GetTempFileName();
        // An initial location that is never created; loading moves off it.
        string initialPath = Path.Combine(
            Path.GetTempPath(), $"initial-{Guid.NewGuid():N}.json");

        try
        {
            File.WriteAllText(myPlanPath, """
                {
                  "sessions": [
                    { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0 }
                  ]
                }
                """);

            var repository = new JsonTrainingPlanRepository(initialPath);
            repository.Load(myPlanPath);
            repository.Save(
            [
                new ScheduledSession(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m),
                    Completed: true)
            ]);

            // The runner's own file on disk records the completion.
            new JsonTrainingPlanRepository(myPlanPath).GetAll()[0]
                .Completed.Should().BeTrue();
        }
        finally
        {
            File.Delete(myPlanPath);
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
