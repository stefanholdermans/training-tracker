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

- [x] Define `TrainingType` in Domain layer
- [x] Define `TrainingSession` and `ScheduledSession` value objects in Domain layer
- [x] Define `TrainingCalendar`, `TrainingWeek`, and `TrainingDay` DTOs in
      Application layer
- [x] Define `ITrainingPlanRepository` and `IGetTrainingPlanQuery` in
      Application layer
- [ ] Implement `JsonTrainingPlanRepository` (read and parse training-plan.json)
- [ ] Implement `GetTrainingPlanQuery` (group sessions into calendar weeks)
- [ ] Implement `TrainingPlanViewModel` (map `TrainingCalendar` to ViewModels)
