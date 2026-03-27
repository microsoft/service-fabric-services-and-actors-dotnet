---
applyTo: "test/**"
---
# Test Conventions

## Structure Test Projects To Mirror Product Project

- **Folders**: The `test` folder containing test projects should have the same names as the `src` folder.
  For example, the `/test/Actors/` folder contains tests for the `/src/Actors/` product code.
- **Projects**: Test projects should have the same name as their respective product projects, followed by the `.Tests`
  suffix, (plural "Tests"). For example, the `Microsoft.ServiceFabric.Actors.Tests.csproj` tests the `Microsoft.ServiceFabric.Actors.csproj`.
- **Namespaces**: Test projects should use the product namespace, **without** the "Tests" suffix. This helps to reduce the
  number of namespaces and using directives. For example, the `Microsoft.ServiceFabric.Actors.Tests.csproj` test classes
  should be in the `Microsoft.ServiceFabric.Actors` namespace.
- **Classes**: Each public product type should have a separate test class `{TypeUnderTest}Test` (singular "Test").
  For example, the `ActorBase` product class should have a test class `ActorBaseTest`.

## Avoid Integration Tests

Integration tests typically don't have the 1:1 equivalency with the product code and tend to drift away over time.
They are also often fragile, producing flakey results and difficult to modify over time. Instead of integration tests,
strive to unit test each product type in isolation of its dependencies.

## Structure Test Classes To Mirror Product Types

Use nested classes to group test methods for the same member of the target type.

Given the following product type:

```csharp
namespace Microsoft.ServiceFabric.Diagnostics.Tracing;

sealed class Trace : ITrace, IEquatable<Trace>
{
    readonly string type;
    readonly string id;
    readonly ITextEventSource events;

    internal Trace(Type type, ServiceContext context, ITextEventSource events)
    {
        this.type = (type ?? throw new ArgumentNullException(nameof(type))).Name;
        id = TraceId(context ?? throw new ArgumentNullException(nameof(context)));
        this.events = events ?? throw new ArgumentNullException(nameof(events));
    }

    void ITrace.Error(string message) =>
        events.ErrorText(id, type, message);

    string TraceId(ServiceContext context) =>
        $"{context.PartitionId:B}:{context.ReplicaOrInstanceId}";
}
```

The test class should be structured similar to this:

```csharp
namespace Microsoft.ServiceFabric.Diagnostics.Tracing;

public abstract class TraceTest
{
    readonly ITrace sut;

    // Constructor parameters
    readonly Type type = fuzzy.Type();
    readonly ServiceContext context = fuzzy.ServiceContext();
    readonly ITextEventSource events = Mock.Of<ITextEventSource>();

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    protected TraceTest() =>
        sut = new Trace(type, context, events);

    public sealed class Constructor : TraceTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenTypeIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Trace(null, context, events));
            Assert.Equal(nameof(type), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Trace(type, null, events));
            Assert.Equal(nameof(context), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenEventsIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Trace(type, context, null));
            Assert.Equal(nameof(events), exception.ParamName);
        }
    }

    public sealed class Error : TraceTest
    {
        // Method parameters
        readonly string message = fuzzy.String();

        readonly string traceId;

        public Error() =>
            traceId = context.PartitionId.ToString("B") + ":" + context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture);

        [Fact]
        public void EmitsErrorTextEvent()
        {
            sut.Error(message);
            Mock.Get(events).Verify(_ => _.ErrorText(traceId, type.Name, message));
        }
    }
}
```

Key elements:
- **Base class**: `abstract` to communicate that it has no test methods of its own.
- **SUT variable**: A field called `sut` (system under test) is declared at the top of the class help reader understand
  what is being tested. If the SUT implements a specific interface, the variable should be of the interface type to communicate
  that the SUT is used primarily through this abstraction.
- **Constructor parameters**: Are listed as additional fields below the `sut` to help the reader see how the instances are
  created. Add a blank line and a comment above the constructor parameters to clearly indicate what they are.
- **Test fixture**: Other fields and methods shared by the test methods below. Add a blank line and a comment above the
  first line of the test fixture to clearly indicate where it begins.
- **Nested classes**: `public sealed`, named after the target being tested, inherit from the base
- **Test methods**: PascalCase descriptive names that should form valid English sentences when read together with the class
  names. For example `TraceTest.Constructor.ThrowsArgumentNullExceptionWhenTypeIsNull`

When testing common methods, the name of the nested test sub-class may conflict with the name of a base class member. 
For example, if SUT overrides the `Object.GetHashCode()` method that needs to be tested, the nested test sub-class cannot
be called simply `GetHashCode` because it would conflict with the method of the test class itself.

