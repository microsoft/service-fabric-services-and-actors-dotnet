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
    [string]$Verbosity = 'minimal'
)

$msbuildArgs = @("buildall.proj", "/nr:false", "/nologo", "/t:$target", "/verbosity:$verbosity", "/property:RequestedVerbosity=$verbosity", "/property:Configuration=$configuration", $args)
dotnet msbuild $msbuildArgs