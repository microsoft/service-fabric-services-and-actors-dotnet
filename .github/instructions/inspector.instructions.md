---
applyTo: "**"
---
# Whitebox Testing
- Where possible, use inspector for reflection to simplify test setup and make tests more maintainable.
- Inspector is a whitebox testing tool that allows you to inspect and manipulate private members of classes during tests. https://github.com/olegsych/inspector
- Since this tool is not commonly known, we have examples here on how to use it
- Examples are taken from the official Inspector github - https://github.com/olegsych/inspector

#### Field access
```csharp
using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Inspector
{
    public class FieldAccessExample
    {
        class Foo
        {
            readonly Bar bar;
            public Foo(Bar bar) => this.bar = bar;
        }

        class Bar { }

        // Shared test fixture
        readonly Foo foo;
        readonly Bar bar = new Bar();
        public FieldAccessExample() => foo = new Foo(bar);

        public class FieldValue : FieldAccessExample
        {
            [Fact]
            public void GetWithMethod() {
                Bar? value = foo.Field<Bar>().Get();
                value.ShouldBeSameAs(bar);
            }

            [Fact]
            public void GetWithProperty() {
                Bar? value = foo.Field<Bar>().Value;
                value.ShouldBeSameAs(bar);
            }

            [Fact]
            public void GetWithImplicitConversionToFieldType() {
                Bar? value = foo.Field<Bar>();
                value.ShouldBeSameAs(bar);
            }
        }

        public class FilterByVisibility
        {
            class Foo
            {
                Bar? privateField;
                protected Bar? protectedField;
                internal Bar? internalField;
                public readonly Bar? publicField;
                protected internal Bar? protectedInternalField;
                private protected Bar? privateProtectedField;
            }

            readonly Foo foo = new Foo();

            [Fact]
            public void SelectPrivateField() {
                FieldInfo field = foo.Private().Field<Bar>();
                field.Name.ShouldBe("privateField");
            }

            [Fact]
            public void SelectProtectedField() {
                FieldInfo field = foo.Protected().Field<Bar>();
                field.Name.ShouldBe("protectedField");
            }

            [Fact]
            public void SelectInternalField() {
                FieldInfo field = foo.Internal().Field<Bar>();
                field.Name.ShouldBe("internalField");
            }

            [Fact]
            public void SelectProtectedInternalField() {
                FieldInfo field = foo.Protected().Internal().Field<Bar>();
                field.Name.ShouldBe("protectedInternalField");
            }

            [Fact]
            public void SelectPrivateProtectedField() {
                FieldInfo field = foo.Private().Protected().Field<Bar>();
                field.Name.ShouldBe("privateProtectedField");
            }

            [Fact]
            public void SelectPublicField() {
                FieldInfo field = foo.Public().Field<Bar>();
                field.Name.ShouldBe("publicField");
            }

            [Fact]
            public void ThrowDescriptiveExceptionWhenCompbinationOfVisibilityFiltersIsInvalid() {
                Should.Throw<InvalidOperationException>(() => foo.Public().Private().Field<Bar>());
                Should.Throw<InvalidOperationException>(() => foo.Public().Internal().Field<Bar>());
                Should.Throw<InvalidOperationException>(() => foo.Public().Protected().Field<Bar>());
            }
        }

        public class FilterByDeclaringType
        {
            class Foo
            {
                public Baz? fooField;
            }

            class Bar : Foo
            {
                public Baz? barField;
            }

            class Baz { }

            readonly Bar bar = new Bar();

            [Fact]
            public void SelectDeclaredField() {
                FieldInfo field = bar.Declared().Field<Baz>();
                field.DeclaringType.ShouldBe(typeof(Bar));
            }

            [Fact]
            public void SelectFieldDeclaredBySpecificType() {
                FieldInfo field = bar.DeclaredBy<Foo>().Field<Baz>();
                field.DeclaringType.ShouldBe(typeof(Foo));
            }

            [Fact]
            public void SelectInheritedField() {
                FieldInfo field = bar.Inherited().Field<Baz>();
                field.DeclaringType.ShouldBe(typeof(Foo));
            }

            [Fact]
            public void SelectFieldInheritedFromSpecificType() {
                FieldInfo field = bar.InheritedFrom<Foo>().Field<Baz>();
                field.DeclaringType.ShouldBe(typeof(Foo));
            }
        }

        public class FilterByName
        {
            class Foo
            {
                public Qux? field1;
                public Qux? field2;
            }

            class Bar : Foo
            {
                public new Qux? field1;
                public new Qux? field2;
            }

            class Baz : Bar
            {
                public new Qux? field1;
                public new Qux? field2;
            }

            class Qux { }

            readonly Foo foo = new Foo();
            readonly Bar bar = new Bar();
            readonly Baz baz = new Baz();

            [Fact]
            public void SelectFieldDeclaredWithSpecificName() {
                FieldInfo field = foo.Field<Qux>(nameof(Foo.field2));
                field.Name.ShouldBe(nameof(Foo.field2));
            }

            [Fact]
            public void SelectFieldWithSpecificNameAndVisibility() {
                FieldInfo field = foo.Public().Field<Qux>(nameof(Foo.field2));
                field.Name.ShouldBe(nameof(Foo.field2));
            }

            [Fact]
            public void SelectInheritedFieldWithSpecificName() {
                FieldInfo field = bar.InheritedFrom<Foo>().Field<Qux>(nameof(Foo.field2));
                field.DeclaringType.ShouldBe(typeof(Foo));
                field.Name.ShouldBe(nameof(Foo.field2));
            }
        }
    }
}
```

