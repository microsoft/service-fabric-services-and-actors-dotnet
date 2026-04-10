---
name: review
description: "Review code changes for correctness, performance, and consistency with project conventions."
argument-hint: "PR #N [with multiple models] [and post feedback to GitHub]."
---

# Code Review

Review code changes against conventions established in this repository's `.instructions.md` files and `CONTRIBUTING.md`.

**Reviewer mindset:** Be polite but skeptical. Your job is to find problems the author may have missed and to question
whether the change is a net positive for the codebase. Treat the PR description and linked issues as claims to verify,
not facts to accept.

## When to Use This Skill

- Reviewing a PR or code change
- Checking code for correctness, style, or consistency before submitting a PR
- Validating that a change follows project conventions

## Review Process

### Step 0: Gather Code Context (No PR Narrative Yet)

Before analyzing anything, collect as much relevant **code** context as you can. **Do NOT read the PR description,
linked issues, or existing review comments yet.** Form your own independent assessment of what the code does and why
before being exposed to the author's framing.

1. **Diff and file list**: Fetch the full diff and the list of changed files.
2. **Full source files**: For every changed file, read the **entire source file**, not just the diff hunks. You need
   the surrounding code to understand invariants, call patterns, and data flow.
3. **Consumers and callers**: If the change modifies a public or internal API, search for callers and usages.
   Understanding how the code is consumed reveals whether the change could break existing behavior.
4. **Sibling types and related code**: If the change fixes a bug or adds a pattern in one type, check whether sibling
   types have the same issue or need the same fix.
5. **Key utility/helper files**: If the diff calls into shared utilities, read those to understand the contracts.
6. **Git history**: Check recent commits to the changed files (`git log --oneline -20 -- <file>`). Look for related
   recent changes, reverts, or prior attempts to fix the same problem.
7. **Applicable instructions**: Read the `.instructions.md` files that apply to the changed files (based on their
   `applyTo` patterns) and the closest `README.md` and `CONTRIBUTING.md` files.

### Step 1: Form an Independent Assessment

Based **only** on the code context gathered above (without the PR description or issue), answer:

1. **What does this change actually do?** Describe the behavioral change in your own words. What was the old behavior?
   What is the new behavior?
2. **Why might this change be needed?** Infer the motivation from the code itself.
3. **Is this the right approach?** Would a simpler alternative work? Could the goal be achieved with existing
   functionality?
4. **What problems do you see?** Identify bugs, edge cases, missing validation, test gaps, and anything else that
   concerns you.

Write down your independent assessment before proceeding. You must produce the
[Holistic Assessment](#holistic-pr-assessment) at this stage.

### Step 2: Incorporate PR Narrative and Reconcile

Now read the PR description, labels, linked issues, author information, and existing review comments. Treat all of
this as **claims to verify**, not facts to accept.

1. **PR metadata**: Fetch the PR description, labels, linked issues, and author. Read linked issues in full.
2. **Related issues**: Search for other open issues in the same area.
3. **Existing review comments**: Check if there are already review comments to avoid duplicating feedback.
4. **Reconcile your assessment with the author's claims.** Where your independent reading of the code disagrees with
   the PR description, investigate further — do not defer to the author's framing.
5. **Update your holistic assessment** if the additional context genuinely changes your evaluation. Do not soften
   findings just because the PR description sounds reasonable.

### Step 3: Detailed Analysis

1. **Focus on what matters.** Prioritize bugs, safety issues, incorrect assumptions, and problems that would affect
   consumers. Do not comment on trivial style issues unless they violate an explicit rule in the applicable
   `.instructions.md` files.
2. **Consider collateral damage.** For every changed code path, brainstorm: what other scenarios, callers, or inputs
   flow through this code? Could any of them break after this change? Surface plausible risks so the author can
   evaluate.
3. **Be specific and actionable.** Every comment should tell the author exactly what to change and why. Include
   evidence of how you verified the issue is real.
4. **Flag severity clearly:**
   - `[❌](# "Must fix before merge")` — Bugs, security issues, test gaps for behavior changes.
   - `[⚠️](# "Should fix")` — Missing validation, inconsistency with established patterns.
   - `[💡](# "Consider changing")` — Readability wins, minor improvements.
5. **Don't pile on.** If the same issue appears many times, flag it once with a note listing all affected
   locations.
6. **Respect existing style.** When modifying existing files, the file's current style takes precedence over general guidelines.
7. **Don't flag what CI catches.** Do not flag issues that a compiler, analyzer, or CI build step would catch.
8. **Avoid false positives.** Before flagging any issue:
   - Verify the concern applies given the full context, not just the diff.
   - Skip theoretical concerns with negligible real-world probability.
   - If unsure, investigate further or surface it explicitly as a low-confidence question.
   - Trust the author's context. If a pattern is consistent with the repo, assume it's intentional.
   - Never assert that something "does not exist" or "is unavailable" based on training data alone. Ask rather
     than assert.
9. **Ensure code suggestions are valid.** Any code you suggest must be syntactically correct and complete.
10. **Label in-scope vs. follow-up.** Distinguish between issues the PR should fix and out-of-scope improvements.
11. **Validate the PR title and description** based on the detailed analysis, make sure it is accurate and meets the
   `CONTRIBUTING.md` guidelines.

## Multi-Model Review (Optional)

When requested, use the `opus`, `gemini` and `codex` sub-agents to get diverse perspectives. Different models catch different
classes of issues. If not requested, proceed with a single-model review using Steps 0–3 above.

1. Launch each sub-agent in parallel, giving each the same review prompt: the PR diff, the review rules from this
   skill, and instructions to produce findings in the severity format defined above.
2. Wait for all agents to complete, then synthesize: deduplicate findings across models, elevate issues flagged by
   multiple models (higher confidence), and include unique findings that meet the confidence bar. If a sub-agent has
   not completed after 10 minutes and you have results from others, proceed without it.
3. Present a single unified review, noting when an issue was flagged by multiple models.

## Review Output Format

### Structure

```
**<✅ Looks Good / ⚠️ Needs Human Review / ⚠️ Needs Changes / ❌ Reject>**: <2-3 sentence summary of the overall
verdict and key points. If "Needs Human Review," state which findings you are uncertain about and what a human
reviewer should focus on.>

**Motivation**: <1-2 sentences on whether the PR is justified and the problem is real>

**Approach**: <1-2 sentences on whether the change takes the right approach>

---

### ✅/⚠️/❌ <Category Name> — <Brief description>

<Explanation with specifics. Reference code, line numbers, etc.>

(Repeat for each finding category. Group related findings under a single heading.)
```

### Guidelines

- **Holistic Assessment** comes first with Summary, Motivation and Approach.
- **Detailed Findings** uses emoji-prefixed category headers.
- **Test quality** should be assessed as its own finding when tests are part of the PR.
- **Summary** gives a clear verdict: LGTM (no blocking issues, confident the change is correct), Needs Human Review
  (unresolved concerns requiring human judgment), Needs Changes (blocking issues listed), or Reject (explaining why).
- **Never give a blanket LGTM when you are unsure.** Use "Needs Human Review" instead.
- Keep the review concise but thorough. Every claim should be backed by evidence from the code.

### Verdict Consistency Rules

1. **The verdict must reflect your most severe finding.** Only use "LGTM" when all findings are ✅ or 💡 and you are
   confident the change is correct.
2. **When uncertain, always escalate to human review.** A false LGTM is far worse than an unnecessary
   escalation.
3. **Separate code correctness from approach completeness.** A change can be correct code that is an incomplete
   approach. The verdict must reflect the gap.
4. **Classify each ⚠️ and ❌ finding as merge-blocking or advisory.**
5. **Devil's advocate check before finalizing.** Re-read all ⚠️ findings. For each one, ask: does this represent an
   unresolved concern? If so, the verdict must reflect that tension.

## Post Review to GitHub (Optional)

When requested, post the review as a GitHub PR review with inline comments. 
- Start the first sentence of the review body and each comment with `[:copilot:](https://docs.github.com/copilot/responsible-use/code-review)`
  inline, not on a separate line.

Use the GitHub MCP tools:
1. **Create a pending review** — Use `pull_request_review_write` with `method: "create"` (no `event` or `body`).
2. **Add inline comments** — For each Detailed Finding, use `add_comment_to_pending_review` to post a comment on
   the relevant file and line. Use the severity tooltip links defined above, e.g. `[❌](# "error")`, not bare emojis.
3. **Submit the review** — Use `pull_request_review_write` with `method: "submit_pending"`
   - `body` should contain the Holistic Assessment. The GitHub API ignores the body from the create step.
   - `event` should be `REQUEST_CHANGES` if the review contains merge-blocking findings.
     Otherwise it should be `COMMENT` and leave the approval decision to the human reviewer.

## Holistic PR Assessment

Before reviewing individual lines of code, evaluate the PR as a whole.

### Motivation & Justification

- Every PR must articulate what problem it solves and why.
- Challenge every addition with "Do we need this?"
- Demand real-world use cases. Hypothetical benefits are insufficient motivation.

### Approach & Alternatives

- Check whether the PR solves the right problem at the right layer.
- When a PR takes a fundamentally wrong approach, redirect early.
- Always prefer the simplest solution. The burden of proof is on the complex solution.

### Cost-Benefit & Complexity

- Explicitly weigh whether the change is a net positive.
- Reject overengineering — complexity is a first-class cost.
- Every addition creates a maintenance obligation.

### Scope & Focus

- Require large or mixed PRs to be split into focused changes. Each PR should address one concern.
- Defer tangential improvements to follow-up PRs.

### Risk & Compatibility

- Flag breaking changes and require documentation.
- Assess regression risk proportional to the change's blast radius.

### Codebase Fit & History

- Ensure new code matches existing patterns and conventions.
- Check whether a similar approach has been tried and rejected before.
