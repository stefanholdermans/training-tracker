# PLAN — The one where I see the shape of my programme

Weekly mileage totals are visually coded by how they compare to the peak week,
so the build-up and taper of the training load are clear at a glance.

The total is rendered as a horizontal fill bar in the calendar's Total column:
its length and tint track a normalized intensity. The scale is stretched across
the spread between the lowest *active* week and the peak week, with a visual
floor, so a true rest week (0 km) stays distinct from the easiest working week.

## Normalization

Given `Hi` = peak weekly total, `Lo` = lowest active (> 0) weekly total:

- `T == 0`  → `0.0`  (rest/down week: empty track)
- `Hi == Lo` → `1.0` (single active week, or all active weeks equal)
- otherwise → `FLOOR + (1 - FLOOR) * (T - Lo) / (Hi - Lo)`, `FLOOR = 0.15`

## Tasks

- [x] Acceptance tests for the story (skipped, "pending implementation")
- [x] `TrainingCalendar.PeakWeeklyDistanceKm`
- [x] `TrainingCalendar.LowestActiveWeeklyDistanceKm`
- [x] `WeekViewModel.IntensityFraction`
- [x] `WeekViewModel.IntensityColor`
- [x] Compute intensity in `TrainingPlanViewModel.MapWeeks`
- [x] Unskip acceptance tests
- [x] Render the load bar in the Total cell (`TrainingPlanPage.xaml`, ProgressBar)
- [ ] Mark story complete in `STORIES.md`

## Test cases

### TrainingCalendar (unit)
- Peak is the greatest weekly total across the calendar.
- Peak is `0` for an empty calendar.
- Lowest active is the smallest total among weeks with any load.
- Lowest active is `null` when every week is a rest week.

### TrainingPlanViewModel (unit)
- Lowest active week → fraction `0.15`.
- Peak week → fraction `1.0`.
- A mid-load week → fraction stretched between floor and 1.
- A rest week → fraction `0.0`.
- All active weeks equal (incl. a single active week) → fraction `1.0`.
- Peak week colour is the peak-load hex; a rest week is neutral.

### Acceptance (training-plan.json: weeks 13, 0, 16, 20 km)
- Week fractions are `0.15`, `0.0`, `~0.5142857`, `1.0`.
- The peak week is coded with the peak-load colour.
