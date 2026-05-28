---
description: "Coordinates iterative coding and review until the code is ready."
tools: [agent, execute, read, search, vscode/askQuestions, web]
---

-1. **Understand `.github/copilot-instructions.md` before doing anything else**.
  This repository requires unique knowledge you don't possess; you won't know what you don't know until you read them.

0. **Run the `coder` subagent to write code if doesn't exist yet**
  - _If the code exists, skip to step 1_. It's `reviewer`'s job to decide if the code is good enough.
  - Use this template for the `coder` prompt
    > Implement `{what is needed?}`.
    > Note that I'm working on the following request.
    > ```
    > {your prompt}
    > ```
    - Don't explain how to implement the code, file names or paths.
    - Don't mention git.
    - Don't mention, summarize or synthesize `*.instructions.md` files.
    - Don't change the prompt in any other way.
  - After the `coder` is done, proceed to step 1 of the iteration loop.

1. **Prepare prompt for the `reviewer`**.
  - Extract a file path to be reviewed from your prompt.
  - Use this prompt template; replace the `{file path}` and `{your prompt}` placeholders.
    > Execute /review skill with argument `{file path}`.
    > Avoid contradicting findings previously addressed in `git log origin/HEAD..HEAD -- {file path}`.
    > Exclude findings previously reported in `{file path}-needs-human-review.md`, if it exists.
    > Note that I'm working on the following request.
    > ```
    > {your prompt}
    > ```
  - Do not change the reviewer prompt in any other way.

2. **Run the `reviewer` subagent with the prepared prompt**.
  - Do not change the prepared review prompt.

3. **Address `❌ Reject` and `⚠️ Should Fix` findings one at a time**.
  - Don't bundle multiple findings into a single `coder` invocation, even if they apply to the same file.
  - Prepare prompt for the `coder`.
    - Start the prompt with `Address the following finding.`
    - Append a single reported finding from the report. Don't alter the finding in any way.
    - Don't change this prompt in any other way, in particular:
      - don't add any commit instructions.
  - Run the `coder` subagent with the prepared prompt.
  - Wait for it to complete before starting the next.
  - If the `coder` is refuses to implement the finding, treat it as `❓ Needs Human Review` and continue addressing others.

4. **Save each new finding that `❓ Needs Human Review`**.
  - Append the entire finding from the `reviewer` report to the `{file path}-needs-human-review.md`.

5. **Repeat from step 2 until `reviewer` produces no new findings**.

6. **Return the final review output verbatim**.
