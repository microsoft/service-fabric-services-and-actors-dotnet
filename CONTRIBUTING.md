## Contributing code and content
We welcome all forms of contributions from the community. Please read the following guidelines to maximize the chances of your PR being merged.

### Reporting security issues and bugs
Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC)  secure@microsoft.com. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://technet.microsoft.com/en-us/security/ff852094.aspx).

### Other discussions
For general "how-to" and guidance questions about using Service Fabric to build and run applications, please use [Stack Overflow](http://stackoverflow.com/questions/tagged/azure-service-fabric) tagged with `azure-service-fabric`.

### Development process
Please be sure to follow the usual process for submitting PRs:

 - Fork the repo
 - Create a pull request into correct branch (see branching information below).
 - Make sure your PR title is descriptive
 - Include a link back to an open issue in the PR description

We reserve the right to close PRs that are not making progress. If no changes are made for 7 days, we'll close the PR. Closed PRs can be reopened again later and work can resume.

### <a name="BranchingInformation"></a>Branching Information
All development for future releases happen in the develop branch.
A new branch is forked off of develop branch for each release to stabilize it before final release. (eg. release_3.1 branch represents the 3.1.* release).
A bug fix in an already released version is made both to its release branch and to develop branch so that its available in refresh of the release and for future new releases.

### Contributor License Agreement
Before you submit a pull request, a bot will prompt you to sign the [Microsoft Contributor License Agreement](https://cla.microsoft.com/). This needs to be done only once for any Microsoft-sponsored open source project - if you've signed the Microsoft CLA for any project sponsored by Microsoft, then you are good to go for all the repos sponsored by Microsoft.

 > **Important**: Note that there are **two** different CLAs commonly used by Microsoft projects: [Microsoft CLA](https://cla.microsoft.com/) and [.NET Foundation CLA](https://cla2.dotnetfoundation.org/). Service Fabric open source projects use the [Microsoft](https://cla.microsoft.com/) CLA. The .NET Foundation is treated as a separate entity, so if you've signed the .NET Foundation CLA in the past, you will still need to sign the Microsoft CLA to contribute to Service Fabric open source projects.
