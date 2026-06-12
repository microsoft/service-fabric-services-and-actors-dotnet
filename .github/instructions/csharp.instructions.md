---
description: "Guidelines for C# code."
applyTo: "**/*.cs"
---

- **Read the `coding.instructions.md` first**.
  Instructions in this file are incomplete without them.

# C# Guidelines

- Follow [.NET Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/) unless overridden by the rules below.
- Pay attention to the `file_header_template` setting in `.editorconfig`; existing files may have legacy headers that differ from the template.
- Don't suppress compiler, analyzer or style warnings in code - fix them instead.
  `.instructions.md` files must be consistent with the `.editorconfig`, but some rules may be interpreted differently by
  the C# tooling. When this happens, accept tooling's interpretation.

## Make the code as informative and intuitive as possible

- **Use `nameof(...)` instead of hardcoding type or member names as string literals**.
  Hard-coded literals are more likely to get out-of-sync during refactoring.
- **Make types and members `readonly` or `volatile` if possible**.
  Types should be as immutable as possible. `readonly` helps to communicate and enforce that. While `volatile` doesn't guarantee
  thread safety, it helps to communicate that a field can be modified by multiple threads. Legitimate use of mutable types
  that don't need thread safety is rare, so a field that's neither `readonly` nor `volatile` usually indicates a design problem 
- **Make types, members and lambdas `static` if possible**.
  Types that can't be instantiated, members that don't need instance state and lambdas that don't need to capture variables
  should be static to communicate this explicitly and encourage thread-safety and efficiency.
- **Make types and members `abstract` or `sealed` if possible**.
  Inheritance is a first-class design consideration and should be communicated as an explicit contract.
- **Make member visibility specifiers represent the actual member visibility**.
  For example, effective visibility of a `public` method in an `internal` class is `internal`, specify `internal` instead.
  When `public` visibility is required by the implicit implementation of an interface method, implement it explicitly instead.

  ❌ Instead of this:
  ```csharp
  class Foo: IDisposable
  {
      public string Bar() {}
      public void Dispose() {}
  }
  ```
  ✅ Do this:
  ```csharp
  class Foo: IDisposable
  {
      internal string Bar() {}
      void IDisposable.Dispose() {}
  }
  ```
- **Specify variable type explicitly unless it's obvious**. This helps the reader understand the code without having to lookup the type.
  Instead of ❌`var foo = Environment.GetEnvironmentVariable("foo");` do ✅`string? foo = Environment.GetEnvironmentVariable("foo");`.
  - _Use `var` to prevent duplication of the variable type in the initialization expression_.
    Instead of ❌`DateTime today = DateTime.Today;` do ✅`var today = DateTime.Today;`
  - _Prefer explicitly-typed variables with target-typed `new()` expressions_ over the more verbose `var` syntax.
    Instead of ❌`var bar = new Bar();` do ✅`Bar bar = new();`

- **Don't use target-typed `new()` expressions when the type is not apparent** and requires reader to look it up elsewhere,
  such as when initializing a field declared separately. Instead of ❌`baz = new();` do ✅`baz = new Baz();`

## Make the code as concise as possible

- **Use file-scoped namespaces instead of `{}`-scoped classic syntax**.
  - Example: `namespace Microsoft.ServiceFabric.Services;`.
  - If a C# files contains multiple namespaces, split it instead.
  - Add a blank line after the namespace declaration.
- **Use simple using statements instead of `{}`-scoped classic syntax**.
  E.g. `using StreamReader reader = File.OpenText(filePath);`. If a method requires scoped using statements, split it instead.
- **Don't specify optional visibility keywords**.
  C# has carefully designed defaults that should be obvious to engineers and agents working on code in this repo. For example,
  top-level types in C# are `internal` by default, so they should specify visibility only when they are `public`. Likewise,
  type members are `private` by default, so they should specify visibility only when they are `public`, `protected` or `internal`.
- **Use auto properties and events**.
  Hand-coding what compiler can generate only adds noise and doesn't convey any additional information.
