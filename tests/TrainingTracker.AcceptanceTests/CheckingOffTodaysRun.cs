using System.Text.Json;
using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given the runner's own plan, loaded from a file on disk into the in-sandbox
/// active plan, with an easy run scheduled for the 7th and intervals for the
/// 10th.
/// </summary>
public sealed class CheckingOffTodaysRun : IDisposable
{
    private static readonly DateOnly EasyRunDay = new(2026, 9, 7);
    private static readonly DateOnly IntervalsDay = new(2026, 9, 10);

    private readonly string _activePlanPath;
    private readonly string _myPlanPath;
    private readonly TrainingPlanViewModel _viewModel;

    public CheckingOffTodaysRun()
    {
        // The active plan store starts empty; the runner's own plan lives
        // elsewhere on disk.
        _activePlanPath = Path.GetTempFileName();
        _myPlanPath = Path.GetTempFileName();
        File.WriteAllText(_myPlanPath, """
            {
              "sessions": [
                { "date": "2026-09-07", "type": "EasyRun",   "distanceKm": 6.0 },
                { "date": "2026-09-10", "type": "Intervals", "distanceKm": 9.0 }
              ]
            }
            """);

        var repository = new JsonTrainingPlanRepository(_activePlanPath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository));
        _viewModel.LoadPlan(_myPlanPath);
    }

    public void Dispose()
    {
        File.Delete(_activePlanPath);
        File.Delete(_myPlanPath);
        GC.SuppressFinalize(this);
    }

    private DayViewModel DayOn(DateOnly date) =>
        _viewModel.Weeks.SelectMany(week => week.Days)
            .Single(day => day.Date == date);

    [Fact(Skip = "pending implementation")]
    public void MarkingTodaysRunShowsItAsCompleted()
    {
        DayOn(EasyRunDay).IsCompleted.Should().BeFalse();

        _viewModel.MarkCompleted(EasyRunDay);

        DayOn(EasyRunDay).IsCompleted.Should().BeTrue();
    }

    [Fact(Skip = "pending implementation")]
    public void OtherSessionsStayUncompleted()
    {
        _viewModel.MarkCompleted(EasyRunDay);

        DayOn(IntervalsDay).IsCompleted.Should().BeFalse();
    }

    [Fact(Skip = "pending implementation")]
    public void CompletionSurvivesReloadingTheInSandboxPlan()
    {
        _viewModel.MarkCompleted(EasyRunDay);

        // A fresh view model reading the same in-sandbox active plan, as if the
        // app had been restarted without re-picking the runner's file.
        var repository = new JsonTrainingPlanRepository(_activePlanPath);
        var reopened = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository));

        reopened.Weeks.SelectMany(week => week.Days)
            .Single(day => day.Date == EasyRunDay)
            .IsCompleted.Should().BeTrue();
    }

    [Fact(Skip = "pending implementation")]
    public void CompletionIsWrittenThroughToMyPlanOnDisk()
    {
        _viewModel.MarkCompleted(EasyRunDay);

        using JsonDocument document =
            JsonDocument.Parse(File.ReadAllText(_myPlanPath));
        JsonElement easyRun = document.RootElement
            .GetProperty("sessions")
            .EnumerateArray()
            .Single(session => session.GetProperty("date").GetString()
                == "2026-09-07");

        easyRun.GetProperty("completed").GetBoolean().Should().BeTrue();
    }
}
