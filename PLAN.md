# Plan: The one where I track my weekly progress

As a runner, I want to see my completed volume compared to the planned
volume for the week, so that I can gauge whether I'm on track.

## Design

Each week already reports its planned volume as `TotalDistanceKm`. The
completed volume is the same sum restricted to the sessions the runner
has checked off, so it belongs alongside it on `TrainingWeek` as
`CompletedDistanceKm`.

The week view model surfaces the completed distance and a short
"5K of 13K" progress summary for the week's total column. A rest week
(no planned volume) has no progress to report, so its summary is empty.

The figures ride the existing repository -> query -> view-model
pipeline; no new plumbing is needed.

## Tasks

- [x] Add the weekly-progress fixture and skipped acceptance tests
- [x] `TrainingWeek.CompletedDistanceKm` sums the completed sessions
- [x] Map the completed weekly distance onto the week view model
- [x] Summarise weekly progress as "completed of planned"
- [x] Show the weekly progress in the calendar's total column
- [x] Unmark the acceptance tests and tick the story off
