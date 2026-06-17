$assemblies = "Microsoft.ServiceFabric.Data.Interfaces",
              "Microsoft.ServiceFabric.Data.Interfaces.V2"

if (-not $env:DevEnvDir)
{
    & "${env:ProgramFiles}\Microsoft Visual Studio\18\Enterprise\Common7\Tools\Launch-VsDevShell.ps1" -SkipAutomaticLocation
}

foreach ($assembly in $assemblies)
{
    gacutil.exe /u $assembly
}