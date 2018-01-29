##
#  Builds the source code and generates nuget packages. You can optionally just build the source code by opening individual solutions in Visual Studio.
##

param
(
    # Configuration to build.
	[ValidateSet('Debug', 'Release')]
	[string]$configuration = "Release",	 
	
    # target to build. 
	#Options are:
	#   RebuildAll: Clean, Build all .csproj and Generate Nuget Packages. This is the default option.
	#   BuildAll: Build all .csproj and Generate Nuget Packages.
	#   GeneratePackages: Build all product .csprojs and generate nuget packages.
	#   BuildTests: Builds all test .csprojs.
	#   BuildProduct: Builds all product .csprojs.
	[ValidateSet('Rebuildall', 'BuildAll', 'GeneratePackages')]
	[string]$target = "RebuildAll",

	# msbuild verbosity level.
	[ValidateSet('quiet','minimal', 'normal', 'detailed', 'diagnostic')]
	[string]$verbosity = 'minimal'
)



# Check msbuild exists. Find msbuild for VS2017
$msbuildPath = "MSBuild\14.0\bin\MSBuild.exe"

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
	$msbuildFullPath = join-path $VS2017VersionPath "MSBuild\15.0\Bin\MSBuild.exe"

	if (Test-Path $msbuildFullPath)
	{
		break
	}
}

if (!(Test-Path $msbuildFullPath))
{
	throw "Unable to find MSBuild installed on this machine. Please install it (or install Visual Studio 2017.)"
}


Write-Output "Using msbuild from $msbuildFullPath"

$msbuildArgs = @("buildall.proj", "/nr:false", "/nologo", "/t:$target", "/verbosity:$verbosity", "/property:RequestedVerbosity=$verbosity", "/property:Configuration=$configuration", $args)
& $msbuildFullPath $msbuildArgs