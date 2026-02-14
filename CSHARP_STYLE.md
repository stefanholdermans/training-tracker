# C# Coding Style Guide

> **Purpose**: This guide provides coding conventions for C# development, optimised for AI-assisted development and agent workflows.

## Table of Contents

- [Naming Conventions](#naming-conventions)
- [Type Conventions](#type-conventions)
- [Code Structure](#code-structure)
- [Language Features](#language-features)
- [LINQ Guidelines](#linq-guidelines)
- [Error Handling](#error-handling)
- [Comments and Documentation](#comments-and-documentation)
- [Layout and Formatting](#layout-and-formatting)

---

## Naming Conventions

### Casing Rules

| Element | Casing | Example | Notes |
|---------|--------|---------|-------|
| Classes | PascalCase | `DataService` | |
| Interfaces | PascalCase with `I` prefix | `IWorkerQueue` | |
| Structs | PascalCase | `ValueCoordinate` | |
| Enums | PascalCase | `FileMode` (singular for non-flags) | Use plural for flags |
| Methods | PascalCase | `ProcessData()` | |
| Properties | PascalCase | `WorkerQueue` | |
| Public fields | PascalCase | `IsValid` | Use sparingly |
| Constants | PascalCase | `MaximumItems` | Both fields and locals |
| Parameters | camelCase | `workerQueue` | |
| Local variables | camelCase | `currentIndex` | |
| Private instance fields | `_` prefix + camelCase | `_workerQueue` | |
| Static fields | `s_` prefix + camelCase | `s_instance` | Not default VS behaviour |
| Thread-static fields | `t_` prefix + camelCase | `t_timeSpan` | |

### Primary Constructor Parameters

- **Classes and structs**: Use camelCase (consistent with method parameters)
  ```csharp
  public class DataService(IWorkerQueue workerQueue, ILogger logger)
  ```

- **Records**: Use PascalCase (they become public properties)
  ```csharp
  public record Person(string FirstName, string LastName);
  ```

### Generic Type Parameters

- Use descriptive names prefixed with `T`
  ```csharp
  public interface ISessionChannel<TSession> { }
  public delegate TOutput Converter<TInput, TOutput>(TInput from);
  ```

- Use single `T` only when completely self-explanatory
  ```csharp
  public class List<T> { }
  ```

- Indicate constraints in parameter name when helpful
  ```csharp
  public interface ISessionChannel<TSession> where TSession : ISession
  ```

### General Naming Rules

- ✅ Use meaningful, descriptive names
- ✅ Prefer clarity over brevity
- ✅ Avoid abbreviations except widely known ones (HTTP, XML, ID)
- ❌ Don't use single-letter names (except simple loop counters)
- ❌ Avoid consecutive underscores (`__`) - reserved for compiler
- ✅ Use reverse domain notation for namespaces
- ✅ Choose assembly names representing primary purpose

---

## Type Conventions

### Use Language Keywords Over Runtime Types

```csharp
// ✅ Preferred
string name;
int count;

// ❌ Avoid
System.String name;
System.Int32 count;
```

**Exception**: Use `nint` and `nuint` as appropriate.

### Prefer `int` Over Unsigned Types

Use `int` throughout for easier library interaction. Use unsigned types only when specifically needed for the domain.

### Var Usage

Use `var` **only** when the type is obvious from the right-hand side:

```csharp
// ✅ Type is obvious
var message = "This is clearly a string.";
var items = new List<string>();
var customer = new Customer();

// ❌ Type not obvious
var count = GetCount();
var items = ProcessData();

// ✅ Explicit typing when unclear
int count = GetCount();
IEnumerable<Customer> items = ProcessData();
```

**Exception**: Use `var` in LINQ queries where anonymous types or complex generic types are involved.

```csharp
// ✅ LINQ with var
var query = from customer in customers
            where customer.City == "London"
            select new { customer.Name, customer.Orders };
```

**Note**: Don't use `var` as a substitute for `dynamic`.

---

## Code Structure

### Namespace Declarations

Use **file-scoped namespace declarations**:

```csharp
// ✅ Preferred
namespace MySampleCode;

public class MyClass
{
}
```

### Using Directives

Place `using` directives **outside** the namespace declaration:

```csharp
// ✅ Correct
using System;
using System.Collections.Generic;

namespace MyApplication;

public class MyClass
{
}
```

**Rationale**: Prevents ambiguity with namespace resolution and makes fully qualified names explicit.

### Object Instantiation

Use concise forms when variable type matches object type:

```csharp
// ✅ Preferred forms
var instance = new ExampleClass();
ExampleClass instance2 = new();

// ✅ Also acceptable
ExampleClass instance3 = new ExampleClass();
```

### Object Initialisation

Use object initialisers for clarity:

```csharp
// ✅ Preferred
var example = new ExampleClass 
{ 
    Name = "Desktop", 
    ID = 37414,
    Location = "Redmond" 
};
```

Use `required` properties instead of constructors when appropriate:

```csharp
public class Container<T>
{
    public required string Label { get; init; }
    public required T Contents { get; init; }
}
```

### Collection Initialisation

Use **collection expressions** for all collection types:

```csharp
// ✅ Preferred
string[] vowels = ["a", "e", "i", "o", "u"];
List<int> numbers = [1, 2, 3, 4, 5];
```

---

## Language Features

### String Handling

**Concatenation for short strings**:
```csharp
string displayName = $"{lastName}, {firstName}";
```

**StringBuilder for loops or large text**:
```csharp
var builder = new StringBuilder();
for (var i = 0; i < 10000; i++)
{
    builder.Append(phrase);
}
```

**Raw string literals** (preferred over escape sequences):
```csharp
var message = """
    This is a long message.
    It can contain "quotes" and \backslashes without escaping.
    """;
```

**Expression-based string interpolation**:
```csharp
// ✅ Preferred
Console.WriteLine($"{student.Last} Score: {student.score}");

// ❌ Avoid positional
Console.WriteLine("scoreQuery:");
```

### Delegates

**Use `Func<>` and `Action<>`** instead of custom delegate types:

```csharp
// ✅ Preferred
Action<string> log = message => Console.WriteLine(message);
Func<int, int, int> add = (x, y) => x + y;

// ❌ Avoid custom delegates unless necessary
public delegate void CustomDelegate(string message);
```

**Concise delegate instantiation**:
```csharp
// ✅ Preferred
Del handler = DelMethod;

// ❌ Avoid verbose syntax
Del handler = new Del(DelMethod);
```

### Operators

Use **conditional logical operators** (`&&`, `||`) over bitwise (`&`, `|`) for comparisons:

```csharp
// ✅ Correct - short-circuits
if ((divisor != 0) && (dividend / divisor) is var result)
{
    Console.WriteLine($"Quotient: {result}");
}

// ❌ Avoid - evaluates both sides
if ((divisor != 0) & (dividend / divisor) is var result)
```

### Static Members

Call static members using the class name:

```csharp
// ✅ Correct
ClassName.StaticMember();

// ❌ Avoid using derived class name
DerivedClass.StaticMember(); // Even if inherited
```

---

## LINQ Guidelines

### Query Variable Naming

Use meaningful names that describe the data:

```csharp
// ✅ Clear intent
var londonCustomers = from customer in customers
                      where customer.City == "London"
                      select customer.Name;
```

### Implicit Typing in LINQ

Use `var` for query variables (overrides general var rules):

```csharp
var results = from customer in customers
              where customer.City == "London"
              select new { customer.Name, customer.Orders };
```

### Anonymous Type Properties

Use aliases with PascalCase for anonymous types:

```csharp
var localDistributors = 
    from customer in customers
    join distributor in distributors on customer.City equals distributor.City
    select new 
    { 
        CustomerName = customer.Name, 
        DistributorName = distributor.Name 
    };
```

### Query Clause Alignment

Align query clauses under the `from` clause:

```csharp
var results = from customer in customers
              where customer.City == "London"
              orderby customer.Name
              select customer;
```

### Where Clauses First

Place `where` clauses early to filter data before other operations:

```csharp
var results = from customer in customers
              where customer.IsActive  // Filter first
              orderby customer.Name
              select customer;
```

### Multiple From Clauses

Use multiple `from` clauses to access nested collections:

```csharp
// ✅ Preferred for nested collections
var highScores = from student in students
                 from score in student.Scores
                 where score > 90
                 select new { student.LastName, score };

// ❌ Avoid join for nested collections
```

---

## Error Handling

### Exception Handling

Use `try-catch` for exception handling:

```csharp
try
{
    return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
}
catch (ArithmeticException ex)
{
    Console.WriteLine($"Arithmetic error: {ex}");
    throw;
}
```

**Guidelines**:
- ✅ Only catch exceptions you can handle
- ✅ Use specific exception types
- ❌ Avoid catching `System.Exception` without filters
- ✅ Provide meaningful error messages

### Using Statements

**Modern syntax** (preferred):
```csharp
using Font font = new("Arial", 10.0f);
byte charset = font.GdiCharSet;
// Disposed automatically at end of scope
```

**Traditional syntax** (when needed):
```csharp
using (Font font = new("Arial", 10.0f))
{
    byte charset = font.GdiCharSet;
}
```

**Replace try-finally** with using when only calling Dispose:
```csharp
// ❌ Verbose
Font font = new("Arial", 10.0f);
try
{
    byte charset = font.GdiCharSet;
}
finally
{
    font?.Dispose();
}

// ✅ Concise
using Font font = new("Arial", 10.0f);
byte charset = font.GdiCharSet;
```

---

## Comments and Documentation

### Comment Style

Use **single-line comments** for brief explanations:

```csharp
// Calculate the distance between two points
var distance = Math.Sqrt(dx * dx + dy * dy);
```

**Avoid multi-line comments** (`/* */`) for explanations.

### Comment Guidelines

- ✅ Place comments on separate lines, not at end of code
- ✅ Begin with uppercase letter
- ✅ End with a period
- ✅ Insert one space after `//`
- ❌ Don't embed explanations in code samples (they won't be localised)
- ✅ Use longer explanatory text in companion documentation

### XML Documentation

Use XML comments for all public members:

```csharp
/// <summary>
/// Calculates the distance between two points.
/// </summary>
/// <param name="x1">X coordinate of first point</param>
/// <param name="y1">Y coordinate of first point</param>
/// <returns>The distance between the points</returns>
public static double CalculateDistance(double x1, double y1, double x2, double y2)
{
    // Implementation
}
```

---

## Layout and Formatting

### Indentation and Spacing

- **Indentation**: 4 spaces (no tabs)
- **Line length**: Limit to 80 characters for readability
- **Blank lines**: At least one between method/property definitions

### Brace Style

Use **Allman style** (braces on their own line):

```csharp
// ✅ Correct
public class Example
{
    public void Method()
    {
        if (condition)
        {
            DoSomething();
        }
    }
}
```

Braces align with current indentation level.

### Statement Guidelines

- ✅ One statement per line
- ✅ One declaration per line
- ✅ Indent continuation lines one tab stop (4 spaces)

### Parentheses for Clarity

Use parentheses to make clause precedence clear:

```csharp
// ✅ Clear precedence
if ((startX > endX) && (startX > previousX))
{
    // Take action
}
```

### Line Breaks

- Break long statements across multiple lines
- Place line breaks **before** binary operators

```csharp
// ✅ Correct
var result = longVariableName
    + anotherLongVariable
    + yetAnotherVariable;
```

### Event Handlers

Use lambda expressions for event handlers you don't need to remove:

```csharp
// ✅ Concise
public Form()
{
    this.Click += (s, e) => 
    {
        MessageBox.Show(((MouseEventArgs)e).Location.ToString());
    };
}

// ❌ Verbose (unless you need to remove handler)
public Form()
{
    this.Click += Form_Click;
}

void Form_Click(object sender, EventArgs e)
{
    MessageBox.Show(((MouseEventArgs)e).Location.ToString());
}
```

---

## Implicit Typing in Loops

### For Loops

Use `var` for loop variables:

```csharp
// ✅ Preferred
for (var i = 0; i < 10000; i++)
{
    builder.Append(phrase);
}
```

### Foreach Loops

Use **explicit typing** for foreach loop variables:

```csharp
// ✅ Correct - type is explicit
foreach (char ch in text)
{
    ProcessCharacter(ch);
}

// ❌ Avoid var in foreach
foreach (var ch in text)
{
    ProcessCharacter(ch);
}
```

**Rationale**: Collection element types aren't always obvious, and relying on collection names alone can be misleading.

---

## Modern C# Features

### Utilise Modern Language Features

- ✅ Use latest C# language version features when appropriate
- ✅ Async/await for I/O-bound operations
- ✅ Pattern matching and switch expressions
- ✅ Records for immutable data
- ✅ Init-only properties
- ✅ Required properties
- ✅ File-scoped namespaces
- ✅ Collection expressions

### Async Programming

Use `async`/`await` for I/O-bound operations:

```csharp
public async Task<Data> FetchDataAsync()
{
    var response = await httpClient.GetAsync(url);
    return await response.Content.ReadAsAsync<Data>();
}
```

**Be cautious of deadlocks**; use `ConfigureAwait(false)` when appropriate in library code.

---

## Code Quality Principles

### Clarity and Simplicity

- ✅ Write clear, simple code
- ❌ Avoid overly complex logic
- ✅ Prefer readable code over clever code
- ✅ Use LINQ for collection manipulation when it improves readability

### Security

- ✅ Follow secure coding guidelines
- ✅ Validate all inputs
- ❌ Don't expose sensitive information in exceptions
- ✅ Use parameterised queries for database access

---

## Quick Reference Checklist

### Before Committing Code

- [ ] Naming follows conventions (PascalCase, camelCase, prefixes)
- [ ] Using directives outside namespace
- [ ] File-scoped namespace declaration
- [ ] Braces use Allman style
- [ ] Lines limited to 80 characters
- [ ] One statement per line
- [ ] Explicit types in foreach loops
- [ ] var only when type is obvious
- [ ] Language keywords (`string`, `int`) not runtime types
- [ ] XML comments on public members
- [ ] Modern C# features utilised appropriately
- [ ] Collection expressions for collection initialisation
- [ ] Required properties instead of constructors where appropriate

---

## For AI Agents

When generating C# code:

1. **Prioritise readability** - code will be copied/pasted into production
2. **Use modern features** - demonstrate current best practices
3. **Be consistent** - follow these conventions throughout
4. **Validate assumptions** - when type isn't obvious, use explicit typing
5. **Comment judiciously** - use XML docs for public APIs, inline comments sparingly
6. **Handle errors properly** - catch specific exceptions, provide meaningful messages

### Common Patterns to Follow

```csharp
// File structure
using System;
using System.Collections.Generic;

namespace MyApplication.Feature;

/// <summary>
/// Public API with XML documentation
/// </summary>
public class MyService(ILogger logger)
{
    private readonly ILogger _logger = logger;

    public required string ConfigValue { get; init; }

    public async Task<Result> ProcessAsync(string input)
    {
        try
        {
            // Use var when type is obvious
            var items = ParseInput(input);
            
            // Modern collection expressions
            var results = [..items.Where(x => x.IsValid)];
            
            return new Result { Items = results };
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for input");
            throw;
        }
    }
}
```
