---
name: pr-iterate
description: "Iterative review and implementation on a pull request until multi-model confirmation."
argument-hint: "PR #N"
---

This skill extends `.github/skills/iterate/SKILL.md` with PR-specific workflow. Read it first — all rules there apply
here unless overridden below.

0. New: **Set up or a reuse a worktree for the PR**.
  - Follow `.github/skills/pr-worktree/SKILL.md`
  - Run all subsequent steps from the worktree directory.

1. Replace: **Prepare prompt for the `reviewer`**.
  - Reduce the `pr-iterate` skill prompt to a PR number.
  - Use this prompt template `/pr-review {N}` and replace the `{N}` placeholder.
  - Do not synthesize, expand, paraphrase, or restructure the review prompt in any other way.

4. Extend: **Address each finding reported by the `reviewer` separately from other findings**.
  - Make all edits and commits in the PR worktree, on the PR branch.
