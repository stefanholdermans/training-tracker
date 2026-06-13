using FluentAssertions;
using NSubstitute;
using TrainingTracker.Application;

namespace TrainingTracker.Application.Tests;

/// <summary>
/// Given a repository that holds the active training plan.
/// </summary>
public class LoadTrainingPlanCommandTests
{
    private readonly ITrainingPlanRepository _repository;
    private readonly LoadTrainingPlanCommand _command;

    public LoadTrainingPlanCommandTests()
    {
        _repository = Substitute.For<ITrainingPlanRepository>();
        _command = new LoadTrainingPlanCommand(_repository);
    }

    [Fact]
    public void AdoptsThePlanAtTheGivenPath()
    {
        _command.Execute("/runner/my-plan.json");

        _repository.Received(1).Load("/runner/my-plan.json");
    }
}
