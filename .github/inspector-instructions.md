# Summary

This file contains common examples for Inspector API for whitebox testing. These examples are taken from the official Inspector github - https://github.com/olegsych/inspector

## Field access
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

            [Fact]
            public void SetWithMethod() {
                var baz = new Bar();
                foo.Field<Bar>().Set(baz);
                foo.Field<Bar>().Get().ShouldBe(baz);
            }

            [Fact]
            public void SetWithValue() {
                var baz = new Bar();
                foo.Field<Bar>().Value = baz;
                foo.Field<Bar>().Value.ShouldBe(baz);
            }

            //[Fact]
            //public void CompareImplicitly() {
            //    (foo.Field<Bar>() == bar).ShouldBeTrue();
            //    (foo.Field<Bar>() != bar).ShouldBeFalse();
            //    (bar == foo.Field<Bar>()).ShouldBeTrue();
            //    (bar != foo.Field<Bar>()).ShouldBeFalse();
            //}
        }

        public class FieldInfoScenario : FieldAccessExample
        {
            [Fact]
            public void GetWithProperty() {
                FieldInfo info = foo.Field<Bar>().Info;
                info.ShouldBe(typeof(Foo).GetField("bar", BindingFlags.Instance | BindingFlags.NonPublic));
            }

            [Fact]
            public void GetWithImplicitConversionToFieldInfo() {
                FieldInfo info = foo.Field<Bar>();
                info.ShouldBe(typeof(Foo).GetField("bar", BindingFlags.Instance | BindingFlags.NonPublic));
            }
        }

        public class Operators
        {
            class Foo
            {
                int bar;
                public Foo(int bar) => this.bar = bar;
            }

            readonly Foo foo = new Foo(42);

            [Fact]
            public void UseFieldValueWithBinaryOperators() {
                (foo.Field<int>() + 1).ShouldBe(43);
            }

            [Fact]
            public void ChangeFieldValueWithAssignmentOperator() {
                foo.Field<int>().Value += 1;
                foo.Field<int>().Value.ShouldBe(43);
            }
        }

        public class FilterByVisibility
        {
            class Foo
            {
#pragma warning disable 169
                Bar? privateField;
#pragma warning disable 169

#pragma warning disable 649
                protected Bar? protectedField;
                internal Bar? internalField;
                public readonly Bar? publicField;
                protected internal Bar? protectedInternalField;
                private protected Bar? privateProtectedField;
#pragma warning restore 649
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
#pragma warning disable 649

            class Foo
            {
                public Baz? fooField;
            }

            class Bar : Foo
            {
                public Baz? barField;
            }

#pragma warning restore 649

            class Baz { }

            readonly Bar bar = new Bar();

            [Fact]
            public void ThrowDescriptiveExceptionWhenMoreThanOneFieldOfGivenTypeExists() {
                var thrown = Should.Throw<InvalidOperationException>(() => bar.Field<Baz>());
                thrown.Message.ShouldContain(typeof(Baz).FullName!);
                thrown.Message.ShouldContain(typeof(Foo).FullName!);
                thrown.Message.ShouldContain(typeof(Bar).FullName!);
            }

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
#pragma warning disable 649

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

#pragma warning restore 649

            class Qux { }

            readonly Foo foo = new Foo();
            readonly Bar bar = new Bar();
            readonly Baz baz = new Baz();

            [Fact]
            public void ThrowDescriptiveExceptionWhenMoreThanOneFieldOfGivenTypeExistsInDeclaringType() {
                var thrown = Should.Throw<InvalidOperationException>(() => foo.Field<Qux>());
                thrown.Message.ShouldContain(typeof(Qux).FullName!);
                thrown.Message.ShouldContain(typeof(Foo).FullName!);
                thrown.Message.ShouldContain(nameof(Foo.field1));
                thrown.Message.ShouldContain(nameof(Foo.field2));
            }

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

            [Fact]
            public void SelectFieldWithSpecificNameAndDeclaringType() {
                FieldInfo field = baz.DeclaredBy<Bar>().Field<Qux>(nameof(Bar.field2));
                field.DeclaringType.ShouldBe(typeof(Bar));
                field.Name.ShouldBe(nameof(Bar.field2));
            }
        }
    }
}
```

## Object access
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

            static readonly Type typeOfFoo = typeof(Inaccessible).GetNestedType("Foo", BindingFlags.NonPublic)!;
            static readonly Type typeOfBar = typeof(Inaccessible).GetNestedType("Bar", BindingFlags.NonPublic)!;
            static readonly Type typeOfBaz = typeof(Inaccessible).GetNestedType("Baz", BindingFlags.NonPublic)!;

            public class Create : InaccessibleTypes
            {
                [Fact]
                public void NewInstanceWithDefaultConstructor() {
                    object foo = typeOfFoo.New();

                    foo.Field(typeOfBar).Value.ShouldNotBeNull();
                    foo.Field(typeOfBaz).Value.ShouldNotBeNull();
                }

                [Fact]
                public void NewInstanceWithGivenConstructorParameters() {
                    object bar = typeOfBar.New();
                    object baz = typeOfBaz.New();

                    object foo = typeOfFoo.New(bar, baz);

                    foo.Field(typeOfBar).Value.ShouldBeSameAs(bar);
                    foo.Field(typeOfBaz).Value.ShouldBeSameAs(baz);
                }

                [Fact]
                public void UninitializedInstance() {
                    object foo = typeOfFoo.Uninitialized();

                    foo.Field(typeOfBar).Value.ShouldBeNull();
                    foo.Field(typeOfBaz).Value.ShouldBeNull();
                }
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

## Parameters
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

        readonly TestType instance = Type<TestType>.Uninitialized();
        readonly Type runtimeType = typeof(TestType).GetNestedType("Baz", BindingFlags.NonPublic)!;

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

## Property access
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

        readonly TestType instance = Type<TestType>.Uninitialized();
        readonly Type runtimeType = typeof(TestType).GetNestedType("Baz", BindingFlags.NonPublic)!;

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

## Type access
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
#pragma warning disable 414
            public static Bar? barField;
#pragma warning restore 414

            public static Baz? BazProperty { get; set; }

#pragma warning disable 67
            public static event EventHandler<Bar>? BarEvent;
#pragma warning restore 67

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

## Complex reflection example
```csharp
using System;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;

