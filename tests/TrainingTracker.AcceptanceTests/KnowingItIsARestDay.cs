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
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                new JsonTrainingPlanRepository(fixturePath)));
    }

    [Fact(Skip = "pending implementation")]
    public void ADayWithNoSessionIsARestDay()
    {
        DayViewModel tuesday = _viewModel.Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 3));
        tuesday.IsRestDay.Should().BeTrue();
    }

    [Fact(Skip = "pending implementation")]
    public void ADayWithASessionIsNotARestDay()
    {
        DayViewModel monday = _viewModel.Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 2));
        monday.IsRestDay.Should().BeFalse();
    }
}
