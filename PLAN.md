# PLAN — The one where I undo an accidental check-off

A runner who checks off a session by mistake needs a way to undo it.
Uncompleting is the exact inverse of marking completed: clear the
`Completed` flag on the scheduled session for a given date and persist the
plan, so the corrected state shows in the calendar and outlives a restart.

All the plumbing already exists from "The one where I check off today's run":
`ScheduledSession.Completed`, the calendar carrying completion out to
`DayViewModel.IsCompleted`, and `ITrainingPlanRepository.Save` writing through
to the runner's own file. This story only adds the inverse use case, a
view-model entry point, and the calendar tap that toggles a day back off.

## Design

A separate, intention-revealing use case mirrors the existing one, in keeping
with the one-command-per-action shape of the Application layer:

- `IMarkSessionUncompletedCommand` + `MarkSessionUncompletedCommand` — sets
  `Completed = false` for the session on the given date and saves the plan.
- `TrainingPlanViewModel.MarkUncompleted` — drives the command and refreshes
  the calendar.
- `TrainingPlanPage` — a tap on a completed day un-checks it; a tap on an
  uncompleted day still checks it off (tap to toggle).

## Tasks

- [ ] Acceptance tests for the story (skipped, "pending implementation")
- [ ] `IMarkSessionUncompletedCommand` + `MarkSessionUncompletedCommand`
- [ ] `TrainingPlanViewModel.MarkUncompleted`
- [ ] Unskip acceptance tests
- [ ] DI wiring + tap a completed day to un-check it in the app
- [ ] Mark story complete in `STORIES.md`

## Test cases

### MarkSessionUncompletedCommand (unit)
- Marks the session on the given date uncompleted and saves the plan.
- Leaves sessions on other dates untouched.

### TrainingPlanViewModel (unit)
- `MarkUncompleted` clears the session on the given date through the command.
- `MarkUncompleted` refreshes the calendar so the day reads as uncompleted.

### Acceptance (runner's plan: 2026-09-07 EasyRun, 2026-09-10 Intervals)
- Un-checking a completed 7th shows that day as uncompleted again.
- A completion on the 10th is left untouched.
- The undo is written through to the runner's own plan file on disk.
- The undo survives a restart.
