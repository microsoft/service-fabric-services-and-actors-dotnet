// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;

    [DataContract]
    internal class MigrationSettings
    {
        private static readonly string TraceType = typeof(MigrationSettings).ToString();

        public MigrationSettings()
        {
            this.MigrationSourceOrchestrator = "Microsoft.ServiceFabric.Actors.KVSToRCMigration.SourceMigrationOrchestrator, Microsoft.ServiceFabric.Actors.KVSToRCMigration";
            this.MigrationTargetOrchestrator = "Microsoft.ServiceFabric.Actors.KVSToRCMigration.TargetMigrationOrchestrator, Microsoft.ServiceFabric.Actors.KVSToRCMigration";
            this.MigrationMode = MigrationMode.Auto;
        }

        [DataMember]
        public Uri SourceServiceUri { get; set; }

        [DataMember]
        public Uri TargetServiceUri { get; set; }

        [DataMember]
        public MigrationMode MigrationMode { get; set; }

        [DataMember]
        internal string MigrationSourceOrchestrator { get; set; }

        [DataMember]
        internal string MigrationTargetOrchestrator { get; set; }

        internal string MigrationConfigSectionName { get; private set; }

        internal virtual void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationConfig")
        {
            this.MigrationConfigSectionName = configSectionName;
            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                if (configPackageObj.Settings.Sections.Contains(configSectionName))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[configSectionName];
                    if (migrationSettings.Parameters.Contains("SourceServiceUri"))
                    {
                        this.SourceServiceUri = new Uri(migrationSettings.Parameters["SourceServiceUri"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("TargetServiceUri"))
                    {
                        this.TargetServiceUri = new Uri(migrationSettings.Parameters["TargetServiceUri"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("MigrationMode"))
                    {
                        this.MigrationMode = (MigrationMode)Enum.Parse(typeof(MigrationMode), migrationSettings.Parameters["MigrationMode"].Value);
                    }
                }
                else
                {
                    var errorMsg = $"Section {configSectionName} not found in settings file.";
                    ActorTrace.Source.WriteError(TraceType, errorMsg);

                    throw new InvalidMigrationConfigException(errorMsg);
                }
            }
            catch (Exception e)
            {
                var errorMsg = $"Section {configSectionName} not found in settings file.";
                ActorTrace.Source.WriteError(TraceType, errorMsg);

                throw new InvalidMigrationConfigException(errorMsg, e);
            }
        }

        internal virtual void Validate(bool isSource)
        {
            if (!isSource && (this.SourceServiceUri == null || !this.SourceServiceUri.AbsoluteUri.StartsWith("fabric:/", StringComparison.InvariantCultureIgnoreCase)))
            {
                var errorMsg = $"SourceServiceUri is either null or valid. Add SourceServiceUri section in {this.MigrationConfigSectionName} section of settings file.";
                ActorTrace.Source.WriteError(TraceType, errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
            }

            if (isSource && this.TargetServiceUri != null && !this.TargetServiceUri.AbsoluteUri.StartsWith("fabric:/", StringComparison.InvariantCultureIgnoreCase))
            {
                var errorMsg = $"TargetServiceUri is valid.";
                ActorTrace.Source.WriteError(TraceType, errorMsg);

                throw new InvalidMigrationConfigException(errorMsg);
            }
        }
    }
}
