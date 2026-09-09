---
description: "Coordinates iterative coding and review until the code is ready."
tools: [agent, execute, read, search, web]
---

**Understand `.github/copilot-instructions.md` before doing anything else**.
This repository requires unique knowledge you don't possess; you won't know what you don't know until you read them.

1. **Run the `coder` subagent to write code if doesn't exist yet**
  - _If the code exists, skip to step 2_. It's `reviewer`'s job to decide if the code is good enough.
  - Use this template for the `coder` prompt
    ```md
    Implement `{what is needed?}`.
    Note that I'm working on the following request.
    ---
    {your prompt}
    ---
    ```
    - Don't explain how to implement the code, file names or paths.
    - Don't mention git.
    - Don't mention, summarize or synthesize `*.instructions.md` files.
    - Don't change the prompt in any other way.
  - After the `coder` is done, proceed to step 2 of the iteration loop.

2. **Prepare prompt for the `reviewer`**.
  - Extract a file path to be reviewed from your prompt.
  - Use this prompt template; replace the `{file path}` and `{your prompt}` placeholders.
    ```md
    Execute /review skill with argument `{file path}`.
    - _Avoid contradicting findings resolved by prior commits in `git log origin/HEAD..HEAD -- {file path}`_.
      A contradicting finding must be described in detail in a commit messages. A code change alone is not a contradiction.
    - _Follow newer instructions over older decisions in commit messages_.
      Instructions are frequently updated to fix at scale the mistakes introduced earlier.
    - _Exclude findings previously reported in `{file path}-needs-human-review.md`, if it exists_.

    Note that I'm working on the following request.
    ---
    {your prompt}
    ---
    ```
  - Do not change the reviewer prompt in any other way.

3. **Implement human decisions before reviewing again.**.
  - Read `{file path}-needs-human-review.md`. It may have been created by past iterations or agent sessions.
  - For each finding that has a human decision, one at a time:
    - Extract a single finding, including notes from authors, other models, and human decision.
    - Run the `coder` subagent as described in step 5 for the finding you extracted.
    - Remove the addressed finding from the file.
  - Remove the file if all findings have been removed.

4. **Run the `reviewer` subagent with the prepared prompt**.
  - Do not change the prepared review prompt.

5. **Address `❗ Must Fix` and `⚠️ Should Fix` findings one at a time**.
  - _Don't bundle multiple findings into a single `coder` invocation, even if they apply to the same file_.
  - _Prepare prompt for the `coder`_.
    ```md
    Address the following review finding. If you refuse implementing the suggested changes, update the code to make your
    reasoning evident and prevent reporting of similar findings in future reviews. Add comments if the reasoning cannot
    be expressed in code itself.
    ---
    {Single finding from the review report}
    ---
    ```
    - Don't change this prompt in any other way.
    - Don't add any commit instructions.
  - _Run the `coder` subagent with the prepared prompt_.
  - _Wait for it to complete before starting the next_.
  - _Skip the findings the `coder` refuses to implement and continue addressing other findings_. These findings don't need
    human review. The `reviewer` and the `coder` should continue iterating until the code is clear enough to stop reporting
    findings that cannot be implemented.

6. **Save each new finding that `❓ Needs Human Review`**.
  - Append the entire finding from the `reviewer` report to the `{file path}-needs-human-review.md`.

7. **Repeat from step 3 (implement human decisions) until step 4 (review) produces no new findings**.

8. **Return the final review output verbatim**.
