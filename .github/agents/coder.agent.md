---
description: Implements and tests code changes.
tools: [execute, read, search, web]
---

- **Understand `.github/copilot-instructions.md` before doing anything else**.
  This repository requires unique knowledge you don't possess; you won't know what you don't know until you read them.

- **Read all applicable `SKILL.md` and `.instructions.md` files before writing code**. Any code you write must comply with
  the current guidance and not necessarily with the legacy code in this repo.

- **Execute your prompt**. The prompt will tell you _what_ to do. Your job is to decide _how_ to do it and make sure your
  work will pass through the rigorous code reviews with minimal follow-up changes.

- **End every text file you write with a single trailing newline** - `.editorconfig` sets `insert_final_newline = true`.
  Removing the trailing newline is a common mistake requiring unnecessary rounds of reviews and fixes.
  - _After writing a file, confirm its last byte is a line feed_.
    `$b=[IO.File]::ReadAllBytes($path); $b[-1]` must print `10` (this holds for both LF and CRLF endings).

- **Verify completion by running tests as described in the `CONTRIBUTING.md`**.

- **Commit each file you modified**.

  - _Do not combine files_. Each file must be in a separate commit for the `--autosquash` to work reliably later. 

  - `git add -- {file path}`. Where `{file path}` is relative to the repo root, forward slashes.

  - _Prepare the commit message in a `commit-message.tmp` file_.
    Never use `-m` — shell escaping of multi-line content and meta-characters could cause commits to hang indefinitely.
    - Check whether the branch already has a commit for this file:
      - Run `git log origin/HEAD..HEAD --pretty=format:%s --fixed-strings --grep="{file path}"`
      - Empty output → first time. Subject: `{file path}`
      - Non-empty output → follow-up. Subject: `squash! {file path}`
    - Append blank line
    - Append your entire prompt.
    - Append any additional changes you had to make to execute it.
    - Don't change the commit description in any other way.

  - `git commit -F commit-message.tmp`
  - `rm commit-message.tmp`

- **Do not amend, squash, push, merge or rebase git commits**.
