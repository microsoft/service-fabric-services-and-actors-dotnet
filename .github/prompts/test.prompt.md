---
description: "Test public and internal types in a given scope."
argument-hint: "(type|namespace|project)"
---

- Read `.github/copilot-instructions.md` before doing anything else.
- Find all `public` and `internal` types in the target scope.
  - Exclude compiler-generated types, types without concrete methods.
  - When a file contains multiple top-level types, each top-level type is a separate target.
  - Add the target types to the `todo` list.
- Repeat for each target type, **one at a time**.
  - Run the `iterator` subagent with the following prompt.
    > Test <target-type> in <target-project>.
    > Bug fixes, testability improvements in <target-type> are out of scope.
    > Restructure existing, eliminate redundant tests as needed.
  - Print the response returned by the `iterator`.
  - Don't pause for questions if the `iterator` finished with findings that need human review.
  - Mark the `todo` item completed.
  - Go to the next type.
