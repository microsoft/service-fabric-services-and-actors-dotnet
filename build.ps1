##
#  Builds the source code and generates nuget packages. You can optionally just build the source code by opening individual solutions in Visual Studio.
##

param
(
    # Configuration to build.
	[ValidateSet('Debug', 'Release')]
	[string]$configuration = "Release",	 
	
    # target to build. 
	#Options are: RebuildAll: Clean, Build all and Generate Nuget Packages. BuildAll: Build all csproj. GeneratePackages: Build all csproj and generate nuget packages.
	[ValidateSet('Rebuildall', 'BuildAll', 'CleanAll', 'GeneratePackages')]
	[string]$target = "RebuildAll",

	# msbuild verbosity level.
	[ValidateSet('quiet','minimal', 'normal', 'detailed', 'diagnostic')]
	[string]$verbosity = 'minimal'
)

# Check msbuild exists
$msbuildRelativePath = "MSBuild\14.0\bin\MSBuild.exe"

if (Test-Path "env:\ProgramFiles(x86)") 
{
    $msbuildPath = join-path ${env:ProgramFiles(x86)} $msbuildRelativePath
}
elseif (Test-Path "env:\ProgramFiles") 
{
	$msbuildPath = join-path ${env:ProgramFiles(x86)} $msbuildRelativePath
}

if (!(Test-Path $msbuildPath))
{
    throw "Unable to find MSBuild v14 installed on this machine. Please install it (or install Visual Studio 2015.)"
}

$msbuildArgs = @("buildall.proj", "/nr:false", "/nologo", "/t:$target", "/verbosity:$verbosity", "/property:RequestedVerbosity=$verbosity", "/property:Configuration=$configuration", $args)
& $msbuildPath $msbuildArgs