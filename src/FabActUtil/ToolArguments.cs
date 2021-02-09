// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace FabActUtil
{
    using FabActUtil.CommandLineParser;
    using Microsoft.ServiceFabric.Actors.Generator;

    /// <summary>
    /// Output Target for FabActUtil
    /// </summary>
    public enum OutputTarget
    {
        /// <summary>
        /// Output target is manifest generation.
        /// </summary>
        Manifest,
    }

    internal class ToolArguments
    {
#pragma warning disable SA1401 // Fields should be private
        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The input assembly containing the compiled actor definitions. This argument is required for 'Manifest' target.",
            LongName = "in",
            ShortName = "i")]
        public string InputAssembly;

        [CommandLineArgument(
            CommandLineArgumentType.Multiple,
            Description = "The type of the actor interface for which to generate the output. By default the output will be generated for all actors in the input assembly.",
            LongName = "actor",
            ShortName = "a")]
        public string[] Actors;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "If true recurse to find actor types in all assemblies referenced by input assembly. By default this is false.",
            LongName = "recurse",
            ShortName = "r")]
        public bool Recurse;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The target output for the tool, by default tool generates Manifest.",
            LongName = "target",
            ShortName = "t")]
        public OutputTarget Target;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The output directory to create files in. The default is the current directory.",
            LongName = "out",
            ShortName = "o")]
        public string OutputPath;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The application package directory to create/modify application manifest file in. The default is under output directory.",
            LongName = "applicationPackagePath",
            ShortName = "app")]
        public string ApplicationPackagePath;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The service package directory to create/modify service manifest files in. The default is under output directory.",
            LongName = "servicePackagePath",
            ShortName = "spp")]
        public string ServicePackagePath;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Optional prefix for the Service Fabric Application Package, Application Type and Application Name.",
            LongName = "applicationPrefix",
            ShortName = "ap")]
        public string ApplicationPrefix;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Optional prefix for the Service Fabric Service Package.",
            LongName = "servicePackagePrefix",
            ShortName = "sp")]
        public string ServicePackagePrefix;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "The version to use in the manifests. If not specified, this will be the version of the assembly.",
            LongName = "version",
            ShortName = "ver")]
        public string Version;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Path to application parameter file for five node cluster used for deployment by Visual Studio. It contains app name and parameter values.",
            LongName = "Local5NodeAppParametersFile",
            ShortName = "local5nodeappparamfile")]
        public string Local5NodeAppParamFile;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Path to application parameter file for one node cluster used for deployment by Visual Studio. It contains app name and parameter values.",
            LongName = "Local1NodeAppParametersFile",
            ShortName = "local1nodeappparamfile")]
        public string Local1NodeAppParamFile;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Path to folder containing required dependencies.",
            LongName = "AssemblyResolvePath",
            ShortName = "arp")]
        public string AssemblyResolvePath;

        [CommandLineArgument(
            CommandLineArgumentType.AtMostOnce,
            Description = "Type of entryPoint to be generated in the program value of the service manifest. 'Exe' (default) for ServiceName.exe, 'NoExtension' for ServiceName, 'ExternalExecutable' for using ServiceName.dll in the argument for the program 'dotnet' used for Framework dependent deployment",
            LongName = "ServiceManifestEntryPointType",
            ShortName = "smep")]
        public string ServiceManifestEntryPointType;
#pragma warning restore SA1401 // Fields should be private

        public ToolArguments()
        {
            this.InputAssembly = null;
            this.Recurse = false;
            this.Actors = null;
            this.Target = OutputTarget.Manifest;
            this.OutputPath = null;
            this.ApplicationPackagePath = null;
            this.ServicePackagePath = null;
            this.ApplicationPrefix = null;
            this.ServicePackagePrefix = null;
            this.Version = null;
            this.Local5NodeAppParamFile = null;
            this.Local1NodeAppParamFile = null;
            this.AssemblyResolvePath = null;
            this.ServiceManifestEntryPointType = "Exe";
        }

        internal bool IsValid()
        {
            if (this.Target == OutputTarget.Manifest)
            {
                return (!string.IsNullOrEmpty(this.InputAssembly));
            }

            return false;
        }
    }
}
