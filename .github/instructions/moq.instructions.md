---
description: "Use when writing or reviewing tests."
applyTo: "test/**"
---

# Moq Conventions

Use [Moq](https://github.com/devlooped/moq) to stub or observe behavior of dependencies external to the component being
tested.

## Use simple mocks by default

Prefer storing mock objects in variables of their own type, rather than `Mock<T>`, to make the code more terse and readable.
```csharp
readonly ICommunicationListener listener = Mock.Of<ICommunicationListener>();
Mock.Get(listener).Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
```

## Enable default implementations explicitly

Moq doesn't implement abstract/interface methods by default and has to be explicitly instructed to.
```csharp
readonly IServiceInstance instance = new Mock<IServiceInstance> { DefaultValue = DefaultValue.Mock }.Object;
```

## Make test failures readable

When using `.Verify(...)`, always run the failing test first and make sure the failure message is understandable to a human reader.
When failure messages become difficult to understand in non-trivial assertions, replace `.Verify(...)` assertions with
`.Setup(...)` and `.Callback(...)` in the arrange section of the test to capture the actual values and use plain xUnit
`Assert.*` calls instead of `.Verify(...)`.

### Example: Verifying native struct parameters passed as IntPtr

When a mock method takes an `IntPtr` pointing to a native struct (e.g., `FABRIC_METER_DESCRIPTION`), use Moq's `Callback`
to capture the struct contents **during** the call, while the `GCHandle` pins are still alive. Then assert on the captured
values afterward. Do not use `Verify` with `It.Is<IntPtr>(...)` — the pointer is dangling after the call returns.

```csharp
// Arrange
string actualNamespace;
string actualName;
string[] actualDimensionNames;

Mock.Get(provider)
    .Setup(x => x.CreateMeter(It.IsAny<IntPtr>()))
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
