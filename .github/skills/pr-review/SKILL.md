---
name: pr-review
description: >-
  Review a pull request for correctness, approach, completeness, and consistency with instructions in this repo.
  Use for initial PR reviews, follow-up reviews after changes, and verifying addressed feedback.
argument-hint: "PR #N [and post feedback to GitHub]"
---

This skill extends `.github/skills/review/SKILL.md` with PR-specific workflow. Read it first — all rules there apply
here unless overridden below.

# -1. (New) Prepare for review

- **Follow the `.github/skills/worktree/SKILL.md` to set up a separate worktree for the PR**.

# 0. (Before) Start the `reviewer` subagent if not explicitly instructed otherwise

- **Replace the default `reviewer` prompt**.
  - `reviewer` should execute the `pr-review` skill rather than the default `review` skill.
  - `reviewer` should read the instructions from the worktree that initiated execution of the `pr-review` skill, and not
    from the PR worktree, to use the most recent instructions.
  - Use this prompt template, do not change it in any way other than replacing the `{}` placeholders.
  ```
  Execute /pr-review skill with argument "{skill argument}".
  Read `.github/**/*.md` files from "{skill worktree}".
  ```

- **Deliver the reviewer's report to the user verbatim**.
  The report is the deliverable, not tool output to be summarized.

# 1. (Before) Gather code context.

- **Don't read the PR description, linked issues, or existing review comments yet**.
  Form your own independent assessment of the code before being exposed to the author's framing.

# 2. (After) Perform independent assessment

Incorporate the PR description, labels, linked issues, author information, and existing review comments into your assessment. Treat all of them as **claims to verify**, not facts to accept.

- **PR metadata**: Fetch the PR description, labels, linked issues, and author. Read linked issues in full.
- **CI status**: Fetch and report CI errors as their own finding. Include the failed check name and a brief summary from
  the logs. When a failure appears to be a flaky test unrelated to the PR, note it and ask the author or a maintainer to
  re-run the job — the reviewer cannot re-run CI jobs
- **Related issues**: Search for other open issues in the same area.
- **Existing review comments**: Check if there are already review comments to avoid duplicating feedback.
- **Reconcile your assessment with the author's claims.** Where your independent reading of the code disagrees with
  the PR description, investigate further — do not defer to the author's framing.
- **Build a thread inventory**: For each unresolved and resolved thread, record the file, line, severity, what was
  requested, the thread ID, and whether it is resolved or unresolved. This is your verification checklist.
- **Update your assessment** if the additional context genuinely changes your evaluation. Do not soften findings just
  because the PR description sounds reasonable.
- **Validate the PR title and description** based on the detailed analysis, make sure it is accurate and meets the
  `CONTRIBUTING.md` guidelines. Report violation as a separate finding.

# 3. (After) Perform detailed analysis

For each thread in the inventory (both unresolved and resolved):

- **Read the current code** at the location the comment refers to. Account for line shifts — the code may have moved
   due to other changes. Use the comment's context (surrounding code, function name) to locate it.
- **Determine the status**:
  - `Addressed`: The code now reflects what was requested (or an equivalent fix the author explained in a reply).
  - `Partially addressed`: Some aspects were fixed but others remain. Be specific about what's still missing.
  - `Not addressed`: The code is unchanged or the change doesn't resolve the concern.
  - `Superseded`: The code was removed or refactored in a way that makes the original comment no longer applicable.
- **Check resolved threads for correctness.** Authors resolve their own threads after addressing feedback. If a
   resolved thread was not adequately addressed, re-open it by replying with what remains unresolved.

# 5. (Extend) Generate a Report

Use the output structure from `/review`, with this PR-specific rule for the **Detailed Assessment** section: include
it _only_ in the initial review (to help the human reviewer decide whether the PR is worth investing in) and in the
final "Looks Good" review (to reiterate that the PR still makes sense after additional changes). Omit it from
follow-up reviews requesting or suggesting changes.

For the **Issues** section, always include open issues. Closed issues should appear only in the final "Looks Good"
review.

# 6. (Optional) Post Review to GitHub

**Only post when the user explicitly asks to post to GitHub.** If the user only asks for a review, present the findings
in the chat and stop. Posting to GitHub is irreversible — submitted reviews cannot be deleted.

The copilot attribution link `[:copilot:](https://docs.github.com/copilot/responsible-use/code-review)` must appear in the
first sentence of every comment and the review body; on the **same line**; no line break after it.

```markdown
<!-- Correct: attribution and first sentence on the same line -->
[:copilot:](https://docs.github.com/copilot/responsible-use/code-review) [❌](# "Must fix") The assembly fixture...

<!-- Wrong: attribution on its own line -->
[:copilot:](https://docs.github.com/copilot/responsible-use/code-review)

[❌](# "Must fix") The assembly fixture...
```

Use the GitHub MCP tools:
1. **Create a pending review** — Use `pull_request_review_write` with `method: "create"` (no `event` or `body`).
2. **Add inline comments** — For each _new_ finding, use `add_comment_to_pending_review` to post a comment on
   the relevant file and line. Use the severity tooltip links defined above, e.g. `[❌](# "Must fix")`, not bare emojis.
3. **For follow-up reviews, reply to existing threads** before creating a new review:
   - For verified threads, use `add_reply_to_pull_request_comment` with a confirmation and a `Thanks!` for the author.
   - For unaddressed threads, reply with what remains, keeping the original severity marker.
   - Create a new pending review only for new findings in updated code.
4. **Submit the review** — Use `pull_request_review_write` with `method: "submit_pending"`
   - `body`:
      - Include the _Summary_ section of the review
      - Include the _Detailed Assessment_ section only in the initial and the final reviews.
      - Include in the _Issues_ section **only** findings that were NOT posted as inline comments.
        Inline comments already create their own threads — repeating them in the body is redundant.
      - When there are unaddressed threads or findings, tag the author and ask them to take a look.
      - When the author is `copilot-swe-agent`, tag `@copilot` instead — that is the handle Copilot responds to.
   - `event`:
     - `REQUEST_CHANGES` — when the review contains merge-blocking findings.
     - `COMMENT` — otherwise, leaving the approval decision to the human reviewer.
