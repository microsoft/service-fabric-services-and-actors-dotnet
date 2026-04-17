---
description: "Use when writing or reviewing tests."
applyTo: "test/**/*.cs"
---

# Test Input Generation with Fuzzy
[Fuzzy](https://github.com/olegsych/fuzzy) is a test input generation library that produces random values for common .NET types.

## Guidelines

- Verify that `Fuzzy` defaults are not acceptable before adding any constraints. 
  Constraints reduce uniqueness of fuzzy values and reliability of the tests as well. Most `Fuzzy` defaults should be
  applicable to most scenarios, with rare legitimate exceptions for cases when one fuzzy value must be constrained by
  another fuzzy value, such as start/stop dates of an operation, or when SUT allocates resources based on a fuzzy value.
  Note that heap allocating functions like `Array()`, `List()`, `String()` have default constraints built-in.

- Use limited constraints `Minimum()`/`Maximum()` first and verify that a more strict `Between()` is required before using it.
  Constraints serve as documentation of the SUT and over-specifying them misrepresents it.

- Use built-in constraints like `.TimeSpan().Seconds()` instead of ad-hoc `TimeSpan.FromSeconds(fuzzy.Int32().Between(1,5))`.
  `Fuzzy` is designed to make test code terse and readable; ad-hoc constraints should be rare.

- Check for extensions applicable to multiple projects in the `test/TestFramework/IFuzzExtensions.cs`.

- Place project-specific extensions in the individual test projects, like `test/Services/IFuzzExtensions.cs`.

- **Don't assert on fuzzy values of limited sets, such as `bool` and `enum`**.
  A fuzzy value picked from a limited set is likely to match the value expected by test regardless of the logic being tested.
  This can produce false negatives and flaky tests.
  - Instead, use xUnit `[Theory]` with `[InlineData]` to cover all possible values.
  - Do use fuzzy `bool` and `enum` values when _any_ value is needed rather than hard-coding it.

- **Derive "different" values rather than generating them independently**. When you have a fuzzy value `x` and need a
  different value (typically in inequality tests), derive it from the existing value instead of generating it independently.
  This communicates "a different value" rather than "any value" and eliminates collision risk for small domains.
  - For `bool`, use `!x`
  - For strings, use `x + fuzzy.String()`
  - For numbers, use `x + fuzzy.SByte().Between(1, 5)`
  - For dates and timestamps, use `x + fuzzy.TimeSpan().Seconds()`

- Report unexpected `Fuzzy` errors to the user.
  - Ask them to submit an issue to the `olegsych/fuzzy` repo on GitHub.
  - When implementing workarounds, add TODO comments with the explanation, package version and GitHub issue link.
  - Remove workarounds once a new version of `Fuzzy` is available with the fix.

## API Examples
```csharp
using Fuzzy;

static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

// Primitives
bool b = fuzzy.Boolean();
byte n = fuzzy.Byte();
int i = fuzzy.Int32();
long l = fuzzy.Int64();
double d = fuzzy.Double();
float f = fuzzy.Single();

// IComparable<T> values can be constrained
int ranged = fuzzy.Int32().Between(5, 10);
TimeSpan minimum = fuzzy.TimeSpan().Minimum(TimeSpan.FromMinutes(2));
DateTime maximum = fuzzy.DateTime().Maximum(DateTime.Now);

// Strings
string s = fuzzy.String();
string alphanumeric = fuzzy.String().LettersOrDigits();

// Other types may have unique constraints
Uri uri = fuzzy.Uri();
DateTime dt = fuzzy.DateTime().Between(DateTime.Now, TimeSpan.FromDays(2));
TimeSpan ts = fuzzy.TimeSpan().Seconds();
MyEnum e = fuzzy.Enum<MyEnum>();

// Collections can be constrained to a desirable size range
byte[] bytes = fuzzy.Array(fuzzy.Byte);
byte[] sized = fuzzy.Array(fuzzy.Byte, Length.Between(10, 20));
List<string> list = fuzzy.List(fuzzy.String);
List<string> exact = fuzzy.List(fuzzy.String, Count.Exactly(3));

// Pick from existing collection
string element = fuzzy.Element(enumerable);
int index = fuzzy.Index(enumerable);

// Custom types — create IFuzz extensions
static MyType MyType(this IFuzz fuzzy) =>
    new(fuzzy.String(), fuzzy.Int32());
```

If examples above are insufficient, fetch more from the `olegsych/fuzzy` repo for the following files:
- `examples/ArrayExample.cs`
- `examples/CharExample.cs`
- `examples/CustomTypeExample.cs`
- `examples/DictionaryExample.cs`
- `examples/Int32Example.cs`
- `examples/ListExample.cs`
- `examples/StringExample.cs`
- `examples/TimeSpanExample.cs`
