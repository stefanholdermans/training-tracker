using FluentAssertions;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a training calendar made up of weeks.
/// </summary>
public class TrainingCalendarTests
{
    private static TrainingWeek Week(DateOnly startDate, params decimal[] distancesKm) =>
        new(startDate,
        [
            ..distancesKm.Select((km, i) => new TrainingDay(
                startDate.AddDays(i),
                km > 0 ? new TrainingSession(TrainingType.EasyRun, km) : null))
        ]);

    [Fact]
    public void PeakWeeklyDistanceKmIsTheGreatestWeeklyTotal()
    {
        var calendar = new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 13.0m),
            Week(new DateOnly(2026, 3, 9), 0.0m),
            Week(new DateOnly(2026, 3, 16), 16.0m),
            Week(new DateOnly(2026, 3, 23), 20.0m)
        ]);

        calendar.PeakWeeklyDistanceKm.Should().Be(20.0m);
    }

    [Fact]
    public void PeakWeeklyDistanceKmIsZeroForAnEmptyCalendar()
    {
        new TrainingCalendar([]).PeakWeeklyDistanceKm.Should().Be(0.0m);
    }

    [Fact]
    public void LowestActiveWeeklyDistanceKmIsTheSmallestTotalWithLoad()
    {
        var calendar = new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 13.0m),
            Week(new DateOnly(2026, 3, 9), 0.0m),
            Week(new DateOnly(2026, 3, 16), 16.0m),
            Week(new DateOnly(2026, 3, 23), 20.0m)
        ]);

        calendar.LowestActiveWeeklyDistanceKm.Should().Be(13.0m);
    }

    [Fact]
    public void LowestActiveWeeklyDistanceKmIsNullWhenEveryWeekIsRest()
    {
        var calendar = new TrainingCalendar(
        [
            Week(new DateOnly(2026, 3, 2), 0.0m),
            Week(new DateOnly(2026, 3, 9), 0.0m)
        ]);

        calendar.LowestActiveWeeklyDistanceKm.Should().BeNull();
    }

    [Fact]
    public void PlannedSessionCountCountsDaysThatCarryASession()
    {
        var calendar = new TrainingCalendar(
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
        ]);

        calendar.PlannedSessionCount.Should().Be(2);
    }

    [Fact]
    public void PlannedSessionCountIsZeroForAnEmptyCalendar()
    {
        new TrainingCalendar([]).PlannedSessionCount.Should().Be(0);
    }

    [Fact]
    public void CompletedSessionCountCountsOnlyCompletedSessions()
    {
        var calendar = new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m),
                    Completed: true),
                new TrainingDay(
                    new DateOnly(2026, 3, 5),
                    new TrainingSession(TrainingType.Intervals, 8.0m))
            ])
        ]);

        calendar.CompletedSessionCount.Should().Be(1);
    }

    [Fact]
    public void CompletedSessionCountIsZeroForAnEmptyCalendar()
    {
        new TrainingCalendar([]).CompletedSessionCount.Should().Be(0);
    }
}
