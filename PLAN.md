# Plan: The one where I know which race I'm training for

As a runner, I want my training plan to have a title (such as "2026
Rotterdam Marathon"), so that I can see at a glance which event the
programme is preparing me for.

## Design

The title is plan-level metadata authored in the runner's JSON file
alongside the sessions. The app never sets it, but it must not lose it:
checking a session off rewrites the file, so the JSON repository
preserves the title it already holds when saving.

Reading stays simple: the repository exposes the title through a
`GetTitle()` companion to `GetAll()`, the query stamps the calendar with
it, and the view model surfaces it for the page header. A plan with no
title gives an empty header.

## Tasks

- [x] Read and preserve the plan title in the JSON repository
- [x] Add the title acceptance tests (skipped)
- [x] Stamp the calendar with the plan title from the repository
- [ ] Surface the title on the view model (empty when there is none)
- [ ] Show the plan title in the calendar page
- [ ] Unmark the acceptance tests and tick the story off
