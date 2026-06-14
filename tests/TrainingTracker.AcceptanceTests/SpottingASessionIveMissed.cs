using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from missed-plan.json, opened in the app on
/// 2026-03-10. The easy run on the 2nd was completed, the intervals on the
/// 5th were not, today carries a threshold run, and a long run is still to
/// come on the 23rd.
/// </summary>
public class SpottingASessionIveMissed
{
    private readonly TrainingPlanViewModel _viewModel;

    public SpottingASessionIveMissed()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "missed-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                repository, new FixedClock(new DateOnly(2026, 3, 10))),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    private DayViewModel DayOn(DateOnly date) =>
        _viewModel.Weeks.SelectMany(week => week.Days)
            .Single(day => day.Date == date);

    [Fact]
    public void APastSessionLeftUndoneIsMissed()
    {
        DayOn(new DateOnly(2026, 3, 5)).IsMissed.Should().BeTrue();
    }

    [Fact]
    public void ACompletedPastSessionIsNotMissed()
    {
        DayOn(new DateOnly(2026, 3, 2)).IsMissed.Should().BeFalse();
    }

    [Fact]
    public void TodaysUndoneSessionIsNotMissed()
    {
        DayOn(new DateOnly(2026, 3, 10)).IsMissed.Should().BeFalse();
    }

    [Fact]
    public void AFutureSessionIsNotMissed()
    {
        DayOn(new DateOnly(2026, 3, 23)).IsMissed.Should().BeFalse();
    }

    [Fact]
    public void APastRestDayIsNotMissed()
    {
        DayOn(new DateOnly(2026, 3, 4)).IsMissed.Should().BeFalse();
    }
}
