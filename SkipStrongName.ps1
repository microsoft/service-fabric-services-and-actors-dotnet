#  Register assemblies for strong name verification skipping
#  Public Key Token and assembly name is read from SkipStrongName.json

$registryPath = "HKLM:\SOFTWARE\Microsoft\StrongName\Verification"

$currentDir=split-path $SCRIPT:MyInvocation.MyCommand.Path -parent
$jsonFilePath=[io.path]::combine($currentDir, "SkipStrongName.json")

$jsonContent = Get-Content $jsonFilePath
$data=$jsonContent -join "`n" | ConvertFrom-Json
$publicKeyToken=$data.PublicKeyToken

Write-Host "Registering following assemblies for string naem verification skipping:"
foreach ($assembly in $data.ProductAssemblies)
{
    $assemblyIdentity = "$assembly,$publicKeyToken"
    Write-Host $assemblyIdentity
    New-Item -Path "$registryPath\$assemblyIdentity" -Force | Out-Null
}

foreach ($assembly in $data.TestAssemblies)
{
    $assemblyIdentity = "$assembly,$publicKeyToken"
    Write-Host $assemblyIdentity
    New-Item -Path "$registryPath\$assemblyIdentity" -Force | Out-Null
}

