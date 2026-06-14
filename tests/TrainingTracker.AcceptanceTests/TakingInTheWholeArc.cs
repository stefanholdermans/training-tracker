using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json, whose four weeks
/// build from 13K through a rest week and 16K to a 20K peak.
/// </summary>
public class TakingInTheWholeArc
{
    private readonly TrainingPlanViewModel _viewModel;

    public TakingInTheWholeArc()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    [Fact(Skip = "pending implementation")]
    public void TheOverviewHasABarForEveryWeek()
    {
        _viewModel.Weeks.Should().HaveCount(4);
    }

    [Fact(Skip = "pending implementation")]
    public void EachBarsHeightTracksItsWeeksVolumeAgainstThePeak()
    {
        IReadOnlyList<WeekViewModel> weeks = _viewModel.Weeks;

        // 13K, rest, 16K, 20K (peak) — plain proportions of the peak.
        weeks[0].OverviewHeightFraction.Should().BeApproximately(0.65, 1e-9);
        weeks[1].OverviewHeightFraction.Should().Be(0.0);
        weeks[2].OverviewHeightFraction.Should().BeApproximately(0.80, 1e-9);
        weeks[3].OverviewHeightFraction.Should().BeApproximately(1.00, 1e-9);
    }
}