namespace Inspector
{
    public class ReflectionExperiment
    {
        class Foo
        {
            public int bar;
            public Foo(int bar) => this.bar = bar;

            public static int staticBar;

            static Foo() => staticBar = 0;
        }

        public class ConstructorInfoInvoke : ReflectionExperiment
        {
            [Fact]
            public void ConstructorCreatesNewInstance() {
                ConstructorInfo constructor = typeof(Foo).GetConstructor(new[] { typeof(int) })!;
                Assert.False(constructor.IsStatic);

                object foo = constructor.Invoke(new object[] { 42 });

                var typedFoo = (Foo)foo;
                Assert.Equal(42, typedFoo.bar);
            }

            [Fact]
            public void ConstructorReinitializesExistingInstance() {
                ConstructorInfo constructor = typeof(Foo).GetConstructor(new[] { typeof(int) })!;
                Assert.False(constructor.IsStatic);
                var foo = new Foo(0);

                object? result = constructor.Invoke(foo, new object[] { 42 });

                Assert.Null(result);
                Assert.Equal(42, foo.bar);
            }

            [Fact]
            public void StaticConstructorDoesNotReinitializesType() {
                ConstructorInfo constructor = typeof(Foo).TypeInitializer!;
                Assert.True(constructor.IsStatic);
                Foo.staticBar = 42;

                object? act = constructor.Invoke(null, null);

                // This behavior changed in .NET 5, reinitialization stopped working.
                Assert.Equal(42, Foo.staticBar);
            }
        }

        // A very low-level, unsafe way to create a delegate bound to constructor.
        // Requires separate logic for verifying that delegate and constructor have matching signatures.
        public class CreateConstructorDelegateUsingTypedDelegateConstructor : ReflectionExperiment
        {
            [Fact]
            public void CreateOpenDelegate() {
                ConstructorInfo actionInfo = typeof(Action<Foo, int>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) })!;
                Assert.NotNull(actionInfo);

                ConstructorInfo fooInfo = typeof(Foo).GetConstructor(new Type[] { typeof(int) })!;
                Assert.NotNull(fooInfo);

                var ctor = (Action<Foo, int>)actionInfo.Invoke(new object?[] { null, fooInfo.MethodHandle.GetFunctionPointer() });
                Assert.NotNull(ctor);

