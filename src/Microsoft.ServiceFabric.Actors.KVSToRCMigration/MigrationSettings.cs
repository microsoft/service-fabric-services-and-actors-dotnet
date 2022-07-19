// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Microsoft.ServiceFabric.Actors.Generator;

    [DataContract]
    [KnownType(typeof(Actors.Runtime.Migration.MigrationSettings))]
    internal class MigrationSettings : Actors.Runtime.Migration.MigrationSettings
    {
        private static readonly string TraceType = typeof(MigrationSettings).ToString();

        private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MigrationSettings), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        });

        [DataMember]
        public int CopyPhaseParallelism { get; set; }

        [DataMember]
        public long DowntimeThreshold { get; set; }

        [DataMember]
        public int ChunksPerEnumeration { get; set; } = 1;

        [DataMember]
        public long KeyValuePairsPerChunk { get; set; }

        [DataMember]
        public bool EnableDataIntegrityChecks { get; set; }

        // A comma separated list of exception full name(including namespace) for which abort should not be called.
        // A partition error will still be reported
        [DataMember]
        public string ExceptionExclusionListForAbort { get; set; }

        public override string ToString()
        {
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);

                var returnVal = Encoding.UTF8.GetString(stream.GetBuffer());

                return returnVal;
            }
        }

        internal override void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationConfig")
        {
            base.LoadFrom(codePackageActivationContext, configSectionName);

            this.CopyPhaseParallelism = Environment.ProcessorCount;
            this.DowntimeThreshold = 1024;
            this.EnableDataIntegrityChecks = true;
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

                    if (migrationSettings.Parameters.Contains("ChunksPerEnumeration"))
                    {
                        this.ChunksPerEnumeration = int.Parse(migrationSettings.Parameters["ChunksPerEnumeration"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("KeyValuePairsPerChunk"))
                    {
                        this.KeyValuePairsPerChunk = long.Parse(migrationSettings.Parameters["KeyValuePairsPerChunk"].Value);
                    }

                    if (migrationSettings.Parameters.Contains("EnableDataIntegrityChecks"))
                    {
                        this.EnableDataIntegrityChecks = bool.Parse(migrationSettings.Parameters["EnableDataIntegrityChecks"].Value);
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(TraceType, $"Failed to load Migration settings from config package : {e.Message}");
                throw e; // TODO: consider throwing SF Exception.
            }
        }

        internal override void Validate()
        {
            base.Validate();
        }
    }
}
