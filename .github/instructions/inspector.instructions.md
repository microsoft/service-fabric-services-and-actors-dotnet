---
description: "Use when writing or reviewing tests."
applyTo: "test/**/*.cs"
---

# White-box Testing with Inspector

## Definition of Terms

- SUT - System Under Test, typically a `class` or `struct`.

## Guidelines

- **Use [Inspector](https://github.com/olegsych/inspector) instead of `System.Reflection`**.
  Inspector is a more terse and type-safe alternative specifically designed for white-box testing.
- **Test `internal` types and members without Reflection**.
  Reflection-based access to internals makes the tests more brittle and difficult to refactor.
  - Add `[assembly: InternalsVisibleTo("<TestAssembly>" + PublicKey)]` to the product project.
- **Don't test the `private` members**.
  They may be used to arrange or assert in tests for the `public` or `internal` APIs, but only if the tests aren't possible
  to implement otherwise.
- **Don't access `private` members when alternatives exist**.
  Accessing private members in tests indicates a testability issue with the SUT API. It should never be done as a test implementation
  short-cut. You should first evaluate the existing APIs and consider the possibility of adding new APIs to the SUT to implement
  the required tests. Only when the possibility of using alternative APIs is ruled out should you access the private members.
  ```csharp
  class Example: IDisposable
  {
     bool disposed;
     void Explain() { if (disposed) throw new ObjectDisposedException(default); }
     void Dispose() => disposed = true; 
  }
  Example sut = new();
  sut.Field<bool>().Set(true); // ❌ Wrong, alternative exists
  sut.Dispose();               // ✅ Correct.
  _ = Assert.Throws<ObjectDisposedException>(() => sut.Explain());
  ```
- **Prefer type-based filters**.
  SOLID/simple types use distinct member types, so filters like `.Field<Foo>()`, `.Property<Foo>()` should be sufficient.
- **Refactor private members to distinct types/signatures, if possible**.
  Multiple private members of the same type often indicate a design problem in the product code.
  E.g. replace multiple `bool` fields with distinct `enum` types or a single `[Flags] enum`.
- **Use declaration filters, if needed**.
  When multiple types in the inheritance chain declare members of the same type, declaration-based filters like
  `.Declared<>()`, `.InheritedFrom<>()` allow disambiguating them based on the type information.
- **Use visibility filters, if needed**.
  When members of the same type have different visibility, you can disambiguate them with filters like `.Private()`, `.Protected()`.
- **Use name-based filters as a last resort**.
  Before passing a name to `.Field<T>()` / `.Property<T>()` / `.Method<T>()`, confirm the SUT contains multiple members
  of the same type and rule out the alternatives described above.
- **Don't use name-based filters as defense against future SUT changes**.
  This is a common mistake based on the incorrect assumption that member names are stable and intended to make tests resilient
  to SUT adding new members. For example, if the SUT contains a single field of type `object`, it is accessed with a name-based
  filter in case another field of `object` is added to the SUT later. Even when it works as intended, this practice violates
  the Fail Fast principle and prevents re-evaluation of the SUT testability design expected at the time the new field is added.
- **Report unexpected `Inspector` errors to the user**.
  - Ask them to submit an issue to the `olegsych/inspector` repo on GitHub.
  - When implementing workarounds, add TODO comments with the explanation, package version and GitHub issue link.
  - Remove workarounds once a new version of `Inspector` is available with the fix.

## API Examples

```csharp
using Inspector;

// Fields
Bar value = obj.Field<Bar>().Value; // get by type
obj.Field<Bar>().Set(new Bar());    // set
obj.Field<Bar>("name").Value;       // use name to distinguish between multiple fields of the same type

// Properties
Baz value = obj.Property<Baz>().Value; // get by type
obj.Property<Baz>().Set(new Baz());    // set (works for read-only too)
obj.Property<Baz>("name").Value;       // use name to distinguish between multiple properties of the same type

// Methods
Action<Bar> action = obj.Method<Action<Bar>>();     // by delegate signature
Func<Baz, Bar> func = obj.Method<Func<Baz, Bar>>(); // func
obj.Method<Action<Baz>>("MethodName");              // use name to distinguish between multiple methods with same signatures

// Parameters
obj.Constructor().Parameter<string>();        // constructor param by type
obj.Constructor().Parameter<string>("name");  // use name to distinguish between multiple parameters with same type
obj.Method().Parameter<int>();                // method param by type

// Visibility filters
obj.Private().Field<Bar>();
obj.Protected().Field<Bar>();
obj.Internal().Field<Bar>();
obj.Public().Field<Bar>();

// Declaring type filters
obj.Declared().Field<Bar>();                  // declared in runtime type
obj.DeclaredBy<Base>().Field<Bar>();          // declared in specific type
obj.Inherited().Field<Bar>();                 // inherited from base
obj.InheritedFrom<Base>().Field<Bar>();       // inherited from specific type

// Static members (via Type)
typeof(Foo).Field<Bar>();
typeof(Foo).Property<Baz>();

// Object creation
Foo foo = Type<Foo>.New();                    // default constructor
Foo foo = Type<Foo>.New(arg1, arg2);          // parameterized constructor
Foo foo = Type<Foo>.Uninitialized();          // skip constructor
```

If examples above are insufficient, fetch more from the `olegsych/inspector` repo on GitHub for the following files:
- `examples/FieldAccessExample.cs`
- `examples/ObjectAccessExample.cs`
- `examples/ParameterExample.cs`
- `examples/PropertyAccessExample.cs`
- `examples/TypeAccessExample.cs`
