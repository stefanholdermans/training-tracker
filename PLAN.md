# Plan: The one where I spot a session I've missed

As a runner, I want sessions that were scheduled before today but never
completed to stand out in the calendar, so that I can see at a glance
where I've fallen behind the programme.

## Design

A missed session is a planned session whose date is before today and
which the runner has not checked off. The calendar already carries the
date it was assembled for (`Today`), so this is a display concern just
like highlighting today: the day mapping computes the flag from the day's
own state and the calendar's `Today`.

`DayViewModel` gains an `IsMissed` flag. A session due today but not yet
done is not missed (the day is not over); a completed past session is not
missed; a past rest day carries no session and so is never missed.

## Tasks

- [ ] Add the missed-session fixture and skipped acceptance tests
- [ ] `DayViewModel.IsMissed` flags past, undone sessions
- [ ] Make missed sessions stand out in the calendar page
- [ ] Unmark the acceptance tests and tick the story off
