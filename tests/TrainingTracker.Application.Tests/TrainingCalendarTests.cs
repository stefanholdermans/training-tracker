using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Domain;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a training calendar with a set of weeks.
/// </summary>
public class TrainingCalendarTests
{
    [Fact]
    public void PeakWeekDistanceKmIsMaxOfAllWeekTotals()
    {
        var calendar = new TrainingCalendar(
        [
            new TrainingWeek(new DateOnly(2026, 3, 2),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 2),
                    new TrainingSession(TrainingType.EasyRun, 5.0m)),
                new TrainingDay(
                    new DateOnly(2026, 3, 5),
                    new TrainingSession(TrainingType.Intervals, 8.0m))
            ]),
            new TrainingWeek(new DateOnly(2026, 3, 23),
            [
                new TrainingDay(
                    new DateOnly(2026, 3, 23),
                    new TrainingSession(TrainingType.LongRun, 20.0m))
            ])
        ]);

        calendar.PeakWeekDistanceKm.Should().Be(20.0m);
    }

    [Fact]
    public void PeakWeekDistanceKmIsZeroWhenThereAreNoWeeks()
    {
        var calendar = new TrainingCalendar([]);

        calendar.PeakWeekDistanceKm.Should().Be(0.0m);
    }
}
