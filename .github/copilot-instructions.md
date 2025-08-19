# GitHub Copilot Instructions for Service Fabric Services and Actors .NET

This repository contains the Service Fabric Services and Actors .NET SDK. When contributing to this project, please follow these guidelines:

## Project Overview
- This is a Microsoft Service Fabric SDK for .NET
- Contains APIs for Reliable Services and Reliable Actors
- Supports both .NET Framework and .NET targets
- Uses MSBuild and NuGet for package management

## Documentation
- Update XML documentation for all public APIs
- Only generate documentation for public APIs

## Testing
- Use xUnit framework following existing patterns

### Hierarchical Test Structure
- We use this structure to help organize tests and ensure clarity in test cases.
- When writing tests, use a hierarchical pattern similar to existing tests. For example:
  public abstract class SystemUnderTestClassTest
```csharp

    namespace Microsoft.ServiceFabric.TestedProject
    {
        public abstract class SystemUnderTestClassTest
        {
            // Setup common test context
            public SystemUnderTestClassTest()
            {
                // Initialize common resources
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
    }

```
- Avoid adding "Tests" suffix to namespaces in tests. This causes too many not needed imports to be required.
