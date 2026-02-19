# Agent Guidelines

This document provides guidance for AI coding agents working on this project. It outlines our development philosophy, processes, and standards.

## Table of Contents

1. [Development Philosophy](#development-philosophy)
2. [Architectural Principles](#architectural-principles)
3. [Test-Driven Development Process](#test-driven-development-process)
4. [Testing Strategy](#testing-strategy)
5. [Workflow and Practices](#workflow-and-practices)
6. [Git Commit Messages](#git-commit-messages)
7. [Code Style](#code-style)

---

## Development Philosophy

### Kent Beck's Four Rules of Simple Design

We adhere to Kent Beck's Four Rules of Simple Design, in priority order:

1. **Passes all tests** – The code must be correct and verifiable.
2. **Reveals intention** – The code clearly expresses what it does. Names, structure, and organisation should make the purpose obvious.
3. **No duplication** – Knowledge and logic should appear once and only once in the system (DRY principle).
4. **Fewest elements** – Minimise the number of classes, methods, and other elements whilst satisfying the above rules.

When refactoring, use these rules as your guide. They help us achieve emergent design through disciplined practice.

### Domain-Driven Design (DDD)

We practice Domain-Driven Design to keep our code aligned with domain concepts:

- **Ubiquitous Language**: Use the same terminology in code, tests, and conversations that domain experts use.
- **Bounded Contexts**: Identify and respect context boundaries where terms may have different meanings.
- **Entities and Value Objects**: Model domain concepts appropriately—entities have identity, value objects are defined by their attributes.
- **Aggregates**: Group related entities and value objects, with one entity serving as the aggregate root.
- **Domain Events**: Capture significant occurrences in the domain.
- **Repository Pattern**: Abstract data access behind domain-focused interfaces.

---

## Architectural Principles

### Clean Architecture

We apply Clean Architecture principles to maintain separation of concerns and ensure testability:

#### Layers (from innermost to outermost)

1. **Domain Layer** (`TrainingTracker.Domain`)
   - Core business logic and domain models
   - No dependencies on outer layers
   - Framework-agnostic
   - Contains entities, value objects, domain events, and domain services

2. **Application Layer** (`TrainingTracker.Application`)
   - Application-specific business rules
   - Orchestrates the flow of data to and from entities
   - Depends only on the Domain Layer
   - Contains use cases, application services, and DTOs

3. **Presentation Layer** (`TrainingTracker.Presentation`)
   - Adapts Application-layer output for the UI
   - Contains ViewModels, commands, and view model base classes
   - Depends on the Application Layer
   - Targets plain `net10.0` so ViewModels can be unit tested without a device or simulator

4. **Infrastructure Layer** (`TrainingTracker.App`)
   - MAUI application shell: XAML views, DI wiring, platform entry points
   - Implements interfaces defined in inner layers
   - Depends on the Presentation Layer (which transitively includes Application and Domain)
   - Should contain as little logic as possible — all testable logic belongs in the Presentation Layer

#### Key Principles

- **Dependency Rule**: Dependencies point inward. Inner layers know nothing about outer layers.
- **Stable Abstractions**: Define interfaces in inner layers, implement them in outer layers.
- **Framework Independence**: Business logic should not depend on frameworks.
- **Testability**: Business rules can be tested without UI, database, or external services.

---

## Test-Driven Development Process

We strictly apply the **Red-Green-Refactor** cycle:

### The Cycle

1. **Red** – Write a failing test
   - Write the smallest test that fails
   - The test should fail for the right reason (e.g., method doesn't exist yet, or returns wrong value)
   - Run the test and verify it fails

2. **Green** – Make the test pass
   - Write the simplest code that makes the test pass
   - Don't worry about elegance or perfection yet
   - It's acceptable to write "obvious implementation" or even hard-code values if appropriate
   - Run the test and verify it passes

3. **Refactor** – Improve the code
   - Remove duplication
   - Improve names
   - Apply the Four Rules of Simple Design
   - Ensure all tests still pass
   - **Commit after this step**

### Critical Rules

- **Never write production code without a failing test first**
- **Write only enough test code to make a test fail**
- **Write only enough production code to make a failing test pass**
- **All unit tests must pass before committing**

---

## Testing Strategy

### Unit Tests

- **Purpose**: Test individual components in isolation
- **Scope**: Single class or small group of collaborating classes
- **Speed**: Fast (milliseconds)
- **Test Doubles**: Use NSubstitute for mocks, stubs, and fakes
- **Coverage**: Written before implementation code (TDD)
- **Commit Policy**: **Must pass before committing**

#### Test Tiers

| Project | Scope | What it verifies |
|---|---|---|
| `TrainingTracker.Domain.Tests` | Entities, value objects, domain services | Pure domain logic; no fakes needed |
| `TrainingTracker.Application.Tests` | Use cases, application services | Orchestration with NSubstitute fakes for repository interfaces |
| `TrainingTracker.Presentation.Tests` | ViewModels, commands | ViewModel state and behaviour with faked Application-layer interfaces |

### Acceptance Tests

Acceptance tests verify that user stories are implemented correctly. They
exercise the full stack end-to-end — from the Infrastructure layer (file
reading) through the Application layer (use cases) to the Presentation layer
(ViewModels) — with no test doubles. We follow Behaviour-Driven Development
(BDD) principles.

#### Writing Acceptance Tests

1. **Before starting a user story**, write acceptance tests covering all scenarios
2. **Mark new tests** with "expect to fail" or similar mechanism
3. **When scenarios are implemented**, remove the "expect to fail" marker
4. **Once unmarked**, the test becomes a regression test and must pass before committing

#### Format

Structure acceptance tests to be readable by non-technical stakeholders:

```
Given [initial context]
When [event occurs]
Then [expected outcome]
```

Map these to real objects, not test doubles:

- **Given** = a JSON fixture file (checked into the test project) containing
  the training plan data for the scenario
- **When** = wire up the real `JsonTrainingPlanRepository` → real use case →
  real ViewModel, then observe the ViewModel
- **Then** = assert ViewModel properties

Test doubles (NSubstitute) are used only in **unit tests**, never in
acceptance tests.

#### Commit Policy

- **May commit with failing acceptance tests** if they are marked "expect to fail"
- **Must not commit** if an unmarked acceptance test fails (this indicates a regression)

---

## Workflow and Practices

### Trunk-Based Development

- All work happens on `main` (or `trunk`)
- No long-lived feature branches
- Commit frequently (multiple times per day)
- Keep commits small and atomic
- Each commit should leave the code in a working state (all unit tests pass)

### User Stories and Planning

#### STORIES.md

- Maintains the backlog of user stories
- Stories are written from the user's perspective
- Format: "As a [role], I want [feature], so that [benefit]"
- Check off stories as they are completed

#### PLAN.md

- Active whilst working on a story
- Contains a task list and test cases for the current story
- Inspired by Kent Beck's approach in *Test-Driven Development: By Example*
- Add tasks as you discover them
- Check off tasks as they are completed
- Can be discarded or archived once the story is complete

### Workflow Summary

When starting a new story:

1. Select a story from STORIES.md
2. Create/update PLAN.md with initial tasks and test cases
3. Write acceptance tests for the story (marked "expect to fail")
4. Commit the acceptance tests
5. For each task/test case in PLAN.md:
   - Follow Red-Green-Refactor cycle
   - Commit after refactoring
   - Check off the task in PLAN.md
6. When acceptance tests pass, remove "expect to fail" markers
7. Mark the story as complete in STORIES.md

---

## Git Commit Messages

We follow best practices for writing clear, useful commit messages. A well-written commit message communicates the **why** and **what** of a change to your future self and collaborators.

### Format

```
Short summary (50 characters or less)

More detailed explanatory text, if necessary. Wrap it to about 72
characters. The blank line separating the summary from the body is
critical; tools like git log, git shortlog and git rebase can get
confused if you run the two together.

Explain the problem that this commit is solving. Focus on why you
are making this change as opposed to how (the code explains that).
Are there side effects or other unintuitive consequences of this
change? Here's the place to explain them.

Further paragraphs come after blank lines.

- Bullet points are okay, too
- Use a hyphen or asterisk for the bullet, followed by a single
  space
```

### The Seven Rules

1. **Separate subject from body with a blank line**
   - Not every commit requires both. Sometimes a single line is fine, especially for simple changes.
   - When a commit deserves explanation and context, write a body.

2. **Limit the subject line to 50 characters**
   - This is a guideline, not a hard limit. Aim for 50, but 72 is the hard limit.
   - Keeping it short forces you to think about the most concise way to explain what's going on.

3. **Capitalise the subject line**
   - Begin the subject line with a capital letter.

4. **Do not end the subject line with a full stop**
   - Trailing punctuation is unnecessary in subject lines.

5. **Use the imperative mood in the subject line**
   - Write as if giving a command or instruction.
   - Examples: "Add validation for email addresses", "Remove deprecated methods", "Fix race condition in order processing"
   - A properly formed Git commit subject line should always be able to complete the sentence: "If applied, this commit will _[your subject line here]_"

6. **Wrap the body at 72 characters**
   - Git never wraps text automatically. Wrap it yourself at 72 characters.
   - This ensures messages remain readable in various contexts (terminal, email, web interfaces).

7. **Use the body to explain what and why vs. how**
   - The diff shows the how. The commit message explains the why.
   - Focus on the reasons you made the change in the first place—the way things worked before the change (and what was wrong with that), the way they work now, and why you decided to solve it the way you did.

### Examples

**Good:**

```
Add caching to product repository

The product lookup was hitting the database on every request,
causing performance issues during peak traffic. This commit
introduces a simple in-memory cache with a 5-minute TTL.

Considered Redis but opted for in-memory as a first step.
Will monitor and move to distributed cache if needed.
```

**Good (simple change):**

```
Fix typo in user validation error message
```

**Bad:**

```
Fixed stuff
```

**Bad:**

```
Updated code to make it work better and fixed some bugs that were causing problems
```

---

## Code Style

C# coding style guidelines are provided in **CSHARP_STYLE.md**.

Please refer to that document for:
- Naming conventions
- Formatting rules
- Language feature usage
- Organisation and structure
- Documentation standards

---

## Summary

As an agent working on this project:

1. Always write tests first (TDD)
2. Follow Red-Green-Refactor religiously
3. Commit after refactoring when unit tests pass
4. Keep the code simple (Four Rules)
5. Respect Clean Architecture boundaries
6. Use domain language (DDD)
7. Update PLAN.md and STORIES.md as you work
8. Ensure all unit tests pass before committing
9. Mark acceptance tests appropriately

When in doubt, prioritise simplicity, clarity, and testability.