#### Object access
```csharp
using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Inspector
{
    public class ObjectAccessExample
    {
        public class AccessibleTypes : ObjectAccessExample
        {
            class Bar { }
            class Baz { }

            public class Create : AccessibleTypes
            {
                class Foo
                {
                    Bar barField;
                    Baz bazField;

                    Foo() : this(new Bar(), new Baz()) { }

                    Foo(Bar bar, Baz baz) {
                        barField = bar;
                        bazField = baz;
                    }
                }

                [Fact]
                public void NewInstanceWithDefaultConstructor() {
                    Foo foo = Type<Foo>.New();

                    foo.Field<Bar>().Value.ShouldNotBeNull();
                    foo.Field<Baz>().Value.ShouldNotBeNull();
                }

                [Fact]
                public void NewInstanceWithGivenConstructorParameters() {
                    var bar = new Bar();
                    var baz = new Baz();

                    Foo foo = Type<Foo>.New(bar, baz);

                    foo.Field<Bar>().Value.ShouldBeSameAs(bar);
                    foo.Field<Baz>().Value.ShouldBeSameAs(baz);
                }

                [Fact]
                public void UninitializedInstance() {
                    Foo foo = Type<Foo>.Uninitialized();

                    foo.Field<Bar>().Value.ShouldBeNull();
                    foo.Field<Baz>().Value.ShouldBeNull();
                }
            }

            public class AccessFields : ObjectAccessExample
            {
                class Foo
                {
                    Bar bar = new Bar();
                    Baz baz1 = new Baz();
                    Baz baz2 = new Baz();
                }

                readonly Foo foo = new Foo();

                [Fact]
                public void ByType() {
                    Field<Bar> field = foo.Field<Bar>();
                }

                [Fact]
                public void ByTypeAndName() {
                    Field<Baz> field = foo.Field<Baz>("baz1");
                }
            }

            public class AccessActions : AccessibleTypes
            {
                class Foo
                {
                    void BarAction(Bar bar) { }

                    void BazAction1(Baz baz) { }
                    void BazAction2(Baz baz) { }

                    void OutAction1(out Bar bar) => throw new NotImplementedException();
                    void OutAction2(out Bar bar) => throw new NotImplementedException();

                    Bar BarFunc(Baz baz) => throw new NotImplementedException();
                }

                readonly Foo foo = new Foo();

                [Fact]
                public void SimpleActionWithUniqueParameters() {
                    Action<Bar> action = foo.Method<Action<Bar>>();
                }

                [Fact]
                public void SimpleActionWithUniqueName() {
                    Action<Baz> action = foo.Method<Action<Baz>>("BazAction1");
                }

                [Fact]
                public void SimpleFuncWithUniqueParameters() {
                    Func<Baz, Bar> fun = foo.Method<Func<Baz, Bar>>();
                }

                delegate void OutAction(out Bar bar);

                [Fact]
                public void MethodWithAdvancedParameters() {
                    OutAction action = foo.Method<OutAction>("OutAction1");
                }
            }
        }

        public class InaccessibleTypes : ObjectAccessExample
        {
            class Inaccessible
            {
                class Foo
                {
                    Bar barField;
                    Baz bazField;

                    Foo() : this(new Bar(), new Baz()) { }

                    Foo(Bar bar, Baz baz) {
                        barField = bar;
                        bazField = baz;
                    }
                }

                class Bar { }
                class Baz { }
            }

            public class AccessFields : InaccessibleTypes
            {
                readonly object foo = Activator.CreateInstance(typeOfFoo, true)!;

                [Fact]
                public void ByType() {
                    Field field = foo.Field(typeOfBar);

                    field.Value.ShouldBeOfType(typeOfBar);
                }

                [Fact]
                public void ByName() {
                    Field field = foo.Field("barField");

                    field.Value.ShouldBeOfType(typeOfBar);
                }

                [Fact]
                public void ByTypeAndName() {
                    Field field = foo.Field(typeOfBar, "barField");

                    field.Value.ShouldBeOfType(typeOfBar);
                }
            }
        }
    }
}
```

