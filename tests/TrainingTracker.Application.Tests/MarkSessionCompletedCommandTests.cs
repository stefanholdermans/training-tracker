using FluentAssertions;
using NSubstitute;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a repository holding the active training plan.
/// </summary>
public class MarkSessionCompletedCommandTests
{
    private readonly ITrainingPlanRepository _repository;
    private readonly MarkSessionCompletedCommand _command;

    public MarkSessionCompletedCommandTests()
    {
        _repository = Substitute.For<ITrainingPlanRepository>();
        _command = new MarkSessionCompletedCommand(_repository);
    }

    [Fact]
    public void MarksTheSessionOnTheGivenDateCompletedAndSavesThePlan()
    {
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m))
        ]);

        _command.Execute(new DateOnly(2026, 3, 2));

        _repository.Received(1).Save(Arg.Is<IReadOnlyList<ScheduledSession>>(
            saved => saved.Single().Completed));
    }

    [Fact]
    public void LeavesSessionsOnOtherDatesUntouched()
    {
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m)),
            new ScheduledSession(
                new DateOnly(2026, 3, 5),
                new TrainingSession(TrainingType.Intervals, 8.0m))
        ]);

        _command.Execute(new DateOnly(2026, 3, 2));

        _repository.Received(1).Save(Arg.Is<IReadOnlyList<ScheduledSession>>(
            saved =>
                saved.Single(s => s.Date == new DateOnly(2026, 3, 2)).Completed
                && !saved.Single(s => s.Date == new DateOnly(2026, 3, 5))
                    .Completed));
    }
}
