---
applyTo: "test/**"
---

# Whitebox Testing
- Where possible, use inspector for reflection to simplify test setup and make tests more maintainable.
- [Inspector](https://github.com/olegsych/inspector) is a whitebox testing tool that allows you to inspect and
  manipulate private members of classes during tests.
- Before writing inspector code, fetch the examples from the official repo using `mcp_github_github_get_file_contents`
  with `owner: "olegsych"`, `repo: "inspector"` and `ref: "refs/heads/master"` for the following files:
  - `examples/FieldAccessExample.cs`
  - `examples/ObjectAccessExample.cs`
  - `examples/ParameterExample.cs`
  - `examples/PropertyAccessExample.cs`
  - `examples/TypeAccessExample.cs`
- Make sure the test project has a `<ProjectReference Include="Inspector"/>`

## API Cheat-Sheet
```csharp
// Fields
Bar value = obj.Field<Bar>().Value;           // get by type
obj.Field<Bar>().Set(new Bar());              // set
obj.Field<Bar>("name").Value;                 // get by type and name

// Properties
Baz value = obj.Property<Baz>().Value;        // get by type
obj.Property<Baz>().Set(new Baz());           // set (works for read-only too)
obj.Property<Baz>("name").Value;              // get by type and name

// Methods
Action<Bar> action = obj.Method<Action<Bar>>();          // by delegate signature
Func<Baz, Bar> func = obj.Method<Func<Baz, Bar>>();     // func
obj.Method<Action<Baz>>("MethodName");                   // by name

// Parameters
obj.Constructor().Parameter<string>();        // constructor param by type
obj.Constructor().Parameter("name");          // constructor param by name
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
