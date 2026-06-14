using System.Text.Json;
using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given the runner's own plan on disk, with an easy run on the 7th and
/// intervals on the 10th both already checked off, opened in the app.
/// </summary>
public sealed class UndoingAnAccidentalCheckOff : IDisposable
{
    private static readonly DateOnly EasyRunDay = new(2026, 9, 7);
    private static readonly DateOnly IntervalsDay = new(2026, 9, 10);

    // The pointer stands in for the security-scoped bookmark: it remembers
    // which file is the plan across "restarts".
    private readonly string _pointerPath;
    private readonly string _myPlanPath;
    private readonly TrainingPlanViewModel _viewModel;

    public UndoingAnAccidentalCheckOff()
    {
        _pointerPath = Path.Combine(
            Path.GetTempPath(), $"pointer-{Guid.NewGuid():N}");
        _myPlanPath = Path.GetTempFileName();
        File.WriteAllText(_myPlanPath, """
            {
              "sessions": [
                { "date": "2026-09-07", "type": "EasyRun",   "distanceKm": 6.0, "completed": true },
                { "date": "2026-09-10", "type": "Intervals", "distanceKm": 9.0, "completed": true }
              ]
            }
            """);

        _viewModel = OpenApp();
        _viewModel.LoadPlan(_myPlanPath);
    }

    // Wires the app afresh against the same remembered location, as if it had
    // been quit and relaunched.
    private TrainingPlanViewModel OpenApp()
    {
        var repository =
            new JsonTrainingPlanRepository(new FilePlanLocation(_pointerPath));
        return new TrainingPlanViewModel(
            new GetTrainingPlanQuery(repository),
            new LoadTrainingPlanCommand(repository),
            new MarkSessionCompletedCommand(repository),
            new MarkSessionUncompletedCommand(repository));
    }

    public void Dispose()
    {
        File.Delete(_pointerPath);
        File.Delete(_myPlanPath);
        GC.SuppressFinalize(this);
    }

    private static DayViewModel DayOn(
        TrainingPlanViewModel viewModel, DateOnly date) =>
        viewModel.Weeks.SelectMany(week => week.Days)
            .Single(day => day.Date == date);

    private bool CompletedInMyPlanFile(string date)
    {
        using JsonDocument document =
            JsonDocument.Parse(File.ReadAllText(_myPlanPath));
        JsonElement session = document.RootElement
            .GetProperty("sessions")
            .EnumerateArray()
            .Single(s => s.GetProperty("date").GetString() == date);

        return session.TryGetProperty("completed", out JsonElement flag)
            && flag.GetBoolean();
    }

    [Fact]
    public void UnmarkingARunShowsItAsUncompleted()
    {
        DayOn(_viewModel, EasyRunDay).IsCompleted.Should().BeTrue();

        _viewModel.MarkUncompleted(EasyRunDay);

        DayOn(_viewModel, EasyRunDay).IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void OtherSessionsStayCompleted()
    {
        _viewModel.MarkUncompleted(EasyRunDay);

        DayOn(_viewModel, IntervalsDay).IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void UncompletionIsWrittenToTheRunnersOwnFileOnDisk()
    {
        _viewModel.MarkUncompleted(EasyRunDay);

        CompletedInMyPlanFile("2026-09-07").Should().BeFalse();
    }

    [Fact]
    public void UncompletionSurvivesARestart()
    {
        _viewModel.MarkUncompleted(EasyRunDay);

        // Relaunch with no re-pick: the remembered location alone must lead
        // back to the runner's file and its corrected state.
        TrainingPlanViewModel relaunched = OpenApp();

        DayOn(relaunched, EasyRunDay).IsCompleted.Should().BeFalse();
    }
}
