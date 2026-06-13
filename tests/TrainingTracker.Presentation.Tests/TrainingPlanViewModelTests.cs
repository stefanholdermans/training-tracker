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
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 2), null)
            ])
        ]));

        IReadOnlyList<WeekViewModel> weeks =
            new TrainingPlanViewModel(_query).Weeks;

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

        IReadOnlyList<DayViewModel> days =
            new TrainingPlanViewModel(_query).Weeks[0].Days;

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
                new TrainingDay(
                    new DateOnly(2026, 3, 5),
                    new TrainingSession(TrainingType.Intervals, 8.0m))
            ])
        ]));

        SessionViewModel? daySession =
            new TrainingPlanViewModel(_query).Weeks[0].Days[0].Session;

        daySession.Should().NotBeNull();
        daySession?.DisplayName.Should().Be("Intervals");
        daySession?.Color.Should().Be("#7050C0");
        daySession?.DistanceKm.Should().Be(8.0m);
    }

    [Theory]
    [InlineData(TrainingType.EasyRun, "Easy Run")]
    [InlineData(TrainingType.ThresholdRun, "Threshold Run")]
    [InlineData(TrainingType.Repetitions, "Repetitions")]
    [InlineData(TrainingType.Intervals, "Intervals")]
    [InlineData(TrainingType.LongRun, "Long Run")]
    [InlineData(TrainingType.Race, "Race")]
    public void MapsTrainingTypeToDisplayName(
        TrainingType type, string expectedDisplayName)
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(type, 10.0m))
            ])
        ]));

        string? displayName = new TrainingPlanViewModel(_query)
            .Weeks[0].Days[0].Session?.DisplayName;

        displayName.Should().Be(expectedDisplayName);
    }

    [Theory]
    [InlineData(TrainingType.EasyRun, "#4CAF80")]
    [InlineData(TrainingType.ThresholdRun, "#E07820")]
    [InlineData(TrainingType.Repetitions, "#C04040")]
    [InlineData(TrainingType.Intervals, "#7050C0")]
    [InlineData(TrainingType.LongRun, "#4080C0")]
    [InlineData(TrainingType.Race, "#C09020")]
    public void MapsTrainingTypeToColor(
        TrainingType type, string expectedColor)
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(type, 10.0m))
            ])
        ]));

        string? color = new TrainingPlanViewModel(_query)
            .Weeks[0].Days[0].Session?.Color;

        color.Should().Be(expectedColor);
    }

    [Fact]
    public void LeavesRestDaysWithNoSession()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 2), null)
            ])
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[0].Days[0].Session.Should().BeNull();
    }

    [Fact]
    public void IsRestDayIsTrueWhenThereIsNoSession()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(new DateOnly(2026, 3, 2), null)
            ])
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[0].Days[0].IsRestDay.Should().BeTrue();
    }

    [Fact]
    public void IsRestDayIsFalseWhenThereIsASession()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m))
            ])
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[0].Days[0].IsRestDay.Should().BeFalse();
    }

    [Fact]
    public void ExposesTotalDistanceKmForAWeekWithSessions()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m)),
                new TrainingDay(new DateOnly(2026, 3, 3), null),
                new TrainingDay(
                    new DateOnly(2026, 3, 5),
                    new TrainingSession(TrainingType.Intervals, 8.0m))
            ])
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[0].TotalDistanceKm.Should().Be(13.0m);
    }

    [Fact]
    public void ExposesZeroTotalDistanceKmForARestWeek()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 9),
            [
                new TrainingDay(new DateOnly(2026, 3, 9),  null),
                new TrainingDay(new DateOnly(2026, 3, 10), null)
            ])
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[0].TotalDistanceKm.Should().Be(0.0m);
    }

    private void GivenAProgramme()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 13.0m),
            Week(new DateOnly(2026, 3, 9), 0.0m),
            Week(new DateOnly(2026, 3, 16), 16.0m),
            Week(new DateOnly(2026, 3, 23), 20.0m)
        ]));
    }

    private static TrainingWeek Week(DateOnly startDate, decimal distanceKm) =>
        new(startDate,
        [
            new TrainingDay(
                startDate,
                distanceKm > 0
                    ? new TrainingSession(TrainingType.EasyRun, distanceKm)
                    : null)
        ]);

    [Fact]
    public void IntensityFractionPutsTheLowestActiveWeekAtTheVisualFloor()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[0].IntensityFraction.Should().BeApproximately(0.15, 1e-9);
    }

    [Fact]
    public void IntensityFractionIsZeroForARestWeek()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[1].IntensityFraction.Should().Be(0.0);
    }

    [Fact]
    public void IntensityFractionStretchesAMidLoadWeekBetweenFloorAndOne()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[2].IntensityFraction.Should().BeApproximately(0.5142857, 1e-6);
    }

    [Fact]
    public void IntensityFractionIsOneForThePeakWeek()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[3].IntensityFraction.Should().BeApproximately(1.0, 1e-9);
    }

    [Fact]
    public void IntensityFractionIsOneWhenThereIsASingleActiveWeek()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 0.0m),
            Week(new DateOnly(2026, 3, 9), 18.0m)
        ]));

        new TrainingPlanViewModel(_query)
            .Weeks[1].IntensityFraction.Should().BeApproximately(1.0, 1e-9);
    }

    [Fact]
    public void IntensityFractionIsOneWhenAllActiveWeeksAreEqual()
    {
        _query.Execute().Returns(new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 15.0m),
            Week(new DateOnly(2026, 3, 9), 15.0m)
        ]));

        IReadOnlyList<WeekViewModel> weeks = new TrainingPlanViewModel(_query).Weeks;

        weeks[0].IntensityFraction.Should().BeApproximately(1.0, 1e-9);
        weeks[1].IntensityFraction.Should().BeApproximately(1.0, 1e-9);
    }

    [Fact]
    public void IntensityColorIsThePeakLoadColourForThePeakWeek()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[3].IntensityColor.Should().Be("#1A5FB4");
    }

    [Fact]
    public void IntensityColorIsNeutralForARestWeek()
    {
        GivenAProgramme();

        new TrainingPlanViewModel(_query)
            .Weeks[1].IntensityColor.Should().Be("#C8C8C8");
    }
}
