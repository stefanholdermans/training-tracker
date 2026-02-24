using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json.
/// </summary>
public class ViewingThisWeeksVolume
{
    private readonly TrainingPlanViewModel _viewModel;

    public ViewingThisWeeksVolume()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                new JsonTrainingPlanRepository(fixturePath)));
    }

    [Fact]
    public void EachWeekShowsItsTotalPlannedDistance()
    {
        // Week 1 (2026-03-02): EasyRun 5.0 + Intervals 8.0 = 13.0.
        _viewModel.Weeks[0].TotalDistanceKm.Should().Be(13.0m);

        // Week 2 (2026-03-09): rest week.
        _viewModel.Weeks[1].TotalDistanceKm.Should().Be(0.0m);

        // Week 3 (2026-03-16): ThresholdRun 10.0 + Repetitions 6.0 = 16.0.
        _viewModel.Weeks[2].TotalDistanceKm.Should().Be(16.0m);

        // Week 4 (2026-03-23): LongRun 20.0.
        _viewModel.Weeks[3].TotalDistanceKm.Should().Be(20.0m);
    }
}
