using FluentAssertions;
using TrainingTracker.Infrastructure;

namespace TrainingTracker.Infrastructure.Tests;

/// <summary>
/// Given a location backed by a pointer file on disk.
/// </summary>
public sealed class FilePlanLocationTests : IDisposable
{
    private readonly string _pointerPath;

    public FilePlanLocationTests() =>
        _pointerPath = Path.Combine(
            Path.GetTempPath(), $"pointer-{Guid.NewGuid():N}");

    public void Dispose()
    {
        File.Delete(_pointerPath);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void HasNoFilePathWhenNothingHasBeenRemembered()
    {
        new FilePlanLocation(_pointerPath).FilePath.Should().BeNull();
    }

    [Fact]
    public void RemembersTheChosenFileAcrossInstances()
    {
        new FilePlanLocation(_pointerPath).Remember("/runner/my-plan.json");

        // A fresh instance reading the same pointer, as if after a restart.
        new FilePlanLocation(_pointerPath).FilePath
            .Should().Be("/runner/my-plan.json");
    }

    [Fact]
    public void ReplacesTheRememberedFileWhenAnotherIsChosen()
    {
        var location = new FilePlanLocation(_pointerPath);
        location.Remember("/runner/first.json");

        location.Remember("/runner/second.json");

        new FilePlanLocation(_pointerPath).FilePath
            .Should().Be("/runner/second.json");
    }
}
