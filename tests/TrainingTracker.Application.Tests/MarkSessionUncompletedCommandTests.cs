using FluentAssertions;
using NSubstitute;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a repository holding the active training plan.
/// </summary>
public class MarkSessionUncompletedCommandTests
{
    private readonly ITrainingPlanRepository _repository;
    private readonly MarkSessionUncompletedCommand _command;

    public MarkSessionUncompletedCommandTests()
    {
        _repository = Substitute.For<ITrainingPlanRepository>();
        _command = new MarkSessionUncompletedCommand(_repository);
    }

    [Fact]
    public void MarksTheSessionOnTheGivenDateUncompletedAndSavesThePlan()
    {
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m),
                Completed: true)
        ]);

        _command.Execute(new DateOnly(2026, 3, 2));

        _repository.Received(1).Save(Arg.Is<IReadOnlyList<ScheduledSession>>(
            saved => !saved.Single().Completed));
    }

    [Fact]
    public void LeavesSessionsOnOtherDatesUntouched()
    {
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m),
                Completed: true),
            new ScheduledSession(
                new DateOnly(2026, 3, 5),
                new TrainingSession(TrainingType.Intervals, 8.0m),
                Completed: true)
        ]);

        _command.Execute(new DateOnly(2026, 3, 2));

        _repository.Received(1).Save(Arg.Is<IReadOnlyList<ScheduledSession>>(
            saved =>
                !saved.Single(s => s.Date == new DateOnly(2026, 3, 2)).Completed
                && saved.Single(s => s.Date == new DateOnly(2026, 3, 5))
                    .Completed));
    }
}
