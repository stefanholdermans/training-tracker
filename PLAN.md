# Plan: The one where I see the shape of my programme

> As a runner, I want the weekly mileage totals to be visually coded by how they compare to the
> peak week, so that I can grasp at a glance how the training load builds up and tapers across
> the programme.

**UX approach:** A horizontal fill bar inside the total-column cell, scaled proportionally to the
peak week, with a cool-to-warm colour gradient (blue at low intensity, amber at peak). Both the
bar and the tint apply simultaneously. Rest weeks show an empty bar.

---

## Step 1 — Acceptance tests (skipped)

Add `tests/TrainingTracker.AcceptanceTests/SeeingTheShapeOfMyProgramme.cs`.

Using the existing fixture (`training-plan.json`), the four weeks have totals of 13 km, 0 km,
16 km, and 20 km respectively. The peak is Week 4 at 20 km.

Write three skipped tests against `TrainingPlanViewModel`:

- **`PeakWeekHasRelativeIntensityOfOne`** — `Weeks[3].RelativeIntensity` equals `1.0m`.
- **`OtherWeeksAreScaledRelativeToPeakWeek`** — `Weeks[0].RelativeIntensity` equals `0.65m`
  (13 ÷ 20) and `Weeks[2].RelativeIntensity` equals `0.8m` (16 ÷ 20).
- **`RestWeekHasZeroRelativeIntensity`** — `Weeks[1].RelativeIntensity` equals `0.0m`.

All three tests are marked `[Fact(Skip = "Not yet implemented")]`. All non-skipped tests pass
after this step.

---

## Step 2 — Application layer: `TrainingCalendar.PeakWeekDistanceKm`

### Red

Add `tests/TrainingTracker.Application.Tests/TrainingCalendarTests.cs` with two tests:

- **`PeakWeekDistanceKmIsMaxOfAllWeekTotals`** — calendar with two weeks (totals 13 km and 20 km)
  returns `20.0m`.
- **`PeakWeekDistanceKmIsZeroWhenThereAreNoWeeks`** — empty calendar returns `0.0m`.

Tests fail because `TrainingCalendar` has no `PeakWeekDistanceKm` property yet.

### Green

Add a computed property to `TrainingCalendar` in
`src/TrainingTracker.Application/TrainingCalendar.cs`:

```csharp
public decimal PeakWeekDistanceKm =>
    Weeks.Count == 0 ? 0m : Weeks.Max(w => w.TotalDistanceKm);
```

Both new tests pass. All other tests continue to pass.

### Refactor

None anticipated.

---

## Step 3 — Presentation layer: `WeekViewModel.RelativeIntensity`

### Red

Add three new test cases to
`tests/TrainingTracker.Presentation.Tests/TrainingPlanViewModelTests.cs`:

- **`PeakWeekHasRelativeIntensityOfOne`** — a calendar with a single week (any non-zero total)
  maps that week to `RelativeIntensity == 1.0m`.
- **`AWeekIsScaledRelativeToPeakWeek`** — a calendar with two weeks (totals 10 km and 20 km)
  maps them to `RelativeIntensity` values of `0.5m` and `1.0m` respectively.
- **`RestWeekHasZeroRelativeIntensity`** — a week where all days are rest days (total 0 km) in a
  calendar whose peak is also 0 km maps to `RelativeIntensity == 0.0m`. This guards the
  divide-by-zero edge case.

Tests fail because `WeekViewModel` has no `RelativeIntensity` property yet.

### Green

1. Add `RelativeIntensity` to `WeekViewModel`:

   ```csharp
   public required decimal RelativeIntensity { get; init; }
   ```

