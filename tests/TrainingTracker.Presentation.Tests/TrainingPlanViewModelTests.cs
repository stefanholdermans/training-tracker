using FluentAssertions;
using NSubstitute;
using TrainingTracker.Application;
using TrainingTracker.Domain;
using TrainingTracker.Presentation;

namespace TrainingTracker.Presentation.Tests;

/// <summary>
/// Given a training calendar returned by the query.
/// </summary>
public class TrainingPlanViewModelTests
{
    private readonly IGetTrainingPlanQuery _query;

    public TrainingPlanViewModelTests()
    {
        _query = Substitute.For<IGetTrainingPlanQuery>();
    }

    [Fact]
    public void ReturnsNoWeeksWhenCalendarIsEmpty()
    {
        _query.Execute().Returns(new TrainingCalendar([]));

        new TrainingPlanViewModel(_query).Weeks.Should().BeEmpty();
    }

    [Fact]
    public void MapsEachWeekFromTheCalendar()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2), [new TrainingDay(new DateOnly(2026, 3, 2), null)])
        ]));

        var weeks = new TrainingPlanViewModel(_query).Weeks;

        weeks.Should().HaveCount(1);
        weeks[0].StartDate.Should().Be(new DateOnly(2026, 3, 2));
    }

    [Fact]
    public void MapsEachDayInTheWeek()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 2), null),
                new TrainingDay(new DateOnly(2026, 3, 3), null)
            ])
        ]));

        var days = new TrainingPlanViewModel(_query).Weeks[0].Days;

        days.Should().HaveCount(2);
        days[0].Date.Should().Be(new DateOnly(2026, 3, 2));
        days[1].Date.Should().Be(new DateOnly(2026, 3, 3));
    }

    [Fact]
    public void MapsSessionOntoItsDay()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 5), new TrainingSession(TrainingType.Intervals, 8.0m))
            ])
        ]));

        var daySession = new TrainingPlanViewModel(_query).Weeks[0].Days[0].Session;

        daySession.Should().NotBeNull();
        daySession?.DisplayName.Should().Be("Intervals");
        daySession?.DistanceKm.Should().Be(8.0m);
    }

    [Theory]
    [InlineData(TrainingType.EasyRun, "Easy Run")]
    [InlineData(TrainingType.ThresholdRun, "Threshold Run")]
    [InlineData(TrainingType.Repetitions, "Repetitions")]
    [InlineData(TrainingType.Intervals, "Intervals")]
    [InlineData(TrainingType.LongRun, "Long Run")]
    [InlineData(TrainingType.Race, "Race")]
    public void MapsTrainingTypeToDisplayName(TrainingType type, string expectedDisplayName)
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 2), new TrainingSession(type, 10.0m))
            ])
        ]));

        var displayName = new TrainingPlanViewModel(_query).Weeks[0].Days[0].Session?.DisplayName;

        displayName.Should().Be(expectedDisplayName);
    }

    [Fact]
    public void LeavesRestDaysWithNoSession()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2), [new TrainingDay(new DateOnly(2026, 3, 2), null)])
        ]));

        new TrainingPlanViewModel(_query).Weeks[0].Days[0].Session.Should().BeNull();
    }
}
