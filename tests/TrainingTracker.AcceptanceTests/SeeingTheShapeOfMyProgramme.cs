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

    [Fact(Skip = "pending implementation")]
    public void WeeklyTotalsAreCodedRelativeToThePeakWeek()
    {
        // Peak week (20K) anchors the top of the scale; the lowest active
        // week (13K) sits at the visual floor.
        IReadOnlyList<WeekViewModel> weeks = _viewModel.Weeks;

        // Week 1 (2026-03-02): EasyRun 5.0 + Intervals 8.0 = 13.0 (lowest active).
        weeks[0].IntensityFraction.Should().BeApproximately(0.15, 1e-9);

        // Week 2 (2026-03-09): rest week.
        weeks[1].IntensityFraction.Should().Be(0.0);

        // Week 3 (2026-03-16): ThresholdRun 10.0 + Repetitions 6.0 = 16.0.
        weeks[2].IntensityFraction.Should().BeApproximately(0.5142857, 1e-6);

        // Week 4 (2026-03-23): LongRun 20.0 (peak).
        weeks[3].IntensityFraction.Should().BeApproximately(1.0, 1e-9);
    }

    [Fact(Skip = "pending implementation")]
    public void ThePeakWeekIsCodedWithThePeakLoadColour()
    {
        _viewModel.Weeks[3].IntensityColor.Should().Be("#1A5FB4");
    }
}
