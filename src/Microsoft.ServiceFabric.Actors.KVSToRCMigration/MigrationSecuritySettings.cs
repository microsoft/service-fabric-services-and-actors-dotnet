// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;

    [DataContract]
    internal class MigrationSecuritySettings
    {
        private static readonly string TraceType = typeof(MigrationSecuritySettings).ToString();

        private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MigrationSecuritySettings), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        });

        // TODO: Add support for more Credential Types
        [DataMember]
        public string SecurityCredentialsType { get; set; }

        [DataMember]
        public X509FindType CertificateFindType { get; set; }

        [DataMember]
        public string CertificateFindValue { get; set; }

        [DataMember]
        public string CertificateRemoteThumbprints { get; set; }

        [DataMember]
        public string CertificateRemoteCommonNames { get; set; }

        [DataMember]
        public StoreLocation CertificateStoreLocation { get; set; }

        [DataMember]
        public string CertificateStoreName { get; set; }

        [DataMember]
        public string CertificateProtectionLevel { get; set; }

        internal void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationSecurityConfig")
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                if (configPackageObj.Settings.Sections.Contains(configSectionName))
                {
                    var migrationSecuritySettings = configPackageObj.Settings.Sections[configSectionName];
                    if (migrationSecuritySettings.Parameters.Contains("SecurityCredentialsType"))
                    {
                        this.SecurityCredentialsType = migrationSecuritySettings.Parameters["SecurityCredentialsType"].Value.Trim();
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateFindType"))
                    {
                        this.CertificateFindType = (X509FindType)Enum.Parse(typeof(X509FindType), migrationSecuritySettings.Parameters["CertificateFindType"].Value.Trim());
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateFindValue"))
                    {
                        this.CertificateFindValue = migrationSecuritySettings.Parameters["CertificateFindValue"].Value.Trim();
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateRemoteThumbprints"))
                    {
                        this.CertificateRemoteThumbprints = migrationSecuritySettings.Parameters["CertificateRemoteThumbprints"].Value.Trim();
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateRemoteCommonNames"))
                    {
                        this.CertificateRemoteCommonNames = migrationSecuritySettings.Parameters["CertificateRemoteCommonNames"].Value.Trim();
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateStoreLocation"))
                    {
                        this.CertificateStoreLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), migrationSecuritySettings.Parameters["CertificateStoreLocation"].Value.Trim());
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateStoreName"))
                    {
                        this.CertificateStoreName = migrationSecuritySettings.Parameters["CertificateStoreName"].Value.Trim();
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateProtectionLevel"))
                    {
                        this.CertificateProtectionLevel = migrationSecuritySettings.Parameters["CertificateProtectionLevel"].Value.Trim();
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(TraceType, $"Failed to load Migration security settings from config package : {e.Message}");
                throw e; // TODO: consider throwing SF Exception.
            }
        }

        internal void Validate()
        {
            // TODO: Validate thubmprint formats
            if (string.IsNullOrWhiteSpace(this.SecurityCredentialsType))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.CertificateFindValue))
            {
                throw new InvalidMigrationConfigException($"CertificateFindValue cannot be empty for {TraceType}");
            }
        }
    }
}
