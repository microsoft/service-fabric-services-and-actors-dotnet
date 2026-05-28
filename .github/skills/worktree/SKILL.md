---
name: worktree
description: |
  Create, reuse or clean up a dedicated git worktree for working on a pull request or a private branch.
  Use from any PR-scoped workflow that needs an isolated checkout to keep current code and build output untouched.
argument-hint: "PR #N"
---

## Set Up

Reuse the existing worktree if present, otherwise create one. Run subsequent steps from the worktree directory.

```pwsh
$pr = <N>
$repo = git rev-parse --show-toplevel
# Guard against recursion if the current working tree is already a worktree under `<repo>.worktrees`
if ((Split-Path (Split-Path $repo -Parent) -Leaf) -match '\.worktrees$') {
  $repo = (git worktree list --porcelain | Select-String -Pattern '^worktree ' | Select-Object -First 1).Line.Substring(9)
}
$branch = gh pr view $pr --json headRefName -q .headRefName
$folder = $branch -replace '[^A-Za-z0-9._-]', '-'
$worktree = Join-Path "$repo.worktrees" $folder
if (-not (Test-Path $worktree)) {
  git worktree add --no-checkout $worktree HEAD
  Push-Location $worktree
  gh pr checkout $pr
  Pop-Location
}
Push-Location $worktree
```

## Tear Down

When the caller is done with the worktree and didn't make any changes, remove it and the local branch. Otherwise, instruct
the user how to do this.

```pwsh
Pop-Location
git worktree remove $worktree
git branch -D $branch
```
