# Plan: The one where I see what I've actually done

As a runner, I want to see which sessions I've completed versus what was
planned, so that I can quickly assess my adherence to the programme.

## Design

Adherence is a property of the whole programme: how many of the planned
sessions has the runner actually done? A planned session is any day that
carries a session (rest days do not count); a completed session is a
planned session the runner has checked off.

The counts are derived from the calendar the query already produces, so
they belong on `TrainingCalendar` as `PlannedSessionCount` and
`CompletedSessionCount`, alongside the existing peak/lowest-week
calculations. The view model surfaces both counts and a short summary
string ("3 of 5 sessions completed") for the page header, and refreshes
them whenever a session is checked off or undone, exactly as the weeks
already refresh.

No new plumbing is needed: the figures ride the existing repository ->
query -> view-model pipeline.

## Tasks

- [ ] Add the adherence fixture and skipped acceptance tests
- [ ] `TrainingCalendar.PlannedSessionCount` counts days with a session
- [ ] `TrainingCalendar.CompletedSessionCount` counts completed sessions
- [ ] View model exposes `PlannedSessionCount` and `CompletedSessionCount`
- [ ] View model exposes `AdherenceSummary` ("3 of 5 sessions completed")
- [ ] Counts refresh after marking a session completed or uncompleted
- [ ] Show the adherence summary in the calendar page
- [ ] Unmark the acceptance tests and tick the story off
