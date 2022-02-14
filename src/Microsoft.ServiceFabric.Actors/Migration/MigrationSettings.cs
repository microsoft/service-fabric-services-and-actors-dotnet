// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Generator;

    internal class MigrationSettings
    {
        private static readonly string TraceType = typeof(MigrationSettings).ToString();

        public MigrationSettings()
        {
            this.MigrationSourceOrchestratorNameSpace = "Microsoft.ServiceFabric.Actors.KVSToRCMigration,Microsoft.ServiceFabric.Actors.KVSToRCMigration.SourceMigrationOrchestrator";
            this.MigrationTargetOrchestratorNameSpace = "Microsoft.ServiceFabric.Actors.KVSToRCMigration,Microsoft.ServiceFabric.Actors.KVSToRCMigration.TargetMigrationOrchestrator";
        }

        public Uri SourceServiceUri { get; set; }

        public Uri TargetServiceUri { get; set; }

        public MigrationMode MigrationMode { get; set; }

        internal string MigrationSourceOrchestratorNameSpace { get; set; }

        internal string MigrationTargetOrchestratorNameSpace { get; set; }

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
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(TraceType, $"Failed to load Migration settings from config package : {e.Message}");
                throw e; // TODO: conside throwing SF Exception.
            }
        }
    }
}
