---
description: "Iterative review and implementation until multi-model confirmation."
tools: [agent, execute, read, search, vscode, web]
---

0. **Run the `coder` subagent to write code if doesn't exist yet**
  - After the `coder` is done, proceed to step 1 of the iteration loop.

1. **Prepare prompt for the `reviewer`**.
  - Extract a file path to be reviewed from your prompt.
  - Use this prompt template; replace the `{file path}` and `{your prompt}` placeholders.
    > /review `{file path}`.
    > Exclude findings already reported in `{file path}-needs-human-review.md`, if it exists.
    > Note that I'm working on the following request.
    > ```
    > {your prompt}
    > ```
  - Do not change the reviewer prompt in any other way.

2. **Run the `reviewer` subagent with the prepared prompt**.
  - Do not change the prepared review prompt.

3. **Address `❌ Reject` and `⚠️ Should Fix` findings one at a time**.
  - Prepare prompt for the `coder`.
    - Start the prompt with `Address the following finding.`
    - Append a single reported finding from the report. Don't alter the finding in any way.
    - Don't change this prompt in any other way.
  - Run the `coder` subagent with the prepared prompt.
  - Wait for it to complete before starting the next.
  - Don't bundle multiple findings into a single `coder` invocation, even if they apply to the same file.

4. **Save each new finding that `❓ Needs Human Review`**.
  - Append the entire finding from the `reviewer` report to the `{file path}-needs-human-review.md`.

5. **Repeat from step 2 until `reviewer` produces no new findings**.

6. **Return the final review output verbatim**.
