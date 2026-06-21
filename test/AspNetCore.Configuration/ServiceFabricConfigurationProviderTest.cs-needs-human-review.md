### ❓ Needs Human Review — `Assert.Equal` where `Assert.Same` applies

**Reported by:** opus. **Cross-check:** gemini Disagree (rule misquoted), gpt Agree. **Author response:** Insist with corrected attribution.

Lines 122 and 201 use `Assert.Equal(existingValue, actualValue)` for values round-tripped through `Set`/`TryGet`. The sibling test `PreservesPreviouslyLoadedData` (line 268) uses `Assert.Same` for the same pattern, making this locally inconsistent.

The original citation of `moq.instructions.md` was imprecise; the actual rule is "Use `Assert.Same` to verify return values ... and rule out unexpected transformations." Opus argues this applies to value-preservation round-trips too. Gemini argues the rule is specific to Moq return values, not xUnit state assertions in general.

**Action:** Human decides whether `Assert.Same` should replace `Assert.Equal` on lines 122 and 201 for consistency with line 268.


## ❓ Needs Human Review — Rename `Constructor.LoadsPackageAndNotifiesChangeWhen…` methods

**Reported by:** gemini (💡)  
**Cross-check:** gpt Agree, opus Disagree  
**Author response:** Insist

> gemini: Under the nested-class convention, the class name establishes the subject of the sentence. `Constructor.LoadsPackage…` grammatically asserts the *constructor* performs the loading, which is demonstrably false — the constructor's only synchronous responsibility is wiring the event listener. The event subscription *is* the constructor's externally observable behavior; triggering the event in the test is merely the verification technique. An alternative like `ConfiguresSutToLoadPackageWhenAddedEventMatches` accurately reflects the constructor's setup role.

> opus (cross-check): "Loads package and notifies change" describes the *outcome* triggered through the constructor's wiring. The suggested rewording foregrounds the mechanism, which is closer to test mechanics than externally observable behavior; the current names already read as natural sentences.

> gpt (cross-check): Agree — names should describe SUT behavior accurately; loading happens later when events fire.

Defer to human judgment on whether the constructor's "behavior" is the wiring or the eventual outcome.