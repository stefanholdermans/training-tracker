using FluentAssertions;
using NSubstitute;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a training plan retrieved from the repository.
/// </summary>
public class GetTrainingPlanQueryTests
{
    private readonly ITrainingPlanRepository _repository;
    private readonly GetTrainingPlanQuery _query;

    public GetTrainingPlanQueryTests()
    {
        _repository = Substitute.For<ITrainingPlanRepository>();
        _query = new GetTrainingPlanQuery(_repository);
    }

    [Fact]
    public void ReturnsEmptyCalendarWhenThereAreNoSessions()
    {
        _repository.GetAll().Returns([]);

        _query.Execute().Weeks.Should().BeEmpty();
    }

    [Fact]
    public void ReturnsOneWeekContainingASingleSession()
    {
        // Session on Thursday; expect the enclosing Monday-to-Sunday week.
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 5),
                new TrainingSession(TrainingType.Intervals, 8.0m))
        ]);

        IReadOnlyList<TrainingWeek> weeks = _query.Execute().Weeks;

        weeks.Should().HaveCount(1);
        // StartDate is the Monday of the enclosing week.
        weeks[0].StartDate.Should().Be(new DateOnly(2026, 3, 2));
        weeks[0].Days.Should().HaveCount(7);
        // First day is Monday.
        weeks[0].Days[0].Date.Should().Be(new DateOnly(2026, 3, 2));
        // Last day is Sunday.
        weeks[0].Days[6].Date.Should().Be(new DateOnly(2026, 3, 8));
    }

    [Fact]
    public void SessionAppearsOnItsScheduledDay()
    {
        var session = new TrainingSession(TrainingType.Intervals, 8.0m);
        _repository.GetAll().Returns(
        [
            new ScheduledSession(new DateOnly(2026, 3, 5), session)
        ]);

        TrainingDay thursday = _query.Execute().Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 5));

        thursday.Session.Should().Be(session);
    }

    [Fact]
    public void UnscheduledDaysAreRestDays()
    {
        // Only Thursday has a session; the other six days should be rest days.
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 5),
                new TrainingSession(TrainingType.Intervals, 8.0m))
        ]);

        IReadOnlyList<TrainingDay> days = _query.Execute().Weeks[0].Days;

        days.Where(d => d.Date != new DateOnly(2026, 3, 5))
            .Should().AllSatisfy(d => d.Session.Should().BeNull());
    }

    [Fact]
    public void SpansAllWeeksBetweenFirstAndLastSession()
    {
        // Sessions two weeks apart; the empty middle week must still appear.
        _repository.GetAll().Returns(
        [
            new ScheduledSession(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m)),
            new ScheduledSession(
                new DateOnly(2026, 3, 16),
                new TrainingSession(TrainingType.LongRun, 20.0m))
        ]);

        IReadOnlyList<TrainingWeek> weeks = _query.Execute().Weeks;

        weeks.Should().HaveCount(3);
        weeks[0].StartDate.Should().Be(new DateOnly(2026, 3, 2));
        // Empty middle week.
        weeks[1].StartDate.Should().Be(new DateOnly(2026, 3, 9));
        weeks[2].StartDate.Should().Be(new DateOnly(2026, 3, 16));
    }
}
