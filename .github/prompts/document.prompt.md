---
description: "Document public types in a given scope."
argument-hint: "(type|namespace|project)"
---

- **Read `.github/copilot-instructions.md` before doing anything else**.
- **Identify top-level `public` types in the target scope**.
  - _Include all types, regardless of their number_. User will stop processing if needed.
  - _Include every public top-level type in a file_. Each public top-level type needs to be documented.
  - _`{target-type}` means `{namespace-name}.{type-name} in {relative-file-path}`_.
- **Sort target types from the most generic to the most specific** to enable use of `<inheritdoc/>`.
  - _Place base types/interfaces before types derived from/implementing them_.
  - _Place generics before specializations_. E.g. place `IComparable<T>` extension classes before extension classes for
    types implementing `IComparable<T>`.
  - _Place providers before consumers_. E.g. place `IEnumerable<T>` before `Enumerable` that uses it.
- **Ensure `TODO.md` exists in the repo root**.
  - _Don't use the `manage_todo_list` tool_. It cannot handle large lists efficiently.
  - _Use this file template_. Add one line for each target type.
    ```md
    Executing `<userRequest>`
    - [ ] {target-type}
    ```
  - _If `TODO.md` already exists, check the prompt stored in it_
    - If the stored prompt is the same, resume from the first unfinished type.
    - If the stored prompt is different, stop and ask what to do.
- **Repeat for each target type, _one at a time_**.
  - _Print start note_: `[{types-finished + 1}/{total-types}] Documenting {target-type} ...`
  - _Run the `iterator` subagent_; use this prompt:
    ```md
    Ensure `{target-type}` has complete, accurate, compiler-verified XML documentation comments for all public members.
    API, functional, code style changes are out of scope.
    ```
  - _`{iterator-output}` should be `✅ Looks Good` or `❓ Needs Human Review`_.
  - _Don't pause for `iterator` responses that need human review_. User will review them later.
  - _Print finish note_: `[{types-finished}/{total-types}] Finished {target-type}: {iterator-output}.`
  - _Update the `TODO.md`_.
    - Don't re-read the `TODO.md` file.
    - `replace_string_in_file` from _oldString_ `- [ ] {target-type}` to _newString_ `- [x] {target-type}: {iterator-output}.`
  - _Go to the next type_.
