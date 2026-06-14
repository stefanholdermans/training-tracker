using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given the app already showing a plan (a copy of the bundled
/// training-plan.json), with the runner's own plan (my-training-plan.json)
/// sitting elsewhere on disk.
/// </summary>
public sealed class LoadingMyOwnTrainingPlan : IDisposable
{
    private readonly string _currentPlanPath;
    private readonly string _myPlanPath;
    private readonly TrainingPlanViewModel _viewModel;

    public LoadingMyOwnTrainingPlan()
    {
        // A plan is already loaded; use a copy so the fixture is left untouched.
        _currentPlanPath = Path.GetTempFileName();
        File.Copy(
            Path.Combine(AppContext.BaseDirectory, "training-plan.json"),
            _currentPlanPath,
            overwrite: true);

        // The runner's own plan lives somewhere else on disk.
        _myPlanPath = Path.Combine(
            AppContext.BaseDirectory, "my-training-plan.json");

        var repository = new JsonTrainingPlanRepository(_currentPlanPath);
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    public void Dispose()
    {
        File.Delete(_currentPlanPath);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void TheBundledPlanIsShownBeforeLoadingMyOwn()
    {
        _viewModel.Weeks.Should().HaveCount(4);
    }

    [Fact]
    public void LoadingMyOwnPlanReplacesTheBundledOne()
    {
        _viewModel.LoadPlan(_myPlanPath);

        _viewModel.Weeks.Should().HaveCount(2);
    }

    [Fact]
    public void MyOwnSessionsAppearOnTheirScheduledDates()
    {
        _viewModel.LoadPlan(_myPlanPath);

        DayViewModel thursday = _viewModel.Weeks[1].Days
            .Single(d => d.Date == new DateOnly(2026, 9, 17));
        thursday.Session.Should().NotBeNull();
        thursday.Session?.DisplayName.Should().Be("Long Run");
        thursday.Session?.DistanceKm.Should().Be(22m);
    }
}
