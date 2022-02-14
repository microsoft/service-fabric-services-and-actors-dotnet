// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Generator;

    internal class MigrationSettings : Actors.Runtime.Migration.MigrationSettings
    {
        private static readonly string TraceType = typeof(MigrationSettings).ToString();

        public int CopyPhaseParallelism { get; set; }

        public long DowntimeThreshold { get; set; }

        public long ItemsPerEnumeration { get; set; }

        public long ItemsPerChunk { get; set; }

        internal override void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationConfig")
        {
            base.LoadFrom(codePackageActivationContext, configSectionName);

            this.CopyPhaseParallelism = Environment.ProcessorCount;
            this.DowntimeThreshold = 1024;

            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                if (configPackageObj.Settings.Sections.Contains(configSectionName))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[configSectionName];
                    if (migrationSettings.Parameters.Contains("CopyPhaseParallelism"))
                    {
                        this.CopyPhaseParallelism = int.Parse(migrationSettings.Parameters["CopyPhaseParallelism"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("DowntimeThreshold"))
                    {
                        this.DowntimeThreshold = int.Parse(migrationSettings.Parameters["DowntimeThreshold"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("ItemsPerEnumeration"))
                    {
                        this.ItemsPerEnumeration = int.Parse(migrationSettings.Parameters["ItemsPerEnumeration"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("ItemsPerChunk"))
                    {
                        this.ItemsPerChunk = int.Parse(migrationSettings.Parameters["ItemsPerChunk"].Value);
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
