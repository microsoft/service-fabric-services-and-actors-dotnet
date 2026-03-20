---
applyTo: "test/**"
---

# Test Input Generation
- Avoid hard-coded test inputs; use [Fuzzy](https://github.com/olegsych/fuzzy) to generate them instead.
- Fuzzy is a test input generation library that produces random values for common .NET types.
- Before writing fuzzy code, fetch the examples from the official repo using `mcp_github_github_get_file_contents`
  with `owner: "olegsych"`, `repo: "fuzzy"` and `ref: "refs/heads/master"` for the following files:
  - `examples/ArrayExample.cs`
  - `examples/CharExample.cs`
  - `examples/CustomTypeExample.cs`
  - `examples/DictionaryExample.cs`
  - `examples/Int32Example.cs`
  - `examples/ListExample.cs`
  - `examples/StringExample.cs`
  - `examples/TimeSpanExample.cs`
- Make sure the test project has a `<ProjectReference Include="Fuzzy"/>`
- Check for Service Fabric-specific extensions in the `test/TestFramework/IFuzzExtensions.cs`.
- Place project-specific extensions in the individual test projects, like `test/Services/IFuzzExtensions.cs`.

## API Cheat-Sheet
```csharp
static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

// Primitives
bool b = fuzzy.Boolean();
byte n = fuzzy.Byte();
int i = fuzzy.Int32();
long l = fuzzy.Int64();
double d = fuzzy.Double();
float f = fuzzy.Single();

// Constrained numerics
int ranged = fuzzy.Int32().Between(5, 10);
long minimum = fuzzy.Int64().Minimum(100);
long maximum = fuzzy.Int64().Maximum(999);

// Strings
string s = fuzzy.String();
string alphanumeric = fuzzy.String().LettersOrDigits();

// Other types
Uri uri = fuzzy.Uri();
DateTime dt = fuzzy.DateTime();
TimeSpan ts = fuzzy.TimeSpan();
MyEnum e = fuzzy.Enum<MyEnum>();

// Collections
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
