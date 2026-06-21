#### ❓ Needs Human Review — Local `sut` is inferred as `ServiceFabricSetupFilter`, not `IStartupFilter`

Reported by **Opus**; cross-check: **GPT Agree**, **Gemini Disagree**; Opus **Insists**.

The base-class `sut` field is `IStartupFilter` ([test/AspNetCore/ServiceFabricSetupFilterTest.cs](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L20)). Tests that need alternate constructor arguments shadow it with `var sut = new ServiceFabricSetupFilter(...)` at [#L74](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L74), [#L84](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L84), [#L91](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L91), [#L99](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L99), [#L107](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L107), [#L115](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L115), [#L124](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L124), [#L132](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L132), [#L140](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L140), [#L150](test/AspNetCore/ServiceFabricSetupFilterTest.cs#L150). Each call site uses only `Configure` (the `IStartupFilter` member).

**Opus (Insist):** The C# rule reads "Specify variable type explicitly. Use `var` to prevent duplication of the variable type" — default is explicit, `var` is the exception, justified only against duplication. `IStartupFilter sut = new ServiceFabricSetupFilter(...)` contains no duplication. The test rule affirmatively requires the primary-interface type: the base `sut` is already `IStartupFilter` (line 20), every shadow calls only `Configure`, and product DI registers the SUT as `IStartupFilter`. Letting `var` silently widen the local to the concrete type breaks equivalence with the base field.

**GPT (Agree):** Test guidance says to choose `sut` type based on the primary interface when consumed through a specific interface. `IStartupFilter sut = new ServiceFabricSetupFilter(...)` does not duplicate the variable type; it documents the contract being tested and keeps the shadowed `sut` consistent with the base `sut`.

**Gemini (Disagree):** `var sut = new ServiceFabricSetupFilter(...)` complies with the `var`-when-RHS-states-the-type reading of `csharp.instructions.md`. `test.instructions.md` does not explicitly demand local SUT re-instantiations match the interface type of the class-level field.

Flagged for human review because the same broader tension (local SUT instance conventions) is unresolved in [test/AspNetCore/AspNetCoreCommunicationListenerTest.cs-needs-human-review.md](test/AspNetCore/AspNetCoreCommunicationListenerTest.cs-needs-human-review.md). A human should decide whether the primary-interface rule for the field extends to all local SUT instances in the same test class.

#### ❓ Needs Human Review — Reverse-proxy independence vs deletion of redundant urlSuffix variants

A previous reviewer iteration found `ReturnsActionThatRegistersReverseProxyIntegrationMiddlewareWhenUrlSuffixIs(Null|Empty)` redundant (varying `urlSuffix` while asserting reverse-proxy registration, which is invariant under that variation) and recommended deletion. That deletion is committed.

The current reviewer iteration recommends re-adding those exact tests to prove branch independence (mutation that nested reverse-proxy under the non-empty `urlSuffix` branch would not be caught). These two recommendations directly contradict each other; human judgment is needed to choose between deletion-for-non-independence and re-addition-for-mutation-coverage.

#### ❓ Needs Human Review — `urlSuffix` forwarding: Inspector private-field vs observable-behavior

A prior reviewer iteration found `middleware.Field<string>().Value` (Inspector reaching into `ServiceFabricMiddleware`'s `private` field) brittle and a violation of `inspector.instructions.md` ("Don't access `private` members when alternatives exist"). We replaced it with an observable-behavior assertion that drives a real `DefaultHttpContext` through the captured pipeline.

The current reviewer iteration finds the observable assertion couples to `ServiceFabricMiddleware`'s path-rewriting behavior (a different SUT) and recommends reverting to `middleware.Field<string>().Value`.

These recommendations directly contradict each other; human judgment is needed.

#### ❓ Needs Human Review — `next` field: unwrapped `Action<T> = Mock.Of<...>()` vs `Mock<Action<T>>`

A prior reviewer iteration cited `moq.instructions.md` to prefer unwrapped `Action<IApplicationBuilder> = Mock.Of<...>()` because most sites pass `next` through. We applied that change, replacing the few `Mock<T>` API call sites with `Mock.Get(next)`.

The current reviewer iteration cites the same `moq.instructions.md` to recommend reverting to `Mock<Action<IApplicationBuilder>> next = new();` and passing `next.Object`, on the grounds that there are now multiple `Mock.Get` wrappers.

These recommendations directly contradict each other; human judgment is needed.

#### ❓ Needs Human Review — `var` locals in `urlSuffix` forwarding test

The current reviewer iteration flags `var nextCalled = false;` and `var remainingPath = fuzzy.String().LettersOrDigits();` as needing explicit types per `csharp.instructions.md`. The reviewer itself notes this finding "becomes moot if the previous finding is applied, since the locals disappear" — i.e. it is conditional on the contradictory urlSuffix finding above. Deferred to whichever way the human resolves that one.
