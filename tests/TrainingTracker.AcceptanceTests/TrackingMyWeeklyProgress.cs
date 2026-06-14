using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from weekly-progress-plan.json, in which the
/// first week has 13K planned with 5K completed, the second week is a rest
/// week, and the third week has 16K planned and all of it completed.
/// </summary>
public class TrackingMyWeeklyProgress
{
    private readonly TrainingPlanViewModel _viewModel;

    public TrackingMyWeeklyProgress()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "weekly-progress-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    [Fact(Skip = "pending implementation")]
    public void EachWeekShowsItsCompletedDistance()
    {
        _viewModel.Weeks[0].CompletedDistanceKm.Should().Be(5.0m);
        _viewModel.Weeks[1].CompletedDistanceKm.Should().Be(0.0m);
        _viewModel.Weeks[2].CompletedDistanceKm.Should().Be(16.0m);
    }

    [Fact(Skip = "pending implementation")]
    public void ProgressComparesCompletedToPlanned()
    {
        _viewModel.Weeks[0].ProgressSummary.Should().Be("5K of 13K");
        _viewModel.Weeks[2].ProgressSummary.Should().Be("16K of 16K");
    }

    [Fact(Skip = "pending implementation")]
    public void ARestWeekHasNoProgressToReport()
    {
        _viewModel.Weeks[1].ProgressSummary.Should().BeEmpty();
    }
}
