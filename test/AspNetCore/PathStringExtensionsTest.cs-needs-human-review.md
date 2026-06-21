#### ⚠️ Invoke the extension method through extension-method syntax

*Reported by `opus` as 💡; elevated per cross-check `Agree` from `gpt` and `gemini`.*

Every test calls `PathStringExtensions.StartsWithSegments(pathString, other, out _, out _)` (e.g. [line 33](test/AspNetCore/PathStringExtensionsTest.cs#L33)). The SUT is declared with `this PathString pathString` and is consumed as `pathString.StartsWithSegments(other, …)` at [src/AspNetCore/ServiceFabricMiddleware.cs](src/AspNetCore/ServiceFabricMiddleware.cs#L77). Extension-call form is shorter, idiomatic, and matches consumer usage — consistent with [.github/instructions/coding.instructions.md](.github/instructions/coding.instructions.md) "Make the code as concise as possible". No explicit rule mandates it (style rather than correctness), but all three models agreed.

**Coder declined**: `PathString` has a built-in instance method `StartsWithSegments(PathString other, out PathString matched, out PathString remaining)` with the same signature. C# resolves instance methods before extension methods, so `pathString.StartsWithSegments(...)` would call the BCL method instead of `PathStringExtensions`. The same applies to the consumer in `ServiceFabricMiddleware.cs` — `PathStringExtensions` may be dead code (its XML comment refers to ASP.NET Core 1.0.0).
