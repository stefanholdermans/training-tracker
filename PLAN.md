# PLAN — The one where I check off today's run

A runner marks a training session as completed, so they can track what they
have actually accomplished. Completion is a fact about a scheduled session: it
travels from the domain out to the calendar UI, and a tap on a day in the
calendar checks the run off.

Progress must outlive a single run of the app. It is persisted in two places:

- the **in-sandbox active plan** (the working copy in the app container), so
  the calendar still shows the tick the next time the app starts; and
- the **runner's own plan file on disk** — the "actual" plan they loaded from —
  so the record of what they have done lives with their own programme.

Writing back to the runner's file (outside the sandbox) needs the
`com.apple.security.files.user-selected.read-write` entitlement and a
security-scoped resource held open from the moment they pick the file.

## Persistence model

Two files, as today:

- *active* — the working copy at `AppDataDirectory/training-plan.json`; read on
  every query, exists once a plan has been loaded.
- *source* — the file the runner picked. Its path is remembered when the plan
  is loaded.

`Save` serialises the whole plan and writes it to the active file and, when a
source path is known, through to the source file as well.

## JSON shape

Each session gains an optional `completed` flag (absent ⇒ `false`):

```json
{ "date": "2026-09-07", "type": "EasyRun", "distanceKm": 6.0, "completed": true }
```

## Tasks

- [ ] Acceptance tests for the story (skipped, "pending implementation")
- [ ] `ScheduledSession.Completed`
- [ ] `TrainingDay.Completed` / `DayViewModel.IsCompleted`
- [x] `JsonTrainingPlanRepository` reads `completed`
- [x] `ITrainingPlanRepository.Save` + `JsonTrainingPlanRepository.Save`
      (active file, then through to the source file)
- [x] `GetTrainingPlanQuery` carries completion into the calendar
- [ ] `IMarkSessionCompletedCommand` + `MarkSessionCompletedCommand`
- [ ] `TrainingPlanViewModel.MarkCompleted`
- [ ] Unskip acceptance tests
- [ ] MacCatalyst: read-write entitlement + security-scoped picker
- [ ] Tap a day to check it off + completed tick in `TrainingPlanPage.xaml`
- [ ] Mark story complete in `STORIES.md`

## Test cases

### JsonTrainingPlanRepository (unit)
- Reads `completed: true` onto the scheduled session.
- A session with no `completed` flag is not completed.
- `Save` round-trips the plan, preserving the completed flag.
- After loading a source plan, `Save` writes through to the source file.

### GetTrainingPlanQuery (unit)
- A completed session's day is marked completed.
- An uncompleted session's day is not completed.

### MarkSessionCompletedCommand (unit)
- Marks the session on the given date completed and saves the plan.
- Leaves sessions on other dates untouched.

### TrainingPlanViewModel (unit)
- `MarkCompleted` marks the session on the given date through the command.
- `MarkCompleted` refreshes the calendar so the day reads as completed.
- `DayViewModel.IsCompleted` reflects the calendar day.

### Acceptance (runner's plan: 2026-09-07 EasyRun, 2026-09-10 Intervals)
- Marking the 7th shows that day as completed.
- The 10th stays uncompleted.
- Completion survives reloading from the in-sandbox active plan.
- Completion is written through to the runner's own plan file on disk.
