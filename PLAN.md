# Plan: The one where I know it's a rest day

As a runner, I want rest days to be clearly distinguished from training days, so
that I know when recovery is part of the plan.

## Approach

No changes to the domain or application layers — `TrainingDay.Session == null`
already models a rest day correctly. The work lives entirely in the presentation
and UI layers.

**UX:** Each rest day cell gets a subtle background tint (light: cool near-white
gray; dark: fractionally lighter than `OffBlack`) plus a muted "Rest" label
rendered where the session badge would appear. Training day cells are unaffected.

**Architecture:** A computed `IsRestDay` property on `DayViewModel` drives two
conditional XAML `DataTrigger`s on the day cell: one for the background tint, one
for the "Rest" label visibility.

---

## Step 1 — Acceptance tests (skipped)

**File:** `tests/TrainingTracker.AcceptanceTests/KnowingItIsARestDay.cs`

Wire up the full stack (JSON fixture → repository → query → view model) just like
`ViewingMyTrainingPlan` does. The fixture already contains days with no session.

Mark both tests with `[Fact(Skip = "pending implementation")]` so the suite stays
green at commit time:
- `ADayWithNoSessionIsARestDay` — pick a day from the fixture that has no session
  and assert `IsRestDay == true`.
- `ADayWithASessionIsNotARestDay` — pick a day that has a session and assert
  `IsRestDay == false`.

**Commit.** All tests pass (skipped tests do not fail).

---

## Step 2 — `DayViewModel.IsRestDay` (red → green → refactor)

**File:** `tests/TrainingTracker.Presentation.Tests/TrainingPlanViewModelTests.cs`

Extend the existing class with two new `[Fact]` tests:
- `IsRestDayIsTrueWhenThereIsNoSession` — construct a `TrainingCalendar` with a
  `TrainingDay` whose session is `null`; assert that the corresponding
  `DayViewModel.IsRestDay` is `true`.
- `IsRestDayIsFalseWhenThereIsASession` — construct a `TrainingCalendar` with a
  `TrainingDay` that has a session; assert that the corresponding
  `DayViewModel.IsRestDay` is `false`.

**Red** — both new tests fail to compile (property does not exist yet).

**File:** `src/TrainingTracker.Presentation/DayViewModel.cs`

Add the computed property:

```csharp
public bool IsRestDay => Session is null;
```

**Green** — both new unit tests pass. Remove `Skip` from the acceptance tests in
Step 1; they pass now too.

**Refactor** — assess whether anything warrants tidying (unlikely at this scale).

**Commit.** All tests pass.

---

## Step 3 — Rest day colour resources

**File:** `src/TrainingTracker.App/Resources/Styles/Colors.xaml`

Add one named colour entry:

```xml
<Color x:Key="RestDayBackground">...</Color>
```

Use an `AppThemeBinding` so the tint is appropriate in both modes:
- **Light mode:** a cool, desaturated near-white (e.g. a hint of blue-gray, clearly
  distinct from the plain white cell background but not distracting).
- **Dark mode:** a shade fractionally lighter than `OffBlack` — enough contrast to
  read as "different" without competing with session badge colours.

No tests for this step; it is a pure resource addition.

**Commit.** All tests pass.

---

## Step 4 — Wire the UI

**File:** `src/TrainingTracker.App/TrainingPlanPage.xaml`

Update the day cell template (the `Border` that currently holds the date label and
optional session badge). Two additions, both conditioned on `IsRestDay`:

1. **Background tint:** A `DataTrigger` on the outer cell `Border` that sets
   `BackgroundColor` to `{StaticResource RestDayBackground}` when
   `IsRestDay == true`. Leave `BackgroundColor` unset (transparent) otherwise.

2. **"Rest" label:** Inside the cell's `VerticalStackLayout`, add a `Label` with
   text "Rest", styled in a muted secondary colour (e.g. `Gray400` in light mode /
   `Gray500` in dark mode) and a weight and size consistent with the session badge
   text. Control its visibility with a `DataTrigger` on `IsRestDay`: visible when
   `true`, collapsed when `false`.

The existing session badge (the coloured `Border`) continues to be driven by the
nullability of the `Session` binding — verify that it already collapses gracefully
on a null session; if not, add a `DataTrigger` to hide it when `IsRestDay == true`.

No new tests for this step; the acceptance tests from Step 2 are the observable
specification.

**Commit.** All tests pass.
