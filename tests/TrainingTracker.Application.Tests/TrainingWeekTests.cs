using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a training week with a set of days.
/// </summary>
public class TrainingWeekTests
{
    [Fact]
    public void TotalDistanceKmIsSumOfAllSessionDistances()
    {
        var week = new TrainingWeek(new DateOnly(2026, 3, 2),
        [
            new TrainingDay(
                new DateOnly(2026, 3, 2),
                new TrainingSession(TrainingType.EasyRun, 5.0m)),
            new TrainingDay(new DateOnly(2026, 3, 3), null),
            new TrainingDay(
                new DateOnly(2026, 3, 5),
                new TrainingSession(TrainingType.Intervals, 8.0m))
        ]);

        week.TotalDistanceKm.Should().Be(13.0m);
    }

    [Fact]
    public void TotalDistanceKmIsZeroForARestWeek()
    {
        var week = new TrainingWeek(new DateOnly(2026, 3, 9),
        [
            new TrainingDay(new DateOnly(2026, 3, 9),  null),
            new TrainingDay(new DateOnly(2026, 3, 10), null),
            new TrainingDay(new DateOnly(2026, 3, 11), null)
        ]);

        week.TotalDistanceKm.Should().Be(0.0m);
    }
}
