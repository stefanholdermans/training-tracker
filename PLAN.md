# Plan: The one where I take in the whole arc at once

As a runner, I want a compact overview of every week's planned volume
across the entire programme, so that I can take in the overall build-up
and taper in a single glance without scrolling through the calendar week
by week.

## Design

The overview is a sparkline-style strip: one slim bar per week, its
height proportional to that week's planned volume against the peak week.
Unlike the calendar's IntensityFraction — which compresses active weeks
into a [0.15, 1] band so each reads as effort — the overview wants the
true shape, so its height is a plain linear proportion: a rest week is
empty, the peak week is full, everything else sits in between.

The fraction belongs on the week view model as `OverviewHeightFraction`,
computed from the week's total and the calendar's peak. The page renders
the bars in a compact row above the calendar.

## Tasks

- [ ] Add the overview acceptance tests (skipped)
- [ ] `WeekViewModel.OverviewHeightFraction` is volume against the peak
- [ ] Render the compact overview strip on the calendar page
- [ ] Unmark the acceptance tests and tick the story off
