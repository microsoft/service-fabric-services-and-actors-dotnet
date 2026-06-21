## ❓ Needs Human Review — `host.StartAsync`/`StopAsync` return the well-known `Task.CompletedTask`

*Reported by `gemini` ⚠️; opus flagged same site as 📝 noting the sibling parking. Overlaps with the oscillating preference parked in [test/AspNetCore/AspNetCoreCommunicationListenerTest.cs-needs-human-review.md](AspNetCoreCommunicationListenerTest.cs-needs-human-review.md) — "Oscillating preference: `Task.CompletedTask` vs unique completed task in fixture defaults".*

The base constructor configures both with `Task.CompletedTask` at [test/AspNetCore/WebHostCommunicationListenerTest.cs](test/AspNetCore/WebHostCommunicationListenerTest.cs#L33-L34). [moq.instructions.md](.github/instructions/moq.instructions.md) instructs: *"Use unique/generated argument and return values instead of passing well-known values like `null`, `Task.CompletedTask` that could also be used by the product code unexpectedly."* Concrete substitution: return a unique completed task via `Task.FromResult(new object())` so a regression that bypasses the mocked call cannot accidentally observe the same sentinel.

Flagged as ❓ Needs Human Review because this is the same oscillating tension already parked for the sibling fixture; the two files should be resolved together. The current `Task.CompletedTask` state matches the sibling precedent.
