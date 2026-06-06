---
description: "Use when writing or reviewing tests."
applyTo: "test/**/*.cs"
---

# Moq Conventions

Use [Moq](https://github.com/devlooped/moq) to stub or observe behavior of dependencies external to the component being
tested.

## Choose variable type to reduce verbosity of Mock calls

Use `Mock<T>` variables when mocked objects make many `Setup*()` and `Verify*()` calls.

```csharp
Mock<IReliableStateManagerReplica2> impl = new();
_ = impl.Setup(_ => _.BackupAsync(callback)).Returns(expected); // Majority case - multiple Mock<T> calls in arrange/assert blocks.
// Unwrapping of mocks is more verbose and should be rare
var sut = new ReliableStateManager(impl.Object);
```

Use unwrapped `T` variables when mocked objects are mostly passed through, with no or rare usage of `Mock<T>` calls in the
arrange/assert blocks.

```csharp
ICommunicationListener listener = Mock.Of<ICommunicationListener>();
var sut = new Service(listener); // Majority case - unwrapped object passed through
// Mock<T> calls with unwrapped instances are more verbose and should be rare
_ = Mock.Get(listener).Setup(_ => _.OpenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("http://localhost:80");
```

## Don't include the term Mock in variable names

For variables of type `Mock<T>`, it's redundant and a form of Hungarian naming convention that shouldn't be used in
strongly-typed modern languages. For variables of type `T`, it's an implementation detail and shouldn't be important.

## Enable default implementations explicitly

`Moq` doesn't implement abstract/interface methods by default and has to be explicitly instructed to.

```csharp
var instance = new Mock<IServiceInstance> { DefaultValue = DefaultValue.Mock };
```

## Use discards to reduce noise

- Assign `Setup()` chains to a discard `_` variable.
  This explains that the expression value is not used intentionally and suppresses the `IDE0058` warnings.
- Use discards for lambda parameters in `Setup()`/`Verify()`/etc. calls.
  Name variables only when they are required or communicate something meaningful.

```csharp
_ = dep.Setup(_ => _.DoAsync(arg)).Returns(expected);
dep.Verify(_ => _.DoAsync(It.IsAny<string>()), Times.Once);
```

## Prefer mocks when testing APIs that depend on abstractions even if concrete implementations are sufficient

This rules out the possibility of SUT having a hidden dependency on a concrete implementation. It also eliminates the potential
confusion for someone scanning the API and misinterpreting concrete types used in tests as actual dependencies. Example
was compressed for size and not meant to demonstrate comments or formatting.
```csharp
class SUT(Stream stream) { public Stream stream;}
Stream expected = Mock.Of<Stream>(); // ✅ Mock<Stream>, ❌ not MemoryStream.
SUT sut = new(expected);
Assert.Same(expected, sut.stream);
```

## Make test failures readable

When using `.Verify()`, always run the failing test first and make sure the failure message is understandable enough
to explain the test failure without debugging. When failure messages become too difficult to understand, replace `.Verify()`
assertions with `.Setup()` and `.Callback()` in the arrange section of the test to capture the actual values and use
plain xUnit `Assert.*` calls instead of `.Verify()`.

### Example: Verifying native struct parameters passed as IntPtr

When a mock method takes an `IntPtr` pointing to a native struct (e.g., `FABRIC_METER_DESCRIPTION`), use Moq's `Callback`
to capture the struct contents **during** the call, while the `GCHandle` pins are still alive. Then assert on the captured
values afterward. Do not use `Verify` with `It.Is<IntPtr>(...)` — the pointer is dangling after the call returns.

```csharp
// Arrange
string actualNamespace;
string actualName;
string[] actualDimensionNames;

_ = provider // Mock<IFabricMeterProvider>
    .Setup(_ => _.CreateMeter(It.IsAny<IntPtr>()))
    .Callback((IntPtr ptr) =>
    {
        var desc = Marshal.PtrToStructure<FABRIC_METER_DESCRIPTION>(ptr);
        actualNamespace = Marshal.PtrToStringUni(desc.Namespace);
        actualName = Marshal.PtrToStringUni(desc.Name);
        actualDimensionNames = PtrToStringArray(desc.DimensionNames, (int)desc.TotalDimensionsCount);
    })
    .Returns(fabricMeter);

static string[] PtrToStringArray(IntPtr array, int count)
{
    var result = new string[count];
    for (int i = 0; i < count; i++)
        result[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(array, i * IntPtr.Size));
    return result;
}

// Act
sut.CreateMeter(...);

// Assert
Assert.Equal(testNamespace, actualNamespace);
Assert.Equal(testMetric, actualName);
Assert.Equal(systemDimensionsNames, actualDimensionNames);
```

Naming conventions for captured values:
- Use the `actual` prefix (consistent with xUnit `Assert.Equal(expected, actual)` parameter naming)
- Name fields to match the struct property names, e.g., `actualNamespace`, `actualName`, `actualDimensionNames`
- Asserting array equality implicitly verifies count, so don't capture or assert counts separately

## Use strongest tests for dependency calls

- Use the specific arguments in `.Setup()` calls rather than `It.IsAny()` to verify that arguments are passed to the
  dependency correctly.
- Use unique/generated argument and return values instead of passing well-known values like `null`, `Task.CompletedTask`
  that could also be used by the product code unexpectedly.
- Use `Assert.Same` to verify return values, particularly for `Task<T>`, and rule out unexpected transformations, awaits
  and unhandled async exceptions.
- Use `It.IsAny()` in `.Verify(.., Times.Once)` rather than specific arguments to verify that dependency is _not called_
  with unexpected arguments.
- For void methods, where `.Setup()` cannot be combined with `Assert.Same` to verify argument forwarding, use specific
  arguments in an additional `.Verify(.., Times.Once)` call.

### Example: Non-void methods

```csharp
interface IDep { Task<string> DoAsync(string arg); }
class Sut(IDep dep) { internal Task<string> DoAsync(string arg) => dep.DoAsync(arg); }
Mock<IDep> dep = new();
Sut sut = new(dep.Object);
```

❌ Instead of this:
```csharp
string expected = "expected"; // literal hides unexpected use of well-known values by SUT
_ = dep.Setup(_ => _.DoAsync(It.IsAny<string>())).ReturnsAsync(expected); // IsAny() hides unexpected arguments passed to dependency

string arg = null; // null hides unexpected argument hard-coding by SUT rather than passing through as expected
string actual = await sut.DoAsync(arg); // await hides unnecessary awaits inside the SUT

Assert.Equal(expected, actual); // Equal() misses SUT making unnecessary copy of the result returned by dependency
dep.Verify(_ => _.DoAsync(arg), Times.Once); // arg misses SUT calling DoAsync with other arguments
```

✅ Do this:
```csharp
using Fuzzy;
static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

string arg = fuzzy.String(); // Unique value rules out unexpected use of well-known values
Task<string> expected = Task.FromResult(fuzzy.String()); // Unique Task instance rules out unexpected caching, sync completion. 
_ = dep.Setup(_ => _.DoAsync(arg)).Returns(expected); // Specific argument in Setup() proves argument forwarding

Task<string> actual = sut.DoAsync(arg);

Assert.Same(expected, actual); // Same() proves result return and async exception propagation
dep.Verify(_ => _.DoAsync(It.IsAny<string>()), Times.Once); // IsAny() proves no extra calls with other arguments
```

### Example: Void methods

Argument forwarding to void methods must be tested with unique argument values and an explicit `.Verify()` call.

```csharp
interface IDep { void Fire(string arg); }
class Sut(IDep dep) { internal void Fire(string arg) => dep.Fire(arg); }
Mock<IDep> dep = new();
Sut sut = new(dep.Object);

string arg = fuzzy.String(); // Unique value required rule to out unexpected use of well-known values by SUT

sut.Fire(arg);

dep.Verify(_ => _.Fire(arg), Times.Once); // Required to prove argument forwarding
dep.Verify(_ => _.Fire(It.IsAny<string>()), Times.Once); // Required to rule out unexpected additional calls
```

### Example: Non-void methods returning value types

Argument forwarding to methods returning value types must be tested with unique argument values and an explicit
`.Verify()` call when the return value is a default value of the type.


```csharp
interface IDep { bool Try(string arg); }
class Sut(IDep dep) { internal bool Try(string arg) => dep.Fire(arg); }
Mock<IDep> dep = new();
Sut sut = new(dep.Object);

string arg = fuzzy.String();           // Unique value required to rule out unexpected use of well-known values by SUT
bool expected = false;                 // default value of a value type
_ = dep.Setup(_ => _.Try(arg)).Returns(expected); // Works as dep.Setup(_ => _.Try(It.IsAny<string>()) when expected == default

bool actual = sut.Try(arg);

Assert.Equal(expected, actual);
dep.Verify(_ => _.Try(arg), Times.Once);                // Required to prove argument forwarding when expected == default
dep.Verify(_ => _.Try(It.IsAny<string>()), Times.Once); // Required to rule out unexpected additional calls
```
