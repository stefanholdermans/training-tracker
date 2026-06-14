using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given the runner's own plan on disk, in which five sessions are planned
/// and three of them (2026-03-02, 2026-03-16, and 2026-03-23) are already
/// checked off, opened in the app. The plan lives in a fresh temp file per
/// test so that checking sessions off does not bleed between scenarios.
/// </summary>
public sealed class SeeingWhatIveActuallyDone : IDisposable
{
    private readonly string _planPath;
    private readonly TrainingPlanViewModel _viewModel;

    public SeeingWhatIveActuallyDone()
    {
        _planPath = Path.GetTempFileName();
        File.WriteAllText(_planPath, """
            {
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun",      "distanceKm": 5.0,  "completed": true },
                { "date": "2026-03-05", "type": "Intervals",    "distanceKm": 8.0 },
                { "date": "2026-03-16", "type": "ThresholdRun", "distanceKm": 10.0, "completed": true },
                { "date": "2026-03-19", "type": "Repetitions",  "distanceKm": 6.0 },
                { "date": "2026-03-23", "type": "LongRun",      "distanceKm": 20.0, "completed": true }
              ]
            }
            """);

        var repository = new JsonTrainingPlanRepository(_planPath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    public void Dispose()
    {
        File.Delete(_planPath);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void ThePlanReportsHowManySessionsArePlanned()
    {
        _viewModel.PlannedSessionCount.Should().Be(5);
    }

    [Fact]
    public void ThePlanReportsHowManySessionsAreCompleted()
    {
        _viewModel.CompletedSessionCount.Should().Be(3);
    }

    [Fact]
    public void ThePlanSummarisesAdherence()
    {
        _viewModel.AdherenceSummary.Should().Be("3 of 5 sessions completed");
    }

    [Fact]
    public void CompletingASessionRaisesTheCompletedCount()
    {
        _viewModel.MarkCompleted(new DateOnly(2026, 3, 5));

        _viewModel.CompletedSessionCount.Should().Be(4);
        _viewModel.AdherenceSummary.Should().Be("4 of 5 sessions completed");
    }

    [Fact]
    public void UndoingASessionLowersTheCompletedCount()
    {
        _viewModel.MarkUncompleted(new DateOnly(2026, 3, 2));

        _viewModel.CompletedSessionCount.Should().Be(2);
        _viewModel.AdherenceSummary.Should().Be("2 of 5 sessions completed");
    }
}
