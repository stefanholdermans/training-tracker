using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given no training plan has been loaded yet, so the active plan file does
/// not exist on disk.
/// </summary>
public class StartingWithACleanSlate
{
    private readonly TrainingPlanViewModel _viewModel;

    public StartingWithACleanSlate()
    {
        string missingPath = Path.Combine(
            Path.GetTempPath(), $"no-plan-{Guid.NewGuid():N}.json");
        var repository = new JsonTrainingPlanRepository(missingPath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository));
    }

    [Fact]
    public void TheCalendarStartsEmpty()
    {
        _viewModel.Weeks.Should().BeEmpty();
    }
}