- Always try resolving the conflict by adding the `new` keyword to the test sub-class first, `public new sealed class GetHashCode: FooTest`.

- When testing `Dispose()` method of a SUT where the base test class also implements a `Dispose()` method to teardown the
  test fixture, change the test base to implement it explicitly - `void IDisposable.Dispose()`

- When the test base `Dispose()` method is virtual because it needs to be overridden by the nested test sub-classes, it'd
  take significantly more code to make it explicit. In this case, add `_` suffix to the test sub-class name instead - 
  `public sealed class Dispose_: FooTest`. The `_` suffix is _simpler_ than `Tests`.

## Structure Test Methods

- Each test method should verify a single logical aspect of a single member of the target type.
  Multiple assertions per test method are OK as long as they test the same specific logical aspect of the target.

- Each test method should have clearly visible _arrange_, _act_ and _assert_ sections.

```csharp
public abstract class FooTest
{
    public sealed class Bar: FooTest
    {
        [Fact]
        public void ReturnsBuz()
        {
            // Arrange
            var sut = new Foo();

            // Act
            string result = sut.Bar();

            // Assert
            Assert.Equal("Buz", result);
        }
    }
}
```

- Omit the _arrange_, _act_, _assert_ section comments unless they are needed to separate sections that contain multiple
code paragraphs separated by blank lines. The example above can be condensed to:

```csharp
public abstract class FooTest
{
    public sealed class Bar: FooTest
    {
        [Fact]
        public void ReturnsBuz()
        {
            var sut = new Foo();

            string result = sut.Bar();

            Assert.Equal("Buz", result);
        }
    }
}
```

- Omit the blank lines between the sections when each section consists of a single statement. The example above
can be further condensed to:

```csharp
public abstract class FooTest
{
    public sealed class Bar: FooTest
    {
        [Fact]
        public void ReturnsBuz()
        {
            var sut = new Foo();
            string result = sut.Bar();
            Assert.Equal("Buz", result);
        }
    }
}
```

- If the same _arrange_, and sometimes the _act_ logic has to be repeated multiple times, reduce duplication by extracting
it into the constructors of the test classes. 

```csharp
public abstract class FooTest
{
    readonly Foo sut = new();

    public sealed class Bar: FooTest
    {
        [Fact]
        public void ReturnsBuz()
        {
            string result = sut.Bar();
            Assert.Equal("Buz", result);
        }
    }
}
```

## Generate Test Inputs

Avoid hard-coded test inputs and use `Fuzzy` to generate them instead. See [fuzzy.instructions.md](fuzzy.instructions.md) 
for API details.

```csharp
using Fuzzy;

static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

string s = fuzzy.String();
```

Create `IFuzz` extensions to generate instances of types not supported by Fuzzy. This eliminates duplication
when inputs of the same type are needed by different test classes. You can also create an extension even when it's only
needed once if it helps to reduce complexity of the test setup. Use extensions in the `TestFramework` project for types
needed by multiple test projects.

## Use Moq for mocking

Use [Moq](https://github.com/devlooped/moq) to stub or observe behavior of dependencies external to the component being
tested.

```csharp
// Simple mock (most cases)
readonly ICommunicationListener listener = Mock.Of<ICommunicationListener>();

// Mock needing configuration
readonly IServiceInstance instance = new Mock<IServiceInstance> { DefaultValue = DefaultValue.Mock }.Object;

// Setup and verify via Mock.Get
Mock.Get(listener).Setup(l => l.OpenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("endpoint");
Mock.Get(listener).Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
```

## Use xUnit Assertions

Use the most specific xUnit `Assert.*` methods:

```csharp
using Xunit;

Assert.Equal(expected, actual);
Assert.Same(expected, actual);
Assert.True(condition);
Assert.Throws<ArgumentNullException>(() => action());
```

Don't use `FluentAssertions` in new tests. Change `FluentAssertions` to `Xunit` when making significant changes in the
test code.

## Use Inspector for white-box testing

Ideally, we should use injection of constructor parameters to separate SUT from its dependencies. However, this may not
be possible in existing public types or not practical. We use `Inspector` to access private members in unit tests instead.
See [inspector.instructions.md](inspector.instructions.md) for API details.

```csharp
using Inspector;

Bar value = sut.Field<Bar>().Value; // Access private fields
Foo foo = Type<Foo>.New(); // Create types with private constructors
Foo foo = Type<Foo>.Uninitialized(); // Skip constructor
```

## Platform-Specific Tests

Use `[WindowsOnly("reason")]` from `TestFramework` to skip tests on Linux.

## Project References

Test projects reference:
- `xunit.v3` — test framework
- `Inspector` — whitebox testing
- `Moq` — mocking
- `Fuzzy` — random test data
- `TestFramework` project — shared SF test utilities
