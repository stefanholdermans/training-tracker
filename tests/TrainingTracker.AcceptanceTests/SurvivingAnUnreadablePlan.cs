using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from unreadable-plan.json, which contains a
/// session type the app does not recognise ("SpeedWork").
/// </summary>
public class SurvivingAnUnreadablePlan
{
    private readonly TrainingPlanViewModel _viewModel;

    public SurvivingAnUnreadablePlan()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "unreadable-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    [Fact]
    public void TheAppOpensDespiteTheUnreadablePlan()
    {
        _viewModel.Should().NotBeNull();
    }

    [Fact]
    public void TheCalendarIsEmptyWhenThePlanCannotBeRead()
    {
        _viewModel.Weeks.Should().BeEmpty();
    }

    [Fact]
    public void TheViewModelReportsWhyThePlanCouldNotBeRead()
    {
        _viewModel.PlanReadError.Should().NotBeNullOrEmpty();
    }
}
