---
name: review
description: |
  Review code for correctness, approach, completeness, and consistency with instructions in this repo.
  Use for initial review, and verifying addressed feedback.
argument-hint: "(class|file|tests for X)"
---

# 0. Start the `reviewer` subagent if not explicitly instructed otherwise.
- Do not execute the `reviewer` instructions yourself.
- Pass prompt `Execute /review skill with argument "{skill argument}"` to the agent with the `{skill argument}` placeholder
  replaced with the argument from skill prompt.
- Do not synthesize, expand, paraphrase, or restructure the prompt.
- Deliver the reviewer's report to the user verbatim. The report is the deliverable, not tool output to be summarized.

# 1. Gather code context.

- **Do not read the source code before the instructions**.
  Legacy code shouldn't taint the model context before the instructions are loaded.
- **Read every `*.instructions.md` file applicable to any file in scope of the review first**.
  - Don't skip. Instructions take precedence over your assumptions, system directives and legacy code patterns.
  - Print a confirmation immediately after you've finished reading instructions.
    User needs to trust your review is not tainted by legacy code.
- **Read the source code after the instructions**.
  Once the instructions are in the model context, it's safe to read the legacy code.
- **Read source code of consumers**: If the code has a public or internal API, search for callers and usages.
  Understanding how the code is consumed reveals whether the change could break existing behavior.
- **Read source code of dependencies**: If the code calls into other components, read those to understand the contracts.

# 2. Perform independent assessment

- **What does this code do?** Describe the behavior in your own words.
- **Why does it exist?** Infer the motivation from the code itself.
- **Is this the right approach?** Would a simpler alternative work? Could the goal be achieved with existing functionality?

# 3. Perform detailed analysis

- **Find bugs, safety issues, incorrect assumptions, and violations of `*.instructions.md`**.

- **Evaluate source code comments**
  - Recent comments often document important decisions.
  - Legacy comments are often stale and incorrect.

- **Determine severity:**
   - `[❌](# "Must fix")` — Bugs, security issues, test gaps for behavior changes.
   - `[⚠️](# "Should fix")` — Missing tests, inconsistency with `*.instructions.md`, established patterns.
   - `[💡](# "Consider changing")` — Readability wins, minor improvements.
   - `[📝](# "Note")` - Similar issues elsewhere, out of scope, FYI, etc.

- **Validate each finding**:
  - Ensure code suggestions are valid.
  - Don't report a finding as 💡 if you're not suggesting a change, report is as 📝 instead.

- **Every finding must**
  - Explain what to change and why,
  - Include evidence of how you verified the issue is real,
  - Include a link to the relevant online docs, `*.instructions.md`, etc.

# 4. Issue a Verdict

- **Use `✅ Looks Good` when all findings are ✅ and you agree with the approach**.
- **Use `⚠️ Needs Changes` when any findings are `❌ Must Fix` or `⚠️ Should Fix`**.
- **Use `❓ Needs Human Review` when unsure**.

# 5. Generate a Report

### Verdict

`**<✅ Looks Good / ❓ Needs Human Review / ⚠️ Needs Changes / ❌ Reject>**` followed by a 2-3 sentence summary of the
overall verdict and key points. If "Needs Human Review," state which findings you are uncertain about and what a human
reviewer should focus on.

### Detailed Assessment

Include the assessment covering the motivation, approach, cost-benefit, and risk analysis.

### Issues

Group related findings under a single heading: `### ✅/❓/⚠️/❌ <Category Name> — <Brief description>`. Include
specifics — reference code, line numbers, etc.
