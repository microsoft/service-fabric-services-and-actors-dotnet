: Run as admin to configure local development pre-requisites

winget install Microsoft.DotNet.AspNetCore.8
winget install Microsoft.DotNet.AspNetCore.9
winget install Microsoft.DotNet.AspNetCore.10
winget install Microsoft.DotNet.Runtime.8
winget install Microsoft.DotNet.Runtime.9
winget install Microsoft.DotNet.Runtime.10
winget install Microsoft.DotNet.SDK.10

pwsh SkipStrongName.ps1
