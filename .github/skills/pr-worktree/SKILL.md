---
name: pr-worktree
description: |
  Create, reuse or clean up a dedicated git worktree for working on a pull request.
  Use this skill from any PR-scoped workflow that needs an isolated checkout (e.g. `/pr-review`, `/pr-iterate`) so that
  the user's current code and build output stay untouched.
argument-hint: "PR #N"
---

## Set Up

Reuse the existing PR worktree if present, otherwise create one. Run subsequent steps from the worktree directory.

```pwsh
$pr = <N>
$repo = git rev-parse --show-toplevel
# Guard against recursion: if the current working tree is already a `*-pr-N` worktree,
# resolve `$repo` to the main worktree before composing the PR worktree path. Otherwise
# nested worktrees like `repo-pr-N-pr-N` get created and orphan the original `.git` file.
if ($repo -match '-pr-\d+$') {
  $repo = (git worktree list --porcelain | Select-String -Pattern '^worktree ' | Select-Object -First 1).Line.Substring(9)
}
$worktree = "$repo-pr-$pr"
if (-not (Test-Path $worktree)) {
  git worktree add --no-checkout $worktree HEAD
  Push-Location $worktree
  gh pr checkout $pr
  Pop-Location
}
Push-Location $worktree
$branch = git branch --show-current
```

## Tear Down

When the caller is done with the worktree and didn't make any changes, remove it and the local branch. Otherwise, instruct
the user how to do this.

```pwsh
Pop-Location
git worktree remove $worktree
git branch -D $branch
```
