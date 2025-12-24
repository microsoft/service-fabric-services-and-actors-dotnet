#  Register assemblies for strong name verification skipping

$registryPath = "HKLM:\SOFTWARE\Microsoft\StrongName\Verification"
$publicKeyToken="31bf3856ad364e35"
$assemblies = # Keep the list sorted with VSCode / Sort Lines Ascending
    "FabActUtil",
    "Microsoft.ServiceFabric.Actors.IntegrationTests",
    "Microsoft.ServiceFabric.Actors.Tests",
    "Microsoft.ServiceFabric.Actors.Wcf.Tests",
    "Microsoft.ServiceFabric.Actors.Wcf",
    "Microsoft.ServiceFabric.Actors",
    "Microsoft.ServiceFabric.AspNetCore.Configuration",
    "Microsoft.ServiceFabric.AspNetCore.HttpSys",
    "Microsoft.ServiceFabric.AspNetCore.Kestrel",
    "Microsoft.ServiceFabric.AspNetCore.Tests",
    "Microsoft.ServiceFabric.AspNetCore",
    "Microsoft.ServiceFabric.Diagnostics.Tests",
    "Microsoft.ServiceFabric.Diagnostics",
    "Microsoft.ServiceFabric.Services.Remoting.Tests",
    "Microsoft.ServiceFabric.Services.Remoting",
    "Microsoft.ServiceFabric.Services.Tests",
    "Microsoft.ServiceFabric.Services.Wcf.Tests",
    "Microsoft.ServiceFabric.Services.Wcf",
    "Microsoft.ServiceFabric.Services",
    "Microsoft.ServiceFabric.TestFramework"

foreach ($assembly in $assemblies)
{
    $assemblyIdentity = "$assembly,$publicKeyToken"
    Write-Host "Strong name verification disabled for $assemblyIdentity"
    New-Item -Path "$registryPath\$assemblyIdentity" -Force | Out-Null
}
