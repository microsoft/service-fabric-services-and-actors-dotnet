#### ⚠️ `CapturedRequest` field visibility overstates effective visibility

> *Reported by gpt (⚠️) and opus (💡) — elevated per multi-model agreement.*

[test/AspNetCore/ServiceFabricMiddlewareTest.cs](test/AspNetCore/ServiceFabricMiddlewareTest.cs#L163-L167):

``````csharp
sealed class CapturedRequest
{
    internal PathString Path;
    internal PathString PathBase;
}
``````

`CapturedRequest` is a nested type with no visibility modifier, so it is private to `Invoke`; the effective visibility of its `internal` fields is `private`. Per [csharp.instructions.md](.github/instructions/csharp.instructions.md): *"Make member visibility specifiers represent the actual member visibility."* The enclosing `Invoke` class still has access after the change.

**Action**: Drop `internal` from both fields:

``````csharp
PathString Path;
PathString PathBase;
``````

**Coder rejection note (recurring)**: Removing `internal` makes the fields effectively `private`, and a containing type cannot access private members of its nested types in C# (CS0122 at every call site). `internal` is the minimum that compiles. This finding has been raised in successive review cycles despite verified compile evidence — it should not be re-raised without resolving the underlying disagreement with the language rules.
