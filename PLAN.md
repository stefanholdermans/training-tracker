# Story: The one where I see this week's volume

As a runner, I want to see the total planned distance for each week of my
training plan, so that I can understand the training load across the programme.

---

## Acceptance Tests

**File**: `tests/TrainingTracker.AcceptanceTests/ViewingThisWeeksVolume.cs`

Use the existing fixture (`training-plan.json`). The fixture weeks and their
volumes:

| Week starting | Sessions                            | Total |
|---------------|-------------------------------------|-------|
| 2026-03-02    | EasyRun 5.0 + Intervals 8.0         | 13.0  |
| 2026-03-09    | (rest week)                         |  0.0  |
| 2026-03-16    | ThresholdRun 10.0 + Repetitions 6.0 | 16.0  |
| 2026-03-23    | LongRun 20.0                        | 20.0  |

### Scenario 1 – Each week shows its total planned distance

```
Given a training plan with sessions spread across multiple weeks
When I view the training plan
Then each week displays the sum of its scheduled session distances
```

Concrete assertions:

- Week of 2026-03-02 has TotalDistanceKm == 13.0
- Week of 2026-03-09 has TotalDistanceKm == 0.0
- Week of 2026-03-16 has TotalDistanceKm == 16.0
- Week of 2026-03-23 has TotalDistanceKm == 20.0

Mark the test `[Fact(Skip = "not implemented yet")]` until the story is done.

---

## TDD Steps

### Step 1 – `TotalDistanceKm` on `TrainingWeek` (Application)

- [ ] **Test** (`tests/TrainingTracker.Application.Tests/TrainingWeekTests.cs`):
  - `TotalDistanceKmIsSumOfAllSessionDistances` — a week with two sessions
    returns the correct total
  - `TotalDistanceKmIsZeroForARestWeek` — a week of all rest days returns 0.0

- [ ] **Implement** (`src/TrainingTracker.Application/TrainingWeek.cs`):
  - Add computed property:
    `decimal TotalDistanceKm => Days.Sum(d => d.Session?.DistanceKm ?? 0);`

### Step 2 – `TotalDistanceKm` on `WeekViewModel` (Presentation)

- [ ] **Test** (`tests/TrainingTracker.Presentation.Tests/TrainingPlanViewModelTests.cs`):
  - `ExposesTotalDistanceKmForAWeekWithSessions` — `WeekViewModel.TotalDistanceKm`
    equals the sum of the week's session distances
  - `ExposesZeroTotalDistanceKmForARestWeek` — `WeekViewModel.TotalDistanceKm`
    is 0.0 when the week has no sessions

- [ ] **Implement** (`src/TrainingTracker.Presentation/`):
  - Add `TotalDistanceKm` (decimal) to `WeekViewModel`
  - Map it from `TrainingWeek.TotalDistanceKm` inside `TrainingPlanViewModel`

### Step 3 – UI wiring (App)

- [ ] **Implement** (`src/TrainingTracker.App/TrainingPlanPage.xaml`):
  - Display `TotalDistanceKm` on each week row (e.g., "52K")

### Step 4 – Finish acceptance tests

- [ ] Remove `Skip` from the `ViewingThisWeeksVolume` test and confirm it passes
- [ ] Mark the story `[x]` in `STORIES.md`
