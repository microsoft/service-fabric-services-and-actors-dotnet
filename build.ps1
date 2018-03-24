##
#  Builds the source code and generates nuget packages. You can optionally just build the source code by opening individual solutions in Visual Studio.
##

param
(
    # Configuration to build.
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = "Release",

    # target to build. 
    #Options are:
    #   RebuildAll: Clean, Build all .csproj and Generate Nuget Packages. This is the default option.
    #   BuildAll: Build all .csproj and Generate Nuget Packages.
    #   GeneratePackages: Build all product .csprojs and generate nuget packages.
    #   BuildTests: Builds all test .csprojs.
    #   BuildProduct: Builds all product .csprojs.
    [ValidateSet('Rebuildall', 'BuildAll', 'GeneratePackages')]
    [string]$Target = "RebuildAll",

    # msbuild verbosity level.
    [ValidateSet('quiet','minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',

    # path to msbuild
    [string]$MSBuildFullPath
)

if($MSBuildFullPath -ne "")
{
    if (!(Test-Path $MSBuildFullPath))
    {
        throw "Unable to find MSBuild at the specified path, run the script again with correct path to msbuild."
    }
}

# msbuild path not provided, find msbuild for VS2017
if($MSBuildFullPath -eq "")
{
    if (Test-Path "env:\ProgramFiles(x86)")
    {
        $progFilesPath =  ${env:ProgramFiles(x86)}
    }
    elseif (Test-Path "env:\ProgramFiles")
    {
        $progFilesPath =  ${env:ProgramFiles}
    }

    $VS2017InstallPath = join-path $progFilesPath "Microsoft Visual Studio\2017"
    $versions = 'Community', 'Professional', 'Enterprise'

    foreach ($version in $versions)
    {
        $VS2017VersionPath = join-path $VS2017InstallPath $version
        $MSBuildFullPath = join-path $VS2017VersionPath "MSBuild\15.0\Bin\MSBuild.exe"

        if (Test-Path $MSBuildFullPath)
        {
            break
        }
    }

    if (!(Test-Path $MSBuildFullPath))
    {
        Write-Host "Visual Studio 2017 installation not found in ProgramFiles, trying to find install path from registry."
        if(Test-Path -Path HKLM:\SOFTWARE\WOW6432Node)
        {
            $VS2017VersionPath = Get-ItemProperty (Get-ItemProperty -Path HKLM:\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7 -Name "15.0")."15.0"
        }
        else
        {
            $VS2017VersionPath = Get-ItemProperty (Get-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\VisualStudio\SxS\VS7 -Name "15.0")."15.0"
        }

        $MSBuildFullPath = join-path $VS2017VersionPath "MSBuild\15.0\Bin\MSBuild.exe"
    }
}

if (!(Test-Path $MSBuildFullPath))
{
    throw "Unable to find MSBuild installed on this machine. Please install Visual Studio 2017 or if its installed at non-default location, provide the full ppath to msbuild using -MSBuildFullPath parameter."
}


Write-Output "Using msbuild from $msbuildFullPath"

$msbuildArgs = @("buildall.proj", "/nr:false", "/nologo", "/t:$target", "/verbosity:$verbosity", "/property:RequestedVerbosity=$verbosity", "/property:Configuration=$configuration", $args)
& $msbuildFullPath $msbuildArgs