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
        private static string traceType = typeof(MigrationSettings).ToString();

        public int CopyPhaseParallelism { get; set; }

        public Uri KVSActorServiceUri { get; set; }

        public long DowntimeThreshold { get; set; }

        public long ItemsPerEnumeration { get; set; }

        public long ItemsPerChunk { get; set; }

        public static MigrationSettings LoadFrom(CodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationConfig")
        {
            MigrationSettings temp = new MigrationSettings();
            temp.CopyPhaseParallelism = Environment.ProcessorCount;
            temp.DowntimeThreshold = 1024;

            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                if (configPackageObj.Settings.Sections.Contains(configSectionName))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[configSectionName];
                    if (migrationSettings.Parameters.Contains("CopyPhaseParallelism"))
                    {
                        temp.CopyPhaseParallelism = int.Parse(migrationSettings.Parameters["CopyPhaseParallelism"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("KVSActorServiceUri"))
                    {
                        temp.KVSActorServiceUri = new Uri(migrationSettings.Parameters["KVSActorServiceUri"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("DowntimeThreshold"))
                    {
                        temp.DowntimeThreshold = int.Parse(migrationSettings.Parameters["DowntimeThreshold"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("ItemsPerEnumeration"))
                    {
                        temp.ItemsPerEnumeration = int.Parse(migrationSettings.Parameters["ItemsPerEnumeration"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("ItemsPerChunk"))
                    {
                        temp.ItemsPerChunk = int.Parse(migrationSettings.Parameters["ItemsPerChunk"].Value);
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(traceType, $"Failed to load Migration settings from config package : {e.Message}");
                throw e; // TODO: conside throwing SF Exception.
            }

            return temp;
        }
    }
}
