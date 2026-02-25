using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json.
/// </summary>
public class SeeingTheShapeOfMyProgramme
{
    private readonly TrainingPlanViewModel _viewModel;

    public SeeingTheShapeOfMyProgramme()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                new JsonTrainingPlanRepository(fixturePath)));
    }

    [Fact(Skip = "Not yet implemented")]
    public void PeakWeekHasRelativeIntensityOfOne()
    {
        // Week 4 (2026-03-23): LongRun 20.0 — the peak week.
        _viewModel.Weeks[3].RelativeIntensity.Should().Be(1.0m);
    }

    [Fact(Skip = "Not yet implemented")]
    public void OtherWeeksAreScaledRelativeToPeakWeek()
    {
        // Week 1 (2026-03-02): 5.0 + 8.0 = 13.0 → 13 ÷ 20 = 0.65.
        _viewModel.Weeks[0].RelativeIntensity.Should().Be(0.65m);

        // Week 3 (2026-03-16): 10.0 + 6.0 = 16.0 → 16 ÷ 20 = 0.80.
        _viewModel.Weeks[2].RelativeIntensity.Should().Be(0.80m);
    }

    [Fact(Skip = "Not yet implemented")]
    public void RestWeekHasZeroRelativeIntensity()
    {
        // Week 2 (2026-03-09): rest week — no sessions.
        _viewModel.Weeks[1].RelativeIntensity.Should().Be(0.0m);
    }
}