#### Parameters
```csharp
using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Inspector
{
    public class ParameterExample
    {
        class TestType
        {
            class Baz { }

            TestType(int foo, string bar, Baz baz) { }

            void TestMethod(int foo, string bar, Baz baz) { }
        }
        public class ConstructorParameter: ParameterExample
        {
            [Fact]
            public void CanBeAccessedByRuntimeType() {
                ParameterInfo parameter = instance.Constructor().Parameter(runtimeType);
                parameter.Name.ShouldBe("baz");
            }

            [Fact]
            public void CanBeAccessedByCompileTimeType() {
                ParameterInfo parameter = instance.Constructor().Parameter<string>();
                parameter.Name.ShouldBe("bar");
            }

            [Fact]
            public void CanBeAccessedByName() {
                ParameterInfo parameter = instance.Constructor().Parameter("bar");
                parameter.ParameterType.ShouldBe(typeof(string));
            }
        }

        public class ConstructorInfoParameter: ParameterExample
        {
            [Fact]
            public void CanBeAccessedByRuntimeType() {
                ConstructorInfo constructor = instance.Constructor();
                ParameterInfo parameter = constructor.Parameter(runtimeType);
                parameter.Name.ShouldBe("baz");
            }

            [Fact]
            public void CanBeAccessedByCompileTimeType() {
                ConstructorInfo constructor = instance.Constructor();
                ParameterInfo parameter = constructor.Parameter<string>();
                parameter.Name.ShouldBe("bar");
            }

            [Fact]
            public void CanBeAccessedByName() {
                ConstructorInfo constructor = instance.Constructor();
                ParameterInfo parameter = constructor.Parameter("bar");
                parameter.ParameterType.ShouldBe(typeof(string));
            }
        }

        public class MethodParameter: ParameterExample
        {
            [Fact]
            public void CanBeAccessedByRuntimeType() {
                ParameterInfo parameter = instance.Method().Parameter(runtimeType);
                parameter.Name.ShouldBe("baz");
            }

            [Fact]
            public void CanBeAccessedByCompileTimeType() {
                ParameterInfo parameter = instance.Method().Parameter<string>();
                parameter.Name.ShouldBe("bar");
            }

            [Fact]
            public void CanBeAccessedByName() {
                ParameterInfo parameter = instance.Method().Parameter("bar");
                parameter.ParameterType.ShouldBe(typeof(string));
            }
        }

        public class MethodInfoParameter: ParameterExample
        {
            [Fact]
            public void CanBeAccessedByRuntimeType() {
                MethodInfo method = instance.Method();
                ParameterInfo parameter = method.Parameter(runtimeType);
                parameter.Name.ShouldBe("baz");
            }

            [Fact]
            public void CanBeAccessedByCompileTimeType() {
                MethodInfo method = instance.Method();
                ParameterInfo parameter = method.Parameter<string>();
                parameter.Name.ShouldBe("bar");
            }

            [Fact]
            public void CanBeAccessedByName() {
                MethodInfo method = instance.Method();
                ParameterInfo parameter = method.Parameter("bar");
                parameter.ParameterType.ShouldBe(typeof(string));
            }
        }
    }
}
```

