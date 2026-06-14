using FluentAssertions;
using TrainingTracker.Presentation;

namespace TrainingTracker.Presentation.Tests;

/// <summary>
/// Given a session shown in the calendar.
/// </summary>
public class SessionViewModelTests
{
    [Fact]
    public void DistanceShowsWholeKilometresWhenThereAreNoStrides()
    {
        var session = new SessionViewModel
        {
            DisplayName = "Easy Run",
            Color = "#4CAF80",
            DistanceKm = 6.0m
        };

        session.Distance.Should().Be("6K");
    }

    [Fact]
    public void DistanceAppendsTheStridesWhenThereAreSome()
    {
        var session = new SessionViewModel
        {
            DisplayName = "Easy Run",
            Color = "#4CAF80",
            DistanceKm = 6.0m,
            Strides = 8
        };

        session.Distance.Should().Be("6K + 8 ST");
    }
}