2. Update `TrainingPlanViewModel` so that `MapWeeks` passes the peak distance to `MapWeek`, and
   `MapWeek` computes the ratio:

   ```csharp
   private static IReadOnlyList<WeekViewModel> MapWeeks(TrainingCalendar plan) =>
       [..plan.Weeks.Select(week => MapWeek(week, plan.PeakWeekDistanceKm))];

   private static WeekViewModel MapWeek(TrainingWeek week, decimal peakDistanceKm) =>
       new()
       {
           StartDate         = week.StartDate,
           Days              = [..week.Days.Select(MapDay)],
           TotalDistanceKm   = week.TotalDistanceKm,
           RelativeIntensity = peakDistanceKm == 0m
                                   ? 0m
                                   : week.TotalDistanceKm / peakDistanceKm
       };
   ```

All three new unit tests pass. All other tests continue to pass.

### Unskip acceptance tests

Remove `Skip` from the three tests in `SeeingTheShapeOfMyProgramme.cs`. All three pass. All
tests in the suite pass.

### Refactor

None anticipated.

---

## Step 4 — UI wiring

### Colour resources

Add two light/dark colour pairs to
`src/TrainingTracker.App/Resources/Styles/Colors.xaml`:

```xml
<Color x:Key="LowIntensityWeekLight">#4080C0</Color>   <!-- cool blue  -->
<Color x:Key="LowIntensityWeekDark">#2060A0</Color>
<Color x:Key="PeakIntensityWeekLight">#E07820</Color>  <!-- warm amber -->
<Color x:Key="PeakIntensityWeekDark">#C06010</Color>
```

### Value converter

Add `src/TrainingTracker.App/RelativeIntensityToColorConverter.cs`.

The converter takes a `decimal` value (0.0–1.0) and linearly interpolates the RGB components
between the cool and warm colour pair that corresponds to the current app theme
(`Application.Current?.RequestedTheme`). Returns `Colors.Transparent` for null input (consistent
with `HexToColorConverter`).

Register it as a static resource in `TrainingPlanPage.xaml` (alongside the existing
`HexToColorConverter`).

### XAML — total cell

Replace the current total cell in `TrainingPlanPage.xaml`:

```xml
<!-- Weekly total (before) -->
<Border Grid.Column="7" Padding="6" Stroke="LightGray" StrokeThickness="0.5">
    <Label Text="{Binding TotalDistanceKm, StringFormat='{0:0}K'}"
           FontAttributes="Bold"
           HorizontalOptions="Center"
           VerticalOptions="Center" />
</Border>
```

With a layered structure:

```xml
<!-- Weekly total (after) -->
<Border Grid.Column="7" Padding="0" Stroke="LightGray" StrokeThickness="0.5">
    <Grid>
        <!-- Intensity bar fill: full-size BoxView scaled from the left edge -->
        <BoxView HorizontalOptions="Fill"
                 VerticalOptions="Fill"
                 AnchorX="0"
                 ScaleX="{Binding RelativeIntensity}"
                 Color="{Binding RelativeIntensity,
                         Converter={StaticResource RelativeIntensityToColorConverter}}" />
        <!-- Distance label floats above the bar -->
        <Label Text="{Binding TotalDistanceKm, StringFormat='{0:0}K'}"
               FontAttributes="Bold"
               HorizontalOptions="Center"
               VerticalOptions="Center"
               Margin="6" />
    </Grid>
</Border>
```

`ScaleX` driven by `RelativeIntensity` (0.0–1.0) scales the `BoxView` from the left anchor,
producing the proportional bar. The label sits above it in the same `Grid` cell. Rest weeks
(`RelativeIntensity == 0.0`) render with `ScaleX="0"`, showing an empty cell.

All tests continue to pass after this step.

---

## Checklist

- [ ] Step 1 — Acceptance tests added and skipped
- [ ] Step 2 — `TrainingCalendar.PeakWeekDistanceKm` (red → green)
- [ ] Step 3 — `WeekViewModel.RelativeIntensity` (red → green → unskip acceptance tests)
- [ ] Step 4 — Colour resources, converter, and XAML wiring
- [ ] Mark story as complete in `STORIES.md`
