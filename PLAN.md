# PLAN — The one where I tack on some strides

Some sessions finish with strides: a handful of short, ~100 m accelerations
tacked onto the end of a run. The runner wants to record how many strides a
session carries in the stored plan and see them in the calendar next to the
distance, e.g. "6K + 8 ST". Strides are optional — most sessions have none.

## Design

Strides are an attribute of the planned session, so they belong on the
`TrainingSession` value object as an optional count. From there they ride the
existing pipeline — repository → calendar query → view model — without any new
plumbing, exactly as `DistanceKm` already does.

- `TrainingSession.Strides` — an optional `int?`, defaulting to `null` (no
  strides). A value object attribute, defined by the session's data.
- `JsonTrainingPlanRepository` — reads an optional `strides` field and writes
  it only when present, keeping plans without strides unchanged.
- `SessionViewModel.Strides` — carries the count out for display.
- `SessionViewModel.Distance` — the display text shown in the calendar: the
  distance in whole kilometres, with "+ N ST" appended when there are strides.
- `TrainingPlanPage` — each day binds its distance label to `Distance` so the
  strides appear next to the kilometres.

## Tasks

- [ ] Acceptance tests for the story (skipped, "pending implementation")
- [ ] `TrainingSession.Strides`
- [ ] Read and write `strides` in `JsonTrainingPlanRepository`
- [ ] Map `Strides` onto `SessionViewModel`
- [ ] `SessionViewModel.Distance` display text
- [ ] Bind the calendar's distance labels to `Distance`
- [ ] Unskip acceptance tests
- [ ] Mark story complete in `STORIES.md`

## Test cases

### JsonTrainingPlanRepository (unit)
- Reads the `strides` count from the JSON file.
- A session without a `strides` field has no strides.
- `Save` round-trips a session's strides.
- `Save` omits `strides` for a session without them.

### SessionViewModel (unit)
- `Distance` is the whole-kilometre distance with a "K" suffix when there are
  no strides, e.g. "6K".
- `Distance` appends "+ N ST" when there are strides, e.g. "6K + 8 ST".

### TrainingPlanViewModel (unit)
- Maps a session's strides onto the day's `SessionViewModel`.
- Leaves `Strides` null for a session without strides.

### Acceptance (runner's plan: 2026-03-02 EasyRun 6K + 8 strides,
### 2026-03-04 ThresholdRun 10K, no strides)
- A session with strides exposes the count.
- Its distance reads "6K + 8 ST".
- A session without strides has no strides and reads "10K".