#### Property access
```csharp
using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Inspector
{
    public class PropertyAccessExample
    {
        class Foo
        {
            Bar? Bar { get; set; }
            public Foo(Bar? bar) => Bar = bar;
        }

        class Bar { }

        [Fact]
        public void GetValueExplicitly() {
            var bar = new Bar();
            var foo = new Foo(bar);

            Bar? value = foo.Property<Bar>().Get();

            value.ShouldBeSameAs(bar);
        }

        [Fact]
        public void GetValueImplicitly() {
            var bar = new Bar();
            var foo = new Foo(bar);

            Bar? value = foo.Property<Bar>();

            value.ShouldBeSameAs(bar);
        }

        [Fact]
        public void SetValueExplicitly() {
            var bar = new Bar();
            var foo = new Foo(null);

            foo.Property<Bar>().Set(bar);

            foo.Property<Bar>().Get().ShouldBe(bar);
        }

        [Fact]
        public void GetInfoExplicitly() {
            var bar = new Bar();
            var foo = new Foo(bar);

            PropertyInfo info = foo.Property<Bar>().Info;

            info.ShouldBe(typeof(Foo).GetProperty("Bar", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void GetInfoImplicitly() {
            var bar = new Bar();
            var foo = new Foo(bar);

            PropertyInfo info = foo.Property<Bar>();

            info.ShouldBe(typeof(Foo).GetProperty("Bar", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        public class ReadOnlyPropertyBackedByField
        {
            class Foo { }

            class Bar
            {
                Foo? Foo { get; }
                public Bar(Foo? foo) => Foo = foo;
            }

            [Fact]
            public void CanBeSet() {
                var foo = new Foo();
                var bar = new Bar(null);

                bar.Property<Foo>().Set(foo);

                bar.Property<Foo>().Get().ShouldBe(foo);
            }
        }

        public class ReadOnlyPropertyNotBackedByField
        {
            class Foo
            {
                public Bar BarProperty => new Bar();
            }

            class Bar { }

            [Fact]
            public void CannotBeSetAndWillThrowDescriptiveException() {
                var foo = new Foo();

                var thrown = Should.Throw<InvalidOperationException>(() => foo.Field<Bar>().Set(new Bar()));

                thrown.Message.ShouldContain(nameof(Bar));
                thrown.Message.ShouldContain(nameof(Foo.BarProperty));
            }
        }
    }
}
```

#### Type access
```csharp
using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Inspector
{
    public class TypeAccessExample
    {
        static class Foo
        {
            public static Bar? barField;

            public static Baz? BazProperty { get; set; }

            public static event EventHandler<Bar>? BarEvent;

            public static Baz? BarFunc(Bar _) => default;

            public static void BarAction(Bar _) { }

            static Foo() => barField = default;
        }

        class Bar { }

        class Baz { }

        public class Field : TypeAccessExample
        {
            [Fact]
            public void GetByType() {
                Field<Bar> field = typeof(Foo).Field<Bar>();
                field.Info.ShouldBe(typeof(Foo).GetRuntimeField(nameof(Foo.barField)));
            }

            [Fact]
            public void GetByTypeAndName() {
                Field<Bar> field = typeof(Foo).Field<Bar>(nameof(Foo.barField));
                field.Info.ShouldBe(typeof(Foo).GetRuntimeField(nameof(Foo.barField)));
            }
        }

        public class Property : TypeAccessExample
        {
            [Fact]
            public void GetByType() {
                Property<Baz> property = typeof(Foo).Property<Baz>();
                property.Info.ShouldBe(typeof(Foo).GetRuntimeProperty(nameof(Foo.BazProperty)));
            }

            [Fact]
            public void GetByTypeAndName() {
                Property<Baz> property = typeof(Foo).Property<Baz>(nameof(Foo.BazProperty));
                property.Info.ShouldBe(typeof(Foo).GetRuntimeProperty(nameof(Foo.BazProperty)));
            }
        }
    }
}
```