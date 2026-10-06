# PLAN — The one where I pick up the race pace

A pace run is like a long run, but portions of it are run at the intended race
pace (e.g. marathon pace when training for a marathon). The runner wants to
record pace runs in the stored plan and see them in the calendar as a session
type of their own, distinct from an ordinary long run.

## Design

A pace run is a kind of session, so it is a new `TrainingType` value. From
there it rides the existing pipeline — repository → calendar query → view
model — exactly as the other types already do.

- `TrainingType.PaceRun` — a new value in the domain enum.
- `JsonTrainingPlanRepository` — parses and writes the type by name, so
  `"PaceRun"` is read and written with no further change.
- `TrainingPlanViewModel` — maps `PaceRun` to the display name "Pace Run" and
  the colour `#205090`: a deeper shade of the long run's blue (`#4080C0`), so
  it reads as a harder long run whilst staying distinct from one.

How much of the run is done at race pace is out of scope for this story.

## Tasks

- [x] Acceptance tests for the story (skipped, "pending implementation")
- [x] Read `PaceRun` sessions in `JsonTrainingPlanRepository`
- [x] Map `PaceRun` to the display name "Pace Run"
- [x] Map `PaceRun` to its colour
- [ ] Document `PaceRun` in the README
- [ ] Unskip acceptance tests
- [ ] Mark story complete in `STORIES.md`

## Test cases

### JsonTrainingPlanRepository (unit)
- Reads a session of type `PaceRun` from the JSON file.

### TrainingPlanViewModel (unit)
- Maps `PaceRun` to the display name "Pace Run".
- Maps `PaceRun` to the colour `#205090`.

### Acceptance (runner's plan: 2026-03-02 LongRun 20K,
### 2026-03-08 PaceRun 24K)
- A pace run is shown as "Pace Run".
- A pace run is colour-coded apart from a long run.
- A pace run counts towards the week's volume.
