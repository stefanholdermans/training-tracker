using FluentAssertions;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.AcceptanceTests;

/// <summary>
/// Given a training plan loaded from training-plan.json.
/// </summary>
public class ViewingMyTrainingPlan
{
    private readonly TrainingPlanViewModel _viewModel;

    public ViewingMyTrainingPlan()
    {
        string fixturePath = Path.Combine(
            AppContext.BaseDirectory, "training-plan.json");
        _viewModel = new TrainingPlanViewModel(
            new GetTrainingPlanQuery(
                new JsonTrainingPlanRepository(fixturePath)));
    }

    [Fact]
    public void AllWeeksOfTheProgrammeAppear()
    {
        _viewModel.Weeks.Should().HaveCount(4);
    }

    [Fact]
    public void ASessionAppearsOnItsScheduledDate()
    {
        DayViewModel thursday = _viewModel.Weeks[0].Days
            .Single(d => d.Date == new DateOnly(2026, 3, 5));
        thursday.Session.Should().NotBeNull();
        thursday.Session.DisplayName.Should().Be("Intervals");
        thursday.Session.Color.Should().Be("#7050C0");
        thursday.Session.DistanceKm.Should().Be(8m);
    }

    [Fact]
    public void EachWeekSpansSevenDaysFromMondayToSunday()
    {
        WeekViewModel firstWeek = _viewModel.Weeks[0];
        firstWeek.Days.Should().HaveCount(7);
        // First day is Monday.
        firstWeek.Days[0].Date.Should().Be(new DateOnly(2026, 3, 2));
        // Last day is Sunday.
        firstWeek.Days[6].Date.Should().Be(new DateOnly(2026, 3, 8));
    }

    [Fact]
    public void WeeksAreShownInChronologicalOrder()
    {
        _viewModel.Weeks[0].StartDate.Should().Be(new DateOnly(2026, 3, 2));
        _viewModel.Weeks[1].StartDate.Should().Be(new DateOnly(2026, 3, 9));
    }
}
