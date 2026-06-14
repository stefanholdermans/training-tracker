using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from strides-plan.json, in which the session
/// on 2026-03-02 carries 8 strides and the one on 2026-03-04 has none.
/// </summary>
public class TackingOnSomeStrides
{
    private readonly TrainingPlanViewModel _viewModel;

    public TackingOnSomeStrides()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "strides-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    private SessionViewModel SessionOn(DateOnly date) =>
        _viewModel.Weeks[0].Days.Single(d => d.Date == date).Session!;

    [Fact]
    public void ASessionWithStridesExposesTheCount()
    {
        SessionOn(new DateOnly(2026, 3, 2)).Strides.Should().Be(8);
    }

    [Fact]
    public void ASessionWithStridesShowsThemAlongsideTheDistance()
    {
        SessionOn(new DateOnly(2026, 3, 2)).Distance.Should().Be("6K + 8 ST");
    }

    [Fact]
    public void ASessionWithoutStridesHasNone()
    {
        SessionOn(new DateOnly(2026, 3, 4)).Strides.Should().BeNull();
    }

    [Fact]
    public void ASessionWithoutStridesShowsOnlyTheDistance()
    {
        SessionOn(new DateOnly(2026, 3, 4)).Distance.Should().Be("10K");
    }
}
