using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json, opened in the app on
/// 2026-03-04 — a rest day that falls between the easy run on the 2nd and the
/// intervals on the 5th.
/// </summary>
public class KnowingWhereIAmToday
{
    private readonly TrainingPlanViewModel _viewModel;

    public KnowingWhereIAmToday()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                repository, new FixedClock(new DateOnly(2026, 3, 4))),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    private DayViewModel DayOn(DateOnly date) =>
        _viewModel.Weeks.SelectMany(week => week.Days)
            .Single(day => day.Date == date);

    [Fact(Skip = "pending implementation")]
    public void TodaysCellIsMarkedAsToday()
    {
        DayOn(new DateOnly(2026, 3, 4)).IsToday.Should().BeTrue();
    }

    [Fact(Skip = "pending implementation")]
    public void AnotherDayWithASessionIsNotToday()
    {
        DayOn(new DateOnly(2026, 3, 5)).IsToday.Should().BeFalse();
    }

    [Fact(Skip = "pending implementation")]
    public void AnotherRestDayIsNotToday()
    {
        DayOn(new DateOnly(2026, 3, 3)).IsToday.Should().BeFalse();
    }
}
