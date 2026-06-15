---
description: "Use when writing or reviewing C# code."
applyTo: "**/*.cs"
---

# C# Guidelines

- Follow [.NET Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/) unless overridden by the rules below.
- Pay attention to the `file_header_template` setting in `.editorconfig`; existing files may have legacy headers that differ from the template.

## Make the code as informative and intuitive as possible

- Make types and members `readonly` or `volatile` if possible.
- Make types and members `abstract` or `sealed` if possible.
- Make member visibility specifiers represent the actual member visibility.
  For example, effective visibility of a `public` method in an `internal` class is `internal`, specify `internal` instead.
  When `public` visibility is required by the implicit implementation of an interface method, implement it explicitly instead.
  Instead of this:
  ```csharp
  class Foo: IDisposable
  {
      public string Bar() {}
      public void Dispose() {}
  }
  ```
  Do this:
  ```csharp
  class Foo: IDisposable
  {
      internal string Bar() {}
      void IDisposable.Dispose() {}
  }
  ```

## Make the code as concise as possible

- Use file-scoped namespaces, i.e. `namespace Microsoft.ServiceFabric.Services;`
- Don't specify optional visibility keywords. For example, top-level types in C# are `internal` by default, so they should
  specify visibility only when they are `public`. Likewise, type members are `private` by default, so they should
  specify visibility only when they are `public`, `protected` or `internal`.
- Use auto properties.
- Use expression-bodied members, but place the body on the next line for methods and constructors, particularly when it's long.
- Use primary constructors.
- Shorten `CancellationToken` parameter names to `cancellation` because the term _Token_ is usually redundant.
  This is safe to do in new code as well as internal/private code. This is a build-breaking change in public APIs.
- Don't add `Async` suffix to internal/private methods because it is already obvious from the `Task` return type.
  For public APIs we still need to use `Async` suffixes for consistency with the existing .NET and Service Fabric APIs.

## Reduce potential merge conflicts

- Keep package, project and assembly references in separate groups, sorted alphabetically in the project and solution files.
- Keep `using` directives sorted alphabetically, with `System` above others.
- If conditional compilation is required, use `#if NET` for .NET Core code, `#if NETFRAMEWORK` for .NET Framework code.
