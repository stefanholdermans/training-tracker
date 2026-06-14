using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from adherence-plan.json, in which five
/// sessions are planned and three of them (2026-03-02, 2026-03-16, and
/// 2026-03-23) have been completed.
/// </summary>
public class SeeingWhatIveActuallyDone
{
    private readonly TrainingPlanViewModel _viewModel;

    public SeeingWhatIveActuallyDone()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "adherence-plan.json");
        var repository = new JsonTrainingPlanRepository(fixturePath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    [Fact(Skip = "pending implementation")]
    public void ThePlanReportsHowManySessionsArePlanned()
    {
        _viewModel.PlannedSessionCount.Should().Be(5);
    }

    [Fact(Skip = "pending implementation")]
    public void ThePlanReportsHowManySessionsAreCompleted()
    {
        _viewModel.CompletedSessionCount.Should().Be(3);
    }

    [Fact(Skip = "pending implementation")]
    public void ThePlanSummarisesAdherence()
    {
        _viewModel.AdherenceSummary.Should().Be("3 of 5 sessions completed");
    }

    [Fact(Skip = "pending implementation")]
    public void CompletingASessionRaisesTheCompletedCount()
    {
        _viewModel.MarkCompleted(new DateOnly(2026, 3, 5));

        _viewModel.CompletedSessionCount.Should().Be(4);
        _viewModel.AdherenceSummary.Should().Be("4 of 5 sessions completed");
    }

    [Fact(Skip = "pending implementation")]
    public void UndoingASessionLowersTheCompletedCount()
    {
        _viewModel.MarkUncompleted(new DateOnly(2026, 3, 2));

        _viewModel.CompletedSessionCount.Should().Be(2);
        _viewModel.AdherenceSummary.Should().Be("2 of 5 sessions completed");
    }
}