- **Use fields instead of properties in internal APIs**.
  The .NET guidance to use properties over fields is meant to make the public/protected APIs forward compatible.
  It is not applicable to most private and internal APIs and only makes them more verbose and misleading.
- **Use expression-bodied members**.
  This helps to eliminate curly braces and their otherwise blank lines of code.
  - Place the body on the next line for methods and constructors, particularly when it's long, to keep code readable.
- **Use primary constructors**.
  This helps to eliminate multiple lines of code for SOLID/simple types.
  - In legacy types, with a significant number of constructor parameters or constructor overloads, the classic syntax may
    be more readable.
- **Omit redundant suffixes from field, variable and parameter names**.
  Terms like `Token`, `Stream`, `Source`, etc. are often redundant as suffixes in names because they duplicate information
  readily available from the type. E.g. `CancellationToken` parameter names should be named `cancellation`, not `cancellationToken`.
  - Apply this rule to new `public`/`protected` and all `internal`/`private` APIs.
  - Suffixes can still be used when needed to distinguish between two types that must be used in the same scope,
    e.g. `cancellationToken` and `cancellationSource`.
  - Consider this rule for existing `public`/`protected` APIs in major new versions. It's a build-breaking change.
- **Don't add `Async` suffix to internal/private methods**.
  It is already obvious from the `Task` return type. For public APIs, we still need to use `Async` suffixes for consistency
  with the existing .NET and Service Fabric APIs.
- **Place attributes on the same line until its length reaches the [wrap threshold](./coding.instructions.md)**.
  Multiple attributes are usually related, and should be placed on the same line, just like function parameters.
  ```csharp
  [EditorBrowsable(EditorBrowsableState.Never)]                              // ❌ Wrong
  [Obsolete("Use X.Y instead")]                                              // ❌ Wrong
  [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use X.Y instead")] // ✅ Correct
  ```
- **Use method syntax to invoke delegates**.
  It is more terse than the `.Invoke()` syntax and encourages more careful naming of delegate variables.
  ```csharp
  Action act = () => { };
  act.Invoke(); // ❌ Wrong
  act();        // ✅ Correct
  ```

- **Don't allow low-level exceptions; throw the most specific `ArgumentException` descendant instead**.
  Member access (`arg.M()`, `arg.X`), indexing (`arg[i]`), null-forgiving (`arg!`), downcast (`(T)arg`), `await arg`, and 
  similar operations dereference the argument and shouldn't throw the low-level `NullReferenceException`, `InvalidCastException`,
  `IndexOutOfRangeException`, etc. `this` arguments in extension methods are not exempt.
  ```csharp
  static void Wrong(this IFoo foo) => foo.Bar();                                                     // ❌ NullReferenceException when foo is null
  static void Correct(this IFoo foo) => (foo ?? throw new ArgumentNullException(nameof(foo))).Bar(); // ✅ ArgumentNullException
  ```

- **Don't validate pass-through arguments**.
  They should be validated by the code accessing them, making the additional validation redundant.
  ```csharp
  void Receive(string value) => if (value is null) throw new ArgumentNullException(nameof(value)); // Accessed after validation
  void Wrong(string value) => Receive(value ?? throw new ArgumentNullException(nameof(value)));    // ❌ Wrong
  void Correct(string value) => Receive(value);                                                    // ✅ Correct
  ```
  Redundant validation is acceptable when the pass-through argument is received and handed-off on different threads that
  don't share call context. This helps to Fail Fast and simplify debugging of what would otherwise be a truncated call stack.
  ```csharp
  void Receive(string value) => if (value is null) throw new ArgumentNullException(nameof(value)); // Truncated call stack
  void Callback(object? state) => Receive((string)value);                                          // Accessed on a different thread
  void Correct(string value) => new Timer(Callback, value ?? throw new ArgumentNullException(nameof(value)), 0, Timeout.Infinite); // ✅ Correct
  ```

## Reduce potential merge conflicts

- Keep package, project and assembly references in separate groups, sorted alphabetically in the project and solution files.
- Keep `using` directives sorted alphabetically, with `System` above others.
- If conditional compilation is required, use `#if NET` for .NET Core code, `#if NETFRAMEWORK` for .NET Framework code.
