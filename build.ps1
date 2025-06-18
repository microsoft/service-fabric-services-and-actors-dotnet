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
	#   BuildCode: Builds code, doesn't generates nuget packages.
    #   GeneratePackages: Generates nuget packages, this target doesn't builds code, build must be done using BuildCode target before invoking this target.
    [ValidateSet('Rebuildall', 'BuildAll', 'BuildCode', 'GeneratePackages')]
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

# msbuild path not provided, find msbuild for VS2022
if($MSBuildFullPath -eq "")
{
    $progFilesPaths = ${env:\ProgramFiles(x86)}, ${env:\ProgramFiles}
    foreach ($progFilesPath in $progFilesPaths)
    {
        $VS2022InstallPath = join-path $progFilesPath "Microsoft Visual Studio\2022"
        $versions = 'Preview', 'Community', 'Professional', 'Enterprise'

        foreach ($version in $versions)
        {
            $VS2022VersionPath = join-path $VS2022InstallPath $version
            $testPath = join-path $VS2022VersionPath "MSBuild\Current\Bin\MSBuild.exe"

            if (Test-Path $testPath)
            {
                $MSBuildFullPath = $testPath
            }
        }
    }
}

if (!(Test-Path $MSBuildFullPath))
{
    throw "Unable to find MSBuild installed on this machine. Please install Visual Studio 2022 or if its installed at non-default location, provide the full path to msbuild using -MSBuildFullPath parameter."
}


Write-Output "Using msbuild from $msbuildFullPath"

$msbuildArgs = @("buildall.proj", "/nr:false", "/nologo", "/t:$target", "/verbosity:$verbosity", "/property:RequestedVerbosity=$verbosity", "/property:Configuration=$configuration", $args)
& $msbuildFullPath $msbuildArgs