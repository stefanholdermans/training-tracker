using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given the runner's own plan on disk, titled "2026 Rotterdam Marathon",
/// opened in the app. The plan lives in a fresh temp file per test so that
/// checking a session off does not bleed between scenarios.
/// </summary>
public sealed class KnowingWhichRaceImTrainingFor : IDisposable
{
    private readonly string _planPath;
    private readonly TrainingPlanViewModel _viewModel;

    public KnowingWhichRaceImTrainingFor()
    {
        _planPath = Path.GetTempFileName();
        File.WriteAllText(_planPath, """
            {
              "title": "2026 Rotterdam Marathon",
              "sessions": [
                { "date": "2026-03-02", "type": "EasyRun", "distanceKm": 5.0 }
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

    [Fact(Skip = "pending implementation")]
    public void ThePlanTitleIsShown()
    {
        _viewModel.Title.Should().Be("2026 Rotterdam Marathon");
    }

    [Fact(Skip = "pending implementation")]
    public void TheTitleSurvivesCheckingOffASession()
    {
        _viewModel.MarkCompleted(new DateOnly(2026, 3, 2));

        _viewModel.Title.Should().Be("2026 Rotterdam Marathon");
    }
}
