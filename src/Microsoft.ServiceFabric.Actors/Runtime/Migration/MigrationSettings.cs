// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Generator;

    internal class MigrationSettings
    {
        private static readonly string TraceType = typeof(MigrationSettings).ToString();

        public MigrationSettings()
        {
            this.MigrationSourceOrchestrator = "Microsoft.ServiceFabric.Actors.KVSToRCMigration.SourceMigrationOrchestrator, Microsoft.ServiceFabric.Actors.KVSToRCMigration";
            this.MigrationTargetOrchestrator = "Microsoft.ServiceFabric.Actors.KVSToRCMigration.TargetMigrationOrchestrator, Microsoft.ServiceFabric.Actors.KVSToRCMigration";
            this.MigrationMode = MigrationMode.Auto;
        }

        public Uri SourceServiceUri { get; set; }

        public Uri TargetServiceUri { get; set; }

        public MigrationMode MigrationMode { get; set; }

        internal string MigrationSourceOrchestrator { get; set; }

        internal string MigrationTargetOrchestrator { get; set; }

        internal virtual void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationConfig")
        {
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
                    ActorTrace.Source.WriteError(TraceType, $"Section {configSectionName} not found in settings file.");
                    //// TODO: throw
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(TraceType, $"Failed to load Migration settings from config package : {e.Message}");
                throw e; // TODO: conside throwing SF Exception.
            }
        }

        internal virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.SourceServiceUri.ToString()))
            {
                // TODO: throw
            }
        }
    }
}
