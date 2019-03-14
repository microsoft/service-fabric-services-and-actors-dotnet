param
(
    [Parameter(Mandatory=$True, Position=1)]
    [string] $RuntimeConfigFile = ""
)

if(!(Test-Path $RuntimeConfigFile))
{
  Write-Host "File doesn't exist:" $RuntimeConfigFile
}

$jsonData = Get-Content $RuntimeConfigFile | ConvertFrom-Json
$runtimeVersionList = dotnet --list-runtimes | Where-Object { $_ -match 'Microsoft.NETCore.App' }; 
if($runtimeVersionList -ne $null -and $runtimeVersionList.Length -gt 0)
{
  $runtimeInfo = $runtimeVersionList[$runtimeVersionList.Length - 1].trim().Split(''); 
  if($runtimeInfo.Length -gt 1)
  {
    $name = $runtimeInfo[0]; 
    $version = $runtimeInfo[1]

    if(($name.Split('.')).Length -eq 3 -and ($version.Split('.')).Length -eq 3)
    {
      $tfm = $name.Split('.')[1].toLower() + $name.Split('.')[2].toLower() + $version.Split('.')[0] + "." + $version.Split('.')[1];

      $updated = $false

      #Adding tfm property
      if(!(Get-Member -inputobject $jsonData.runtimeOptions -name "tfm" -Membertype NoteProperty))
      {
        $updated = $true
        $jsonData.runtimeOptions | Add-Member -Name "tfm" -Value $tfm -MemberType NoteProperty
      }

      #Adding framework property
      if(!(Get-Member -inputobject $jsonData.runtimeOptions -name "framework" -Membertype NoteProperty))
      {
        $updated = $true

$jsonDatamInfo = @"
{
"name":"$name",
"version":"$version"
}
"@
        $jsonData.runtimeOptions | Add-Member -Name "framework" -Value (ConvertFrom-Json $jsonDatamInfo) -MemberType NoteProperty
      }

      if($updated -eq $true)
      {
        $jsonData | ConvertTo-Json -depth 100 | foreach-object {$_ -replace "(?m)  (?<=^(?:  )*)", " " } | Set-Content $RuntimeConfigFile -Force
      }
    }
  }
}
else
{
  Write-Host "No installed .NETCore runtime version found."
}