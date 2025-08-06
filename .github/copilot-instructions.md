# GitHub Copilot Instructions for Service Fabric Services and Actors .NET

This repository contains the Service Fabric Services and Actors .NET SDK. When contributing to this project, please follow these guidelines:

## Project Overview
- This is a Microsoft Service Fabric SDK for .NET
- Contains APIs for Reliable Services and Reliable Actors
- Supports both .NET Framework and .NET targets
- Uses MSBuild and NuGet for package management

## Coding Standards, Formatting, and Style (from .editorconfig)
NOTE: Since Copilot can't read .editorconfig, please ensure the following standards are followed:

### General File Formatting
- Use spaces for indentation (never tabs)
- Use CRLF line endings for all files
- End files with CRLF
- Use UTF-8 charset for code files
- Insert final newline in code files

### Indentation Rules
- **C# files (*.cs)**: 4 spaces
- **XML files** (props, targets, csproj, json etc.): 2 spaces

### C# Language Conventions
- **Variable declarations**: Always use `var` (built-in types, apparent types, elsewhere)
- **Type preferences**: Use language keywords instead of BCL types (`int` not `Int32`)
- **this. qualifier**: Avoid using `this.` for fields, properties, methods, and events
- **Accessibility modifiers**: Always specify accessibility modifiers
- **Null checking**: Use null propagation operators and coalesce expressions
- **Expression-bodied members**: 
  - Properties, indexers, accessors: Use expression bodies
  - Methods, constructors, operators: Use block bodies (not expression bodies)
- **Pattern matching**: Prefer pattern matching over `is` with cast checks
- **Braces**: Use braces for multiple lines, avoid for single lines when appropriate

### C# Formatting Rules
- **New lines**: Place opening braces on new lines for all constructs
- **Control flow**: New line before `else`, `catch`, `finally`
- **Object/anonymous initializers**: New line before members
- **Switch statements**: Indent case contents and switch labels
- **Labels**: Flush left alignment
- **Spaces**: 
  - No space after cast operators
  - Space after keywords in control flow statements
  - No spaces between method parentheses
  - No spaces between other parentheses

### Naming Conventions
- **Interfaces**: PascalCase with `I` prefix (e.g., `IMyInterface`)
- **Classes, structs, enums, properties, methods, events, namespaces, delegates**: PascalCase
- **Constant fields**: PascalCase
- **Static fields**: camelCase
- **Public fields**: PascalCase
- **Private/internal fields**: camelCase
- **Parameters, local variables**: camelCase

## Testing
- Use MSTest framework following existing patterns
- Use reflection if needed
- When writing tests use hierarchical pattern used in existing tests. For example:
  public abstract class SystemUnderTestClassTest
    {
        public SystemUnderTestClassTest()
        {
            // Setup common test context
        }

        public sealed class MethodName : SystemUnderTestClassTest
        {
            [Fact]
            public void ShouldDoSomething()
            {
                // Arrange
                // Act
                // Assert
            }
        }
    }
    
## Documentation
- Update XML documentation for all public APIs

## Dependencies
- Minimize external dependencies