                var foo = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo));
                ctor.Invoke(foo, 42);

                Assert.Equal(42, foo.bar);
            }

            [Fact]
            public void CreateClosedDelegate() {
                ConstructorInfo actionInfo = typeof(Action<int>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) })!;
                Assert.NotNull(actionInfo);

                ConstructorInfo fooInfo = typeof(Foo).GetConstructor(new Type[] { typeof(int) })!;
                Assert.NotNull(fooInfo);

                var foo = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo));

                var ctor = (Action<int>)actionInfo.Invoke(new object[] { foo, fooInfo.MethodHandle.GetFunctionPointer() });
                Assert.NotNull(ctor);

                ctor.Invoke(42);

                Assert.Equal(42, foo.bar);
            }
        }

        public class Signature : ReflectionExperiment
        {
            [Fact(Skip = "Broken")]
            public void CompareSig() {
                Type signatureType = Type.GetType("System.Signature")!;
                Assert.NotNull(signatureType);

                Type iRuntimeMethodInfoType = Type.GetType("System.IRuntimeMethodInfo")!;
                Assert.NotNull(iRuntimeMethodInfoType);

                Type runtimeTypeType = Type.GetType("System.RuntimeType")!;
                Assert.NotNull(runtimeTypeType);

                ConstructorInfo signatureCtor = signatureType.GetConstructor(new[] { iRuntimeMethodInfoType, runtimeTypeType })!;
                Assert.NotNull(signatureCtor);

                ConstructorInfo fooCtor = typeof(Foo).GetConstructor(new[] { typeof(int) })!;
                Assert.NotNull(fooCtor);

                object fooCtorSignature = signatureCtor.Invoke(new object[] { fooCtor, typeof(Foo) });
                Assert.NotNull(fooCtorSignature);

                MethodInfo invokeMethod = typeof(Action<int>).GetMethod("Invoke")!;
                Assert.NotNull(invokeMethod);

                object invokeMethodSignature = signatureCtor.Invoke(new object[] { invokeMethod, typeof(Action<int>) });
                Assert.NotNull(invokeMethodSignature);

                MethodInfo compareSigMethod = signatureType.GetMethod("CompareSig", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
                Assert.NotNull(compareSigMethod);

                object result = compareSigMethod.Invoke(null, new object[] { fooCtorSignature, invokeMethodSignature })!;
                bool equal = Assert.IsType<bool>(result);
                Assert.True(equal);
            }
        }

        // A high-level way to create a delegate bound to a constructor.
        // Uses the internal Delegate.BindToMethodInfo method, which ensures that delegate and constructor have matching signatures.
        public class BindDelegateToConstructor : ReflectionExperiment
        {
            [Fact]
            public void BindOpenDelegate() {
                MethodInfo internalAlloc = typeof(Delegate).GetMethod("InternalAlloc", BindingFlags.Static | BindingFlags.NonPublic)!;
                Assert.NotNull(internalAlloc);

                MethodInfo bindToMethodInfo = typeof(Delegate).GetMethod("BindToMethodInfo", BindingFlags.Instance | BindingFlags.NonPublic)!;
                Assert.NotNull(bindToMethodInfo);

                var d = (Delegate)internalAlloc.Invoke(null, new object[] { typeof(Action<Foo, int>) })!;
                Assert.NotNull(d);

                object? firstArgument = null;
                object rtMethod = typeof(Foo).GetConstructor(new[] { typeof(int) })!; // IRuntimeMethodInfo
                object flags = 0x84; // DelegateBindingFlags
                var bound = (bool)bindToMethodInfo.Invoke(d, new object?[] { firstArgument, rtMethod, typeof(Foo), flags})!;
                Assert.True(bound);

                var foo = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo));
                var constructor = (Action<Foo, int>)d;
                constructor.Invoke(foo, 42);
                Assert.Equal(42, foo.bar);
            }

            [Fact]
            public void BindClosedDelegate() {
                MethodInfo internalAlloc = typeof(Delegate).GetMethod("InternalAlloc", BindingFlags.Static | BindingFlags.NonPublic)!;
                Assert.NotNull(internalAlloc);

                MethodInfo bindToMethodInfo = typeof(Delegate).GetMethod("BindToMethodInfo", BindingFlags.Instance | BindingFlags.NonPublic)!;
                Assert.NotNull(bindToMethodInfo);

                var d = (Delegate)internalAlloc.Invoke(null, new object[] { typeof(Action<int>) })!;
                Assert.NotNull(d);

                var foo = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo));

                object firstArgument = foo;
                object rtMethod = typeof(Foo).GetConstructor(new[] { typeof(int) })!; // IRuntimeMethodInfo
                object flags = 0x88; // DelegateBindingFlags
                var bound = (bool)bindToMethodInfo.Invoke(d, new object[] { firstArgument, rtMethod, typeof(Foo), flags })!;
                Assert.True(bound);

                var constructor = (Action<int>)d;
                constructor.Invoke(42);
                Assert.Equal(42, foo.bar);
            }
        }
    }
}
```