using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from pace-run-plan.json, in which the week of
/// 2026-03-02 opens with a 20K long run and closes with a 24K pace run on
/// 2026-03-08.
/// </summary>
public class PickingUpTheRacePace
{
    private readonly TrainingPlanViewModel _viewModel;

    public PickingUpTheRacePace()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "pace-run-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    private SessionViewModel SessionOn(DateOnly date) =>
        _viewModel.Weeks[0].Days.Single(d => d.Date == date).Session!;

    [Fact(Skip = "pending implementation")]
    public void APaceRunIsShownAsAPaceRun()
    {
        SessionOn(new DateOnly(2026, 3, 8)).DisplayName.Should().Be("Pace Run");
    }

    [Fact(Skip = "pending implementation")]
    public void APaceRunIsColourCodedApartFromALongRun()
    {
        SessionOn(new DateOnly(2026, 3, 8)).Color.Should()
            .NotBe(SessionOn(new DateOnly(2026, 3, 2)).Color);
    }

    [Fact(Skip = "pending implementation")]
    public void APaceRunCountsTowardsTheWeeksVolume()
    {
        _viewModel.Weeks[0].TotalDistanceKm.Should().Be(44.0m);
    }
}
