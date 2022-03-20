// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.ServiceFabric.Actors.Generator;

    internal class MigrationSecuritySettings
    {
        private static readonly string TraceType = typeof(MigrationSecuritySettings).ToString();

        public string SecurityCredentialsType { get; set; }

        public X509FindType CertificateFindType { get; set; }

        public string CertificateFindValue { get; set; }

        public string CertificateRemoteThumbprints { get; set; }

        public string CertificateRemoteCommonNames { get; set; }

        public StoreLocation CertificateStoreLocation { get; set; }

        public string CertificateStoreName { get; set; }

        public string CertificateProtectionLevel { get; set; }

        internal void LoadFrom(ICodePackageActivationContext codePackageActivationContext, string configSectionName = "MigrationSecurityConfig")
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName();
            var transportSectionName = configSectionName.Replace("MigrationSecurityConfig", "TransportSettings");
            try
            {
                var configPackageObj = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                if (configPackageObj.Settings.Sections.Contains(configSectionName))
                {
                    var migrationSecuritySettings = configPackageObj.Settings.Sections[configSectionName];
                    if (migrationSecuritySettings.Parameters.Contains("SecurityCredentialsType"))
                    {
                        this.SecurityCredentialsType = migrationSecuritySettings.Parameters["SecurityCredentialsType"].Value;
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateFindType"))
                    {
                        this.CertificateFindType = (X509FindType)Enum.Parse(typeof(X509FindType), migrationSecuritySettings.Parameters["CertificateFindType"].Value);
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateFindValue"))
                    {
                        this.CertificateFindValue = migrationSecuritySettings.Parameters["CertificateFindValue"].Value;
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateRemoteThumbprints"))
                    {
                        this.CertificateRemoteThumbprints = migrationSecuritySettings.Parameters["CertificateRemoteThumbprints"].Value;
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateRemoteCommonNames"))
                    {
                        this.CertificateRemoteCommonNames = migrationSecuritySettings.Parameters["CertificateRemoteCommonNames"].Value;
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateStoreLocation"))
                    {
                        this.CertificateStoreLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), migrationSecuritySettings.Parameters["CertificateStoreLocation"].Value);
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateStoreName"))
                    {
                        this.CertificateStoreName = migrationSecuritySettings.Parameters["CertificateStoreName"].Value;
                    }

                    if (migrationSecuritySettings.Parameters.Contains("CertificateProtectionLevel"))
                    {
                        this.CertificateProtectionLevel = migrationSecuritySettings.Parameters["CertificateProtectionLevel"].Value;
                    }
                }
                else if (configPackageObj.Settings.Sections.Contains(transportSectionName))
                {
                    ActorTrace.Source.WriteError(TraceType, $"Could not find Migration security settings from config package. Trying to load security settings from Transport Settings.");
                    this.LoadFrom(codePackageActivationContext, transportSectionName);
                }
                else
                {
                    ActorTrace.Source.WriteError(TraceType, $"Could not find neither Migration security settings nor TransportSettings from config package.");
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError(TraceType, $"Failed to load Migration security settings from config package : {e.Message}");
                throw e; // TODO: conside throwing SF Exception.
            }
        }

        internal void Validate()
        {
            // TODO: Validate thubmprint formats
        }
    }
}
