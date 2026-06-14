# Plan: The one where I know where I am today

As a runner, I want today's date to be highlighted in the calendar view,
so that I can immediately see where I am in my training plan.

## Design

The calendar already knows the date it was assembled for (its `Today`,
stamped from the clock). Highlighting today is therefore a display
concern: the week view model marks the day whose date matches the
calendar's `Today`, regardless of whether that day carries a session or
is a rest day.

`DayViewModel` gains an `IsToday` flag, set while mapping the days, that
the calendar page binds to so the cell stands out.

## Tasks

- [ ] Add the today fixture/clock and skipped acceptance tests
- [ ] `DayViewModel.IsToday` is set for the day matching the calendar's Today
- [ ] Highlight today's cell in the calendar page
- [ ] Unmark the acceptance tests and tick the story off
