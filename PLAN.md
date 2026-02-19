# Plan: The one where I see my training plan

## Story

As a runner, I want to view my complete training programme in a calendar
format, so that I can see what's coming up and plan my week accordingly.

## Acceptance Tests

- [ ] All weeks of the programme appear
- [ ] A session appears on its scheduled date (with type and distance)
- [ ] Each week spans seven days from Monday to Sunday
- [ ] Weeks are shown in chronological order

## Tasks

- [ ] Define `TrainingType` enum in Domain layer
- [ ] Define `TrainingPlan`, `TrainingWeek`, `TrainingDay`, and
      `TrainingSession` DTOs in Application layer
- [ ] Define `IGetTrainingPlanQuery` in Application layer
- [ ] Implement `TrainingPlanViewModel`
- [ ] Implement `WeekViewModel`
- [ ] Implement `DayViewModel`
- [ ] Implement `SessionViewModel`
