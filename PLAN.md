# PLAN — Cross-restart write-back to the runner's own file

Checking off a run must behave like load + autosave on the runner's *own*
file: completions are written back to that file and are still there after the
app is quit and relaunched. The in-sandbox copy was an app-builder convenience;
the runner only cares about their file, so it becomes the single source of
truth and the private copy is dropped.

## Source of truth

The runner's chosen file is read and written directly. The only thing the app
persists for itself is *which* file that is, so it can be reopened after a
restart:

- a new `IPlanLocation` abstracts "which file is the plan, persistently".
- `FilePlanLocation` records the path in a small pointer file — used in tests
  and on any ordinary filesystem, and the structural twin of the bookmark.
- on Mac Catalyst the file lives outside the sandbox, so the location is backed
  by a **security-scoped bookmark**: created when the runner picks the file,
  resolved (and access re-acquired) on the next launch.

`JsonTrainingPlanRepository` reads `IPlanLocation.FilePath`, `Load` just
remembers the picked file, and `Save` writes that same file. No copy.

## Tasks

- [x] `IPlanLocation` (Application)
- [x] `FilePlanLocation` (Infrastructure) + unit tests
- [x] `JsonTrainingPlanRepository` reads/writes the located file; `Load`
      remembers it; drop the in-sandbox copy
- [x] Update the repository's save-after-load test
- [x] Rework the acceptance test to prove cross-restart write-back
- [ ] Mac Catalyst: security-scoped bookmark location + picker
- [ ] DI: register `IPlanLocation`; repository resolves it

## Test cases

### FilePlanLocation (unit)
- No remembered file yet → `FilePath` is `null`.
- Remembering a file makes a fresh instance (a "restart") return it.
- Remembering another file replaces the first.

### JsonTrainingPlanRepository (unit)
- Saving after loading a file writes that file.

### Acceptance (runner's plan: 2026-09-07 EasyRun, 2026-09-10 Intervals)
- Marking the 7th shows it completed; the 10th stays uncompleted.
- Completion is written to the runner's own file on disk.
- After a restart (a fresh app against the remembered location, with no
  re-pick), the 7th still reads as completed.
- Marking the 10th after a restart writes it through to the runner's own file,
  which then records both completions.
