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
- [ ] Create a full marathon training plan JSON file and bundle it as a MAUI asset
      in the App project (separate from the minimal acceptance-test fixture)
- [ ] Wire up services in `MauiProgram.cs` (`JsonTrainingPlanRepository` with the
      bundled asset path, `GetTrainingPlanQuery`, `TrainingPlanViewModel`)
- [ ] Create `TrainingPlanPage` (XAML + code-behind) bound to `TrainingPlanViewModel`,
      showing weeks and days with session type and distance
- [ ] Update `AppShell.xaml` to route to `TrainingPlanPage` on launch
