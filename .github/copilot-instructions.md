# GitHub Copilot Instructions for Service Fabric .NET SDK

This repository contains the Service Fabric .NET libraries.

## Quick Reference

- **Restore**: `dotnet restore`
  - Restore packages at the beginning to reduce subsequent build times
- **Build**: `dotnet build -c Release --no-restore`
  - Build specific projects to reduce build time
- **Test**: `dotnet test -c Release --no-restore`
  - Run tests for specific projects to reduce test execution time
  - We have known test failures in `Debug` configuration
  - Run tests with `-f net472` and/or `-f net10.0` to speed up change verification
  - Run all tests on all frameworks before considering the change completed
- **Pack**: `dotnet pack` (output: `out/packages/`)
- **Prerequisites**: Run `init.cmd` (Windows) or `init.sh` (Linux) first
- See [CONTRIBUTING.md](../CONTRIBUTING.md) for full setup instructions

## Project Structure

| Directory             | Purpose                                                |
|-----------------------|--------------------------------------------------------|
| `src/`                | Product projects (libraries shipped as NuGet packages) |
| `src/Constants`       | Compile-time constants used by other projects          |
| `test/`               | Test projects (xUnit, one per src project)             |
| `test/TestFramework/` | Shared test utilities                                  |
| `properties/`         | Shared MSBuild props, signing key                      |
| `refs/`               | Reference assemblies without NuGet packages            |

Product projects target `net8.0;net462` with the exception of the `Client.Http` and `PowerShell.Http` which still target
`netstandard2.0` at this time. Test projects target `net10.0;net9.0;net8.0;net472`.

The `src/` and `test/` directories contain sub-directories named based on the projects within them. For example, the `src/Actors`
directory contains the `Microsoft.ServiceFabric.Actors.csproj` and the `test/Actors` contains the `Microsoft.ServiceFabric.Actors.Tests.csproj`
projects. We omit the `Microsoft.ServiceFabric` prefix and the `Tests` suffix from the directory names.

## Coding Conventions

- Follow the following principles unless overridden by the rules below
  - [Beck's Rules of Simple Design](https://martinfowler.com/bliki/BeckDesignRules.html)
  - [.NET Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
  - [Robert Martin's SOLID Design Principles](https://en.wikipedia.org/wiki/SOLID)
  - [Jim Shore's Fail Fast Principle](https://martinfowler.com/ieeeSoftware/failFast.pdf)

- Make the code as concise as possible.
  - Don't use unnecessary braces.
  - Before adding braces, try to make them unnecessary by extracting a function, a class, etc.
  - Use file-scoped namespaces, i.e. `namespace Microsoft.ServiceFabric.Services;`
  - Don't specify optional visibility keywords. For example, top-level types in C# are `internal` by default, so they should
    specify visibility only when they are `public`. Likewise, type members are `private` by default, so they should
    specify visibility only when they are `public`, `protected` or `internal`.
  - Use auto properties.
  - Use expression-bodied members, but place the body on the next line for methods and constructors, particularly when it's long
  - Use primary constructors;
  - Shorten parameter and variable names to the minimum needed to understand them in context. For example, shorten name of 
    a `CancellationToken` parameter to `cancellation` because the term _Token_ is usually redundant. This is safe to do in
    new code as well as internal/private code. This is a build-breaking change in public APIs.
  - Don't add `Async` suffix to the internal/private methods because it is already obvious from the `Task` return type.
    For public APIs we still need to use `Async` suffixes for consistency with the existing .NET and Service Fabric APIs.
  - Don't add comments re-stating the information already available from the member declaration;
  - Before adding comments, try to make them unnecessary by breaking up the code into multiple functions, classes, etc.
  - Don't wrap code lines until they significantly exceed 120 characters.
  - Don't create a new namespace until you have a significant number of types ready to be moved there.

- Make the code as informative and intuitive as possible.
  - Folder structure within a project should match the namespace structure.
  - Define top-level types in separate files with a matching name.
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

- Reduce potential merge conflicts in the future
  - Keep package, project and assembly references in separate groups, sorted alphabetically in the project and solution files.
  - Keep using directives sorted alphabetically in the C# files, with `System` assemblies above others.

- Read and follow the guidelines codified in the [.editorconfig](../.editorconfig) before writing or reviewing code
  - Pay attention to the `file_header_template` setting; existing files may have legacy headers that differ from the template

### Documentation
- Add doc-comments for public APIs only
- Do not add doc-comments to internal or private members
- Do not add optional doc-comment sections unless they contain distinct information. For example, don't add a `<value>`
  section for a property if it already has the same information in the `<summary>` section. Also, don't add `<param>` sections
  for a function if they re-state the information already obvious from the respective parameter names and types.
- Use tags and attributes [valid for C#](https://learn.microsoft.com/dotnet/csharp/language-reference/xmldoc/recommended-tags);
  Don't use undocumented tags like `<cref name="...">` or incorrect attributes.
- Don't use redundant `<para>` tags. They are needed only when the parent section, such as `<remarks>` contains multiple
  paragraphs.

## Build Conventions

- **Central package management**: All package versions defined in `Directory.Packages.props`
- **C# version**: `latestMajor` (set in `Directory.Build.props`)
- **Assembly signing**: Delay-signed with `properties/Key.snk`
- **Conditional compilation**: `#if NET` for .NET Core code, `#if NETFRAMEWORK` for .NET Framework code
