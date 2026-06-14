using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json.
/// </summary>
public class KnowingItIsARestDay
{
    private readonly TrainingPlanViewModel _viewModel;

    public KnowingItIsARestDay()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository));
    }

    [Fact]
    public void ADayWithNoSessionIsARestDay()
    {
        DayViewModel tuesday = _viewModel.Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 3));
        tuesday.IsRestDay.Should().BeTrue();
    }

    [Fact]
    public void ADayWithASessionIsNotARestDay()
    {
        DayViewModel monday = _viewModel.Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 2));
        monday.IsRestDay.Should().BeFalse();
    }
}
