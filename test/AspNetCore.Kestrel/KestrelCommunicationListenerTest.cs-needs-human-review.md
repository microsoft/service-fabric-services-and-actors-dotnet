### ⚠️ Should Fix — Inspector workaround TODO references an unfiled issue
Reported by: `gemini`, `opus`.

[test/AspNetCore/KestrelCommunicationListenerTest.cs#L42-L44](test/AspNetCore/KestrelCommunicationListenerTest.cs#L42-L44) and [test/AspNetCore/HttpSysCommunicationListenerTest.cs#L39-L41](test/AspNetCore/HttpSysCommunicationListenerTest.cs#L39-L41) both contain:

> // TODO: Inspector v0.9.0 sut.Constructor<TSig>() binds multiple overloads when delegate-typed parameters
> // only differ in generic arguments (relaxed signature matching). Track via olegsych/inspector once filed.

[.github/instructions/inspector.instructions.md](.github/instructions/inspector.instructions.md) requires workaround TODOs to link to a filed GitHub issue, and instructs the agent to ask the user to submit one to `olegsych/inspector`. The agent cannot file the upstream issue itself.

**Action required:**
1. File an issue on https://github.com/olegsych/inspector describing the behavior: in Inspector v0.9.0, `sut.Constructor<TSig>()` binds multiple overloads when delegate-typed parameters only differ in generic arguments (relaxed signature matching).
2. Replace `once filed` in both TODO comments with the actual issue URL.
3. Delete this section.

---

#### ❓ Needs Human Review — Style/analyzer diagnostics on the test file

Reported by **gpt**; cross-check **gemini Agree**, **opus Disagree** (Release build is clean with `TreatWarningsAsErrors=true` reporting 0 warnings; header borders match sibling files; `GetConstructor(...)!` suppressions appear required). **gpt re-evaluated: Insist** with narrowed scope.

- [test/AspNetCore/KestrelCommunicationListenerTest.cs#L1](test/AspNetCore/KestrelCommunicationListenerTest.cs#L1), [test/AspNetCore/KestrelCommunicationListenerTest.cs#L28](test/AspNetCore/KestrelCommunicationListenerTest.cs#L28), [test/AspNetCore/KestrelCommunicationListenerTest.cs#L44](test/AspNetCore/KestrelCommunicationListenerTest.cs#L44), [test/AspNetCore/KestrelCommunicationListenerTest.cs#L65](test/AspNetCore/KestrelCommunicationListenerTest.cs#L65), [test/AspNetCore/KestrelCommunicationListenerTest.cs#L237](test/AspNetCore/KestrelCommunicationListenerTest.cs#L237)

**gpt evidence (after re-evaluation):** Release `dotnet build` is clean across all TFMs; VS Code Problems is clean. However, `dotnet format ... style --verify-no-changes --include test/AspNetCore/KestrelCommunicationListenerTest.cs` exits 1 reporting: IDE0073 header mismatch (line 1), IDE0052 unread base `sut` (line 28), IDE0300 collection initialization simplification at the `GetConstructor(new[] { ... })` calls (e.g. line 44), IDE0370 unnecessary `!` suppression (e.g. line 65), IDE0200 lambda can be removed (line 237).

**gpt caveats:** (a) The unread `sut` should likely be intentionally suppressed, not removed, since the repo's test guidance says to create a readonly `sut` field at the top of the base test class even when not applicable to every test. (b) The header mismatch is relative to `.editorconfig`, but many sibling files use the same legacy border banner, suggesting a repo-wide convention/tooling mismatch rather than a file-specific cleanup item.

**opus evidence (still on record):** Sibling files like [test/AspNetCore/HttpSysCommunicationListenerTest.cs#L1](test/AspNetCore/HttpSysCommunicationListenerTest.cs#L1) and [test/AspNetCore/GenericHostCommunicationListenerTest.cs#L1](test/AspNetCore/GenericHostCommunicationListenerTest.cs#L1) use the same header style without triggering IDE0073 in build; the suppressions are required by the BCL's `ConstructorInfo?` return type.

**Why human review:** The diagnostics are real under `dotnet format style`, but they don't fail the build, and several reflect repo-wide convention vs. `.editorconfig` drift rather than file-specific issues. Decide which to clean up (likely IDE0300/IDE0200) vs. intentionally suppress (IDE0052 for `sut`) vs. defer as a repo-wide concern (IDE0073).

---

### ❓ Needs Human Review — `GetListenerUrl*` tests placed inside `Constructor_*` classes

Reported by `gemini` (⚠️ Should Fix) and `opus` (❓ Needs Human Review).

Four `GetListenerUrlReturnsDefaultHttpUrl*` tests live inside the four `Constructor_*` nested classes rather than under the nested `GetListenerUrl` class:
- [test/AspNetCore/KestrelCommunicationListenerTest.cs](test/AspNetCore/KestrelCommunicationListenerTest.cs#L66-L73)
- [test/AspNetCore/KestrelCommunicationListenerTest.cs](test/AspNetCore/KestrelCommunicationListenerTest.cs#L95-L102)
- [test/AspNetCore/KestrelCommunicationListenerTest.cs](test/AspNetCore/KestrelCommunicationListenerTest.cs#L143-L151)
- [test/AspNetCore/KestrelCommunicationListenerTest.cs](test/AspNetCore/KestrelCommunicationListenerTest.cs#L192-L200)

**gemini:** Per `test.instructions.md` — *"Don't create a test class for the SUT constructor when it would duplicate tests for other members"*. The four duplicated tests should be removed and replaced with a single `GetListenerUrl.ReturnsDefaultHttpUrlWhenEndpointNameIsNull` test.

**opus:** Defensible — mirrors the SUT's duplicated `endpointName?.Length == 0 / this.endpointName = endpointName` block across the four constructor overloads ([src/AspNetCore.Kestrel/KestrelCommunicationListener.cs](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L58-L86)) and pins per-overload regressions. But it conflicts with the rule that nested test classes are named after the target member, and a reader scanning `GetListenerUrl` for null-endpoint coverage would miss it. Alternative: parameterize a single `GetListenerUrl.ReturnsDefaultHttpUrlWhenEndpointNameIsNull` test via `[Theory]`/`[MemberData]` with a `Func<ServiceContext, AspNetCoreCommunicationListener>` factory for each overload — keeps per-overload coverage while placing the test under its target member.

**Decision needed:** keep the per-overload pinning as-is, or refactor into a parameterized test under `GetListenerUrl`.

**Iterator note:** This is part of a multi-round oscillation across reviewers on where null-endpoint `GetListenerUrl` coverage should live. Previous rounds pulled this coverage out of `GetListenerUrl`, into separate sibling classes, then collapsed it back into a single `GetListenerUrl` test, then required per-overload coverage (the current placement). Human judgment is requested to break the cycle.

---

## ⚠️ Should Fix — Missing coverage for the non-null `IHost` constructor's `endpointName` assignment

**Reported by GPT, agreed by Opus, Gemini abstained.**

The `GetListenerUrl` fixture at [test/AspNetCore/KestrelCommunicationListenerTest.cs#L207](test/AspNetCore/KestrelCommunicationListenerTest.cs#L207) constructs the SUT through the base class's `build` delegate field, which is typed `Func<string, AspNetCoreCommunicationListener, IWebHost>` at [test/AspNetCore/KestrelCommunicationListenerTest.cs#L34](test/AspNetCore/KestrelCommunicationListenerTest.cs#L34). The endpoint-backed theories at [L220](test/AspNetCore/KestrelCommunicationListenerTest.cs#L220) and [L240](test/AspNetCore/KestrelCommunicationListenerTest.cs#L240) and the not-in-manifest test at [L281](test/AspNetCore/KestrelCommunicationListenerTest.cs#L281) all flow exclusively through the `IWebHost` 3-arg ctor.

The `IHost` 3-arg ctor's `this.endpointName = endpointName` assignment at [src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L78-L86](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L78-L86) is only reached via `GetListenerUrlReturnsDefaultHttpUrlWhenEndpointNameIsNull` at [L143](test/AspNetCore/KestrelCommunicationListenerTest.cs#L143), which passes `endpointName: null`. If that assignment were removed, no test would observe an endpoint-name regression isolated to the `IHost` overload.

**Recommendation.** Add an `IHost` variant of the endpoint-backed `GetListenerUrl` tests, or parameterize the `GetListenerUrl` fixture over both build-delegate shapes.

**Iterator note (oscillation):** An earlier round of this iteration (round ~7) explicitly directed us to COLLAPSE the `WithIHost`/`WithIWebHost` matrix in `GetListenerUrl`, with rationale "GetListenerUrl does not reference the build delegate at all; the IHost vs IWebHost branch is decided in AspNetCoreCommunicationListener's constructor and never reaches GetListenerUrl." We complied (and also applied the same collapse to HttpSysCommunicationListenerTest.cs for consistency). This round flips that position. Human decides which is correct; whichever way, please align both peer files.

---

## ⚠️ Should Fix — Active empty-`endpointName` tests pin the missing-`ParamName` bug

**Reported by GPT, agreed by Opus, Gemini abstained.**

The active tests at [L127](test/AspNetCore/KestrelCommunicationListenerTest.cs#L127) and [L174](test/AspNetCore/KestrelCommunicationListenerTest.cs#L174) assert `Assert.Equal(KestrelSR.EndpointNameEmptyExceptionMessage, exception.Message)` — exact equality on `Message`. `ArgumentException.Message` is formatted as `"{message} (Parameter '{paramName}')"` when `ParamName` is non-null. Once the SUT is fixed per the adjacent `[Fact(Explicit = true)]` tests at [L128-L137](test/AspNetCore/KestrelCommunicationListenerTest.cs#L128-L137) and [L177-L186](test/AspNetCore/KestrelCommunicationListenerTest.cs#L177-L186), `exception.Message` will gain the `" (Parameter 'endpointName')"` suffix, breaking the equality assertion. The SUT fix would simultaneously turn the explicit tests green and the active tests red.

**Recommendation.** Assert the invariant message prefix with `Assert.StartsWith(KestrelSR.EndpointNameEmptyExceptionMessage, exception.Message)` (or `Contains`) and leave `ParamName` to the explicit bug tests.

**Iterator note (oscillation):** An earlier round (round ~9) explicitly directed us to switch FROM `Assert.StartsWith` TO `Assert.Equal` for parity with `HttpSysCommunicationListenerTest.cs` and with rationale "When the bug is fixed, the explicit test becomes non-explicit and the regular test is updated alongside." We complied and aligned both peer files. This round flips that position. Human decides which is correct; whichever way, please align both peer files.
---

## ⚠️ Should Fix — Base-class `sut` field is dead code
*Reported by opus; agreed by gemini and gpt.*

The base `sut` at [test/AspNetCore/KestrelCommunicationListenerTest.cs](test/AspNetCore/KestrelCommunicationListenerTest.cs#L29) is initialized in the base constructor but never referenced by any test. Every nested class either shadows it (`new readonly AspNetCoreCommunicationListener sut;` in `GetListenerUrl`) or constructs a fresh SUT locally inside each test. Consequences:

- Each nested test class still pays construction cost of an unused SUT.
- Constructor argument-null tests pass `null` to a *new* SUT and never touch the base `sut`.

[.github/instructions/test.instructions.md](.github/instructions/test.instructions.md) says *"Don't omit the base-class `sut` variable even if it's not applicable to every test"* — but the example shows the base `sut` being used by at least one nested class. Here no class uses it.

Cross-check consensus: prefer Option 1 — make the base `sut` load-bearing by moving `serviceContext = TestMocksRepository.GetMockStatelessServiceContext()` into the base fixture and having `GetListenerUrl` reuse `base.sut`. This aligns with the repo's pattern where the shared fixture is utilized by at least one nested test class, and resolves the IDE0052 noise documented in the existing `Needs Human Review` finding.

**Iterator note (oscillation):** An earlier round (round ~7) raised the same concern. The coder pushed back with: (1) the pattern is canonical per `test.instructions.md`; (2) HttpSys uses the identical pattern without warning; (3) Release `dotnet build` produces 0 IDE0052 warnings. Now reviewers reopen the finding. Human judgment requested to break the cycle. If the fix is applied, mirror in `HttpSysCommunicationListenerTest.cs`.

---

## ⚠️ Should Fix — `ThrowsArgumentNullExceptionWhenServiceContextIsNull/BuildIsNull` test the base ctor, not the SUT ctor
*Reported by opus; agreed by gemini and gpt.*

Every constructor overload's `ArgumentNullException` for `serviceContext` and `build` is thrown from the *base* `AspNetCoreCommunicationListener` constructor ([src/AspNetCore/AspNetCoreCommunicationListener.cs](src/AspNetCore/AspNetCoreCommunicationListener.cs#L37-L44) and [L60-L67](src/AspNetCore/AspNetCoreCommunicationListener.cs#L60-L67)) with hard-coded `"serviceContext"`/`"build"` literals. The four `Constructor_*` classes each duplicate the same two argument-null tests — eight tests total — that effectively test the base class's behavior.

`KestrelCommunicationListener` does no argument validation for these parameters before chaining `: base(serviceContext, build)` (see [src/AspNetCore.Kestrel/KestrelCommunicationListener.cs](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L28-L41) for the 2-arg overloads and [L56-L63](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L56-L63), [L78-L85](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L78-L85) for the 3-arg overloads). Only the `endpointName` empty-string check belongs to the SUT. The base behavior is already covered in [test/AspNetCore/AspNetCoreCommunicationListenerTest.cs](test/AspNetCore/AspNetCoreCommunicationListenerTest.cs#L254-L291).

Per test.instructions.md (*"Each test method should verify a single logical aspect of a single member of the SUT"*), these tests don't verify SUT behavior. Consider either:
1. Dropping the eight base-ctor-forwarding tests (keep only the `endpointName` tests in the two 3-arg classes plus the `GetListenerUrl` default-URL pinning), or
2. Keeping a single forwarding test per overload.

**Iterator note (oscillation):** An earlier round (round 4 of this iteration) explicitly demanded these tests under the heading "⚠️ Missing Argument Validation — No tests for null `serviceContext` or `build`," with rationale from `test.instructions.md`: *"Create explicit tests for missing argument validation that can cause NullReferenceException, etc. in consuming members."* We complied and added them across all four constructor overloads. This round flips the position, arguing the tests are testing the base class. Both arguments cite the same instructions file. Human judgment requested.
---

## ⚠️ Should Fix — Replace `ctor.Parameter<T>().Name` with `nameof(...)`

**Reported by:** opus. **Cross-check:** gpt Agree, gemini Agree. Elevated from 💡 to ⚠️ because two models independently support it.

The four `Constructor_*` classes capture `static readonly ConstructorInfo ctor` solely to read parameter names for `ArgumentNullException.ParamName` assertions, e.g. [test/AspNetCore/KestrelCommunicationListenerTest.cs#L52-L53](test/AspNetCore/KestrelCommunicationListenerTest.cs#L52-L53) and [test/AspNetCore/KestrelCommunicationListenerTest.cs#L58-L59](test/AspNetCore/KestrelCommunicationListenerTest.cs#L58-L59). The base-class fields `serviceContext`, `endpointName`, and `build` already match the SUT parameter names per `test.instructions.md` ("Use the exact SUT parameter names for fields"). `Assert.Equal(nameof(serviceContext), exception.ParamName)` is shorter, removes the `ctor` field, and matches `csharp.instructions.md` ("Use `nameof(...)` instead of hardcoding type or member names as string literals").

gpt adds: the test instructions' own `ArgumentNullException.ParamName` examples assert against `nameof(...)`; this pattern is established.

gemini adds: the current dynamic lookup is also weaker as a regression guard — if a SUT parameter is renamed, `ctor.Parameter<T>().Name` adapts silently. `nameof(serviceContext)` pins the expected name so a SUT rename breaks the test (and forces a deliberate fixture-field rename), which is the desired behavior.

**Iterator note (oscillation):** An earlier round (round ~6) explicitly demanded the opposite: switch FROM `nameof(testField)` TO `ctor.Parameter<T>().Name` (the Inspector pattern), with rationale: "nameof(serviceContext) resolves to the local test field. The base AspNetCoreCommunicationListener throws with hard-coded strings, not nameof, so the SUT parameter name and the thrown ParamName are decoupled — the test passes today only by naming coincidence." We complied and aligned both Kestrel and HttpSys peer files. This round flips that position. Human judgment requested. If reverted to `nameof`, apply equivalently to `HttpSysCommunicationListenerTest.cs` and remove the new `InspectorWorkarounds` helper if it has no remaining callers.


---

## ❓ Needs Human Review — Cast to `AspNetCoreCommunicationListener` before calling `GetListenerUrl()`

**Reported by:** Opus · **Cross-check:** Gemini Agree, GPT Disagree → Opus **Insist**

In four `Constructor_*` `GetListenerUrlReturnsDefaultHttpUrl*` tests, the listener is cast to `AspNetCoreCommunicationListener` to call `GetListenerUrl()`.

The base declaration at [src/AspNetCore/AspNetCoreCommunicationListener.cs](src/AspNetCore/AspNetCoreCommunicationListener.cs#L163) is `protected internal abstract`. The test assembly has `InternalsVisibleTo` access. Opus argues the cast is unnecessary and the locals can be `var sut = new KestrelCommunicationListener(...)`.

**Cross-check:**
- *Gemini — Agree:* Experimentally removed the casts; the project builds successfully.
- *GPT — Disagree:* Argued that the override at [src/AspNetCore.Kestrel/KestrelCommunicationListener.cs](src/AspNetCore.Kestrel/KestrelCommunicationListener.cs#L94) is declared `protected override`, and per the C# reference, an override of a `protected internal` member from a different assembly has only `protected` access — so calling through a `KestrelCommunicationListener`-typed receiver should fail.

**Opus re-evaluation — Insist (with empirical evidence):**
The test file already contains `new readonly KestrelCommunicationListener sut;` which calls `sut.GetListenerUrl()` directly, and the project builds with 0 errors. GPT applied a plausible-sounding rule without checking actual compiler behavior. The accessibility check for a virtual call considers the original member's declared accessibility on the base.

**Recommendation (pending human confirmation):** Remove the four casts. Consider mirroring in `test/AspNetCore/HttpSysCommunicationListenerTest.cs` if the pattern is identical there.
---

### ⚠️ Missing Inspector workaround TODO at four `GetConstructor` call sites

Reported by `opus`, agreed on cross-check by `gemini` and `gpt`.

[test/AspNetCore.Kestrel/KestrelCommunicationListenerTest.cs#L30-L31](test/AspNetCore.Kestrel/KestrelCommunicationListenerTest.cs#L30-L31), [#L62-L63](test/AspNetCore.Kestrel/KestrelCommunicationListenerTest.cs#L62-L63), [#L92-L93](test/AspNetCore.Kestrel/KestrelCommunicationListenerTest.cs#L92-L93), [#L142-L143](test/AspNetCore.Kestrel/KestrelCommunicationListenerTest.cs#L142-L143):

[.github/instructions/inspector.instructions.md](.github/instructions/inspector.instructions.md) requires using Inspector instead of `System.Reflection` and, when an Inspector workaround is necessary, requires a TODO with explanation, package version, and GitHub issue link. The TODO text has been removed but the workaround code remains.

**Coder investigation (Inspector 0.3.12):** Empirically verified that Inspector cannot disambiguate the two 2-arg `KestrelCommunicationListener` constructors that differ only by the delegate's return type (`Func<…,IHost>` vs `Func<…,IWebHost>`) — it throws `Sequence contains more than one element`. The 3-arg Kestrel constructors and both 2-arg HttpSys constructors resolve correctly. No open issue at https://github.com/olegsych/inspector/issues covers this case.

**Decision needed:**
1. **Hybrid:** Replace 3-arg Kestrel sites + HttpSys sites with Inspector calls; keep the two 2-arg Kestrel `GetConstructor` fields with a TODO citing Inspector 0.3.12 and the limitation. Requires filing an issue at `olegsych/inspector` and pasting its URL into the TODO.
2. **Full revert:** Keep all sites on reflection with the same TODO across both peer files.
