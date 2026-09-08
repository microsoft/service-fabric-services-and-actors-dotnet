# Contributing

## Clone
```
git clone https://github.com/microsoft/service-fabric-dotnet.git
cd ./service-fabric-dotnet
```

## Install pre-requisites

### Windows
```
sudo ./init.cmd
```

### Ubuntu
```
chmod +x ./init.sh
sudo ./init.sh
```

## Overview, Build and Test

See [AGENTS.md](./AGENTS.md).

## Pull Requests

### Service Fabric Engineers
- Find an existing or submit a new [work item](https://dev.azure.com/msazure/One/_backlogs/backlog/Service%20Fabric%20Programming%20Model/Backlog%20items)
  to discuss your idea with us first.
- Join the [service-fabric-write](https://repos.opensource.microsoft.com/orgs/microsoft/teams/service-fabric-write) team.
- Create a branch called `user/{youraccount}/{youproposal}` in your local clone.
- Ask Copilot `/review with multiple models` of your changes.

### External Contributors
- Find an existing or submit a new [issue](https://github.com/microsoft/service-fabric-dotnet/issues) to discuss your idea with us first.
- Sign the [Microsoft Contributor License Agreement](https://cla.microsoft.com/).
- [Fork](https://docs.github.com/articles/fork-a-repo) this repository and implement your proposal in the fork.

### All Contributors
- Create a [draft pull request](https://docs.github.com/articles/creating-a-pull-request).
- Follow the [git commit guidelines](https://cbea.ms/git-commit) to author the PR title and description.
  - Write for the person reading history of the _target_ branch in the future.
  - Describe _what_ it changes and _why_, not _how_ it was developed or which prior PRs it addresses.
  - Title should use the imperative mood and be limited to 72 characters.
  - Don't include textual tags `[MyFeature]` in the title.
- Make sure the validation build completes successfully.
- Link the pull request from the work item/issue you've created.
- Publish the pull request and address the Copilot review feedback.
- Resolve review comment threads after addressing the feedback.
- Tag the person you were discussing your proposal to review the PR.

### Maintainers
- Ask Copilot `review PR {N} with multiple models and post feedback to GitHub`.
- Re-open comment threads if the feedback was not addressed adequately.
- Merge the pull request after approving it.
