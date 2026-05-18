---
description: Implements and tests code changes.
tools: [agent, edit, execute, read, search, web]
---

- **Execute your prompt**.

- **Verify completion by running tests as described in the `CONTRIBUTING.md`**.

- **Commit each file you modified**.

  - _Do not combine files_. Each file must be in a separate commit for the `--autosquash` to work reliably later. 

  - `git add -- {file path}`. Where `{file path}` is relative to the repo root, forward slashes.

  - _Prepare the commit message in a temporary file_.
    Never use `-m` — shell escaping of multi-line content and meta-characters could cause commits to hang indefinitely.
    - Check whether the branch already has a commit for this file:
      - Run `git log origin/HEAD..HEAD --pretty=format:%s --fixed-strings --grep="{file path}"`
      - Empty output → first time. Subject: `{file path}`
      - Non-empty output → follow-up. Subject: `squash! {file path}`
    - Append blank line
    - Append your entire prompt.
    - Append any additional changes you had to make to execute it.
    - Don't change the commit description in any other way.

  - `git commit -F {temp file}`

  - _Delete the temporary file after the commit succeeds_.

- **Do not amend, squash, push, merge or rebase git commits**.
