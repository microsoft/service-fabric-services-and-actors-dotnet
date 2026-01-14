// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Client.Http;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Powershell.Http
{
    /// <summary>
    /// Cmdlet to connect to Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "SFCluster", DefaultParameterSetName = "Default")]
    public class ConnectClusterCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Service Faric cluster http gateway endpoint address.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "Default")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Windows")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Aad")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Dsts")]
        public string[] ConnectionEndpoint { get; set; }

        /// <summary>
        /// Gets or sets switch parameter for selecting Windows credentials when connecting to Service Fabric cluster secured with Windows credentials.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Windows")]
        public SwitchParameter WindowsCredential { get; set; }

        /// <summary>
        /// Gets or sets switch parameter for slecting X509 Credentials when connecting to Service Fabric Cluster secured with X509 certificate.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        public SwitchParameter X509Credential { get; set; }

        /// <summary>
        /// Gets or sets switch parameter for slecting Azure Active Directory credentials when connecting to Service Fabric Cluster secured with Azure Active Directory..
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Aad")]
        public SwitchParameter AzureActiveDirectory { get; set; }

        /// <summary>
        /// Gets or sets switch parameter for slecting DSTS credentials. This parameter is for internal use only.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Dsts")]
        public SwitchParameter DSTS { get; set; }

        /// <summary>
        /// Gets or sets Subject common names or DNS names of X509 certificate of the server.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        [Parameter(Mandatory = false, ParameterSetName = "Dsts")]
        [Parameter(Mandatory = false, ParameterSetName = "Aad")]
        public string[] ServerCommonName { get; set; }

        /// <summary>
        /// Gets or sets Issuer Cert thumbprints for X509 certificate of the server. When provided alongwith ServerCommonName, its used to validate server cert's issuer thumbprint.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        [Parameter(Mandatory = false, ParameterSetName = "Dsts")]
        [Parameter(Mandatory = false, ParameterSetName = "Aad")]
        public string[] IssuerCertThumbprints { get; set; }

        /// <summary>
        /// Gets or sets thumbprint of X509 certificate of the server.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        [Parameter(Mandatory = false, ParameterSetName = "Dsts")]
        [Parameter(Mandatory = false, ParameterSetName = "Aad")]
        public string[] ServerCertThumbprint { get; set; }

        /// <summary>
        /// Gets or sets client timeout in seconds used by underlying http client to wait before the request times out. The default value for
        /// this parameter is 90 seconds.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        [Parameter(Mandatory = true,  ParameterSetName = "Windows")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_FindClientCert")]
        [Parameter(Mandatory = false, ParameterSetName = "X509_ClientCertProvided")]
        [Parameter(Mandatory = true,  ParameterSetName = "Aad")]
        [Parameter(Mandatory = true,  ParameterSetName = "Dsts")]
        public long? ClientTimeout { get; set; } = 90;

        /// <summary>
        /// Gets or sets the switch to get Azure Active Directory metadata. When this switch is specified, Connect commandlet displays the Azure aCtive Directory metadata without validating the connection with AAD credentials.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Aad")]
        public SwitchParameter GetMetadata { get; set; }

        /// <summary>
        /// Gets or sets the client certificate.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "X509_ClientCertProvided")]
        public X509Certificate2 ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="X509FindType"/> to find the client certificate.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "X509_FindClientCert")]
        public X509FindType FindType { get; set; }

        /// <summary>
        /// Gets or sets the FindValue to find the client certificate.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "X509_FindClientCert")]
        public string FindValue { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StoreLocation"/> to find the client certificate.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "X509_FindClientCert")]
        public StoreLocation StoreLocation { get; set; }

        /// <summary>
        /// Gets or sets the StoreName to find the client certificate.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "X509_FindClientCert")]
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the Azure Active Directory Security Token.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Aad")]
        public string SecurityToken { get; set; }

        /// <inheritdoc />
        protected override void BeginProcessing()
        {
            if (ConnectionEndpoint != null)
            {
                foreach (string endPoint in ConnectionEndpoint)
                {
                    if (!endPoint.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !Uri.TryCreate(endPoint, UriKind.Absolute, out var uri))
                        throw new PSArgumentException(Resource.ErrorIncorrectEndpointFormat);
                }
            }
        }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            if (ConnectionEndpoint == null && !TryConfiguringDefaultConnect())
                throw new InvalidOperationException("Cluster Connection Information is not provided. Please try connecting again with correct connection information.");

            var builder = new ServiceFabricClientBuilder()
                .UseEndpoints(ConnectionEndpoint.Select(e => new Uri(e)).ToArray());

            // Configure Security
            if (WindowsCredential.IsPresent)
                builder.UseWindowsSecurity();
            else if (X509Credential.IsPresent)
            {
                var remoteX509SecuritySettings = GetServerX509SecuritySettings();
                Func<CancellationToken, Task<SecuritySettings>> securitySettings;
                if (ClientCertificate == null)
                {
                    X509Certificate2 clientCert = CredentialsUtil.GetCertificate(StoreLocation, StoreName, FindValue, FindType);
                    if (clientCert == null)
                        throw new PSInvalidOperationException(Resource.ErrorLoadingClientCertificate);
                    securitySettings = (ct) => Task.FromResult<SecuritySettings>(new X509SecuritySettings(clientCert, remoteX509SecuritySettings));
                }
                else
                    securitySettings = (ct) => Task.FromResult<SecuritySettings>(new X509SecuritySettings(ClientCertificate, remoteX509SecuritySettings));
                builder.UseX509Security(securitySettings);
            }
            else if (AzureActiveDirectory.IsPresent)
            {
                RemoteX509SecuritySettings remoteX509SecuritySettings = GetServerX509SecuritySettings();
                Func<CancellationToken, Task<SecuritySettings>> securitySettings;
                if (GetMetadata.IsPresent)
                    securitySettings = (ct) => Task.FromResult<SecuritySettings>(new AzureActiveDirectorySecuritySettings("DummyTokenToGetMetadata", remoteX509SecuritySettings));
                else
                {
                    if (SecurityToken != null)
                        securitySettings = (ct) => Task.FromResult<SecuritySettings>(new AzureActiveDirectorySecuritySettings(SecurityToken, remoteX509SecuritySettings));
                    else
                        securitySettings = (ct) => Task.FromResult<SecuritySettings>(new AzureActiveDirectorySecuritySettings(CredentialsUtil.GetAccessTokenAsync, remoteX509SecuritySettings));
                }

                builder.UseAzureActiveDirectorySecurity(securitySettings);
            }
            else if (DSTS.IsPresent)
            {
                var remoteX509SecuritySettings = GetServerX509SecuritySettings();
                Func<CancellationToken, Task<SecuritySettings>> securitySettings =
                    (ct) => Task.FromResult<SecuritySettings>(new DstsClaimsSecuritySettings(CredentialsUtil.GetAccessTokenDstsAsync, remoteX509SecuritySettings));
                builder.UseClaimsSecurity(securitySettings);
            }

            if (ClientTimeout != null)
                builder.ConfigureClientSettings(settings => settings.ClientTimeout = TimeSpan.FromSeconds(ClientTimeout.Value));

            ServiceFabricHttpClient client = builder.BuildAsync(cancellationToken: CancellationToken).GetAwaiter().GetResult() as ServiceFabricHttpClient;

            // set the client type for Telemetry on HttpGateway.
            client.ClientTypeHeaderValue = Constants.PowershellClientTypeHeaderValue;

            if (GetMetadata.IsPresent)
            {
                AadMetadata aadMetadata = client.Cluster.GetAadMetadataAsync(cancellationToken: CancellationToken).GetAwaiter().GetResult().Metadata;

                var result = new PSObject();
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Authority), aadMetadata.Authority));
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Client), aadMetadata.Client));
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Cluster), aadMetadata.Cluster));
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Login), aadMetadata.Login));
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Redirect), aadMetadata.Redirect));
                result.Properties.Add(new PSNoteProperty(nameof(aadMetadata.Tenant), aadMetadata.Tenant));

                WriteObject(result);
            }
            else
            {
                client.Cluster.GetClusterManifestAsync(cancellationToken: CancellationToken).GetAwaiter().GetResult();
                Console.WriteLine(Resource.MsgConnectSuccess);
                SetClusterConnection(client);
            }
        }

        void SetClusterConnection(IServiceFabricClient clusterConnection)
        {
            var oldClusterConnection = SessionState.PSVariable.GetValue(Constants.ClusterConnectionVariableName, null) as IServiceFabricClient;

            if (oldClusterConnection != null)
                WriteWarning(Resource.Info_ClusterConnectionAlreadyExisted);

            SessionState.PSVariable.Set(Constants.ClusterConnectionVariableName, clusterConnection);
        }

        RemoteX509SecuritySettings GetServerX509SecuritySettings()
        {
            var x509Names = new List<X509Name>();
            RemoteX509SecuritySettings remoteX509SecuritySettings = null;

            // ServerCommonName or ServerThumbprint must be provided when connecting with IP Address.
            if (ServerCertThumbprint == null && ServerCommonName == null)
            {
                IEnumerable<Uri> uri = ConnectionEndpoint.Select(e => new Uri(e)).Where(u => u.HostNameType.Equals(UriHostNameType.Dns));
                if (uri.Count() == 0)
                    throw new PSArgumentException(Resource.ErrorNoServerCommonNameIrServerCertThumbprint);

                x509Names = uri.Select(x => new X509Name(x.Host)).ToList();
                remoteX509SecuritySettings = new RemoteX509SecuritySettings(x509Names);
            }
            else if (ServerCommonName != null)
            {
                // Use Issuer thumbprint if provided.
                if (IssuerCertThumbprints != null && IssuerCertThumbprints.Length > 0)
                {
                    if (ServerCommonName.Length != IssuerCertThumbprints.Length)
                        throw new PSArgumentException(Resource.CommonNameIssuerThumbprintMismatch);
                    else
                        for (int i = 0; i < ServerCommonName.Length; i++)
                            x509Names.Add(new X509Name(ServerCommonName[i], IssuerCertThumbprints[i]));
                }
                else
                    x509Names = ServerCommonName.Select(name => new X509Name(name)).ToList();

                remoteX509SecuritySettings = new RemoteX509SecuritySettings(x509Names);
            }
            else if (ServerCertThumbprint != null)
                remoteX509SecuritySettings = new RemoteX509SecuritySettings(ServerCertThumbprint);

            return remoteX509SecuritySettings;
        }

        bool TryConfiguringDefaultConnect()
        {
#if DotNetCoreClr
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                // continue with loading connection parameters for default connect on Windows.
            else
                return false;
#endif

            // Try Default Connect, if running from a node on cluster.
            // Currently supports reading connection settings created Local Dev cluster setup.
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Constants.SFRegistryPath);
            if (!ParameterSetName.Equals("Default") || key == null)
                return false;

            string connectionParamFile = Path.Combine(key.GetValue(Constants.SFDataRootkeyName).ToString(), Constants.LocalDevClusterConnectionParamFileName);
            if (!File.Exists(connectionParamFile))
                return false;

            ConnectionParameters connParams = null;
            try
            {
                connParams = JsonConvert.DeserializeObject<ConnectionParameters>(File.ReadAllText(connectionParamFile));
            }
            catch (JsonReaderException)
            {
                return false;
            }

            if (connParams.HttpConnectionEndpoint != null)
                ConnectionEndpoint = new string[] { connParams.HttpConnectionEndpoint };

            if (connParams.X509Credential)
                X509Credential = SwitchParameter.Present;

            if (connParams.StoreLocation != null)
            {
                Enum.TryParse<StoreLocation>(connParams.StoreLocation, out var storeLocation);
                StoreLocation = storeLocation;
            }

            if (connParams.FindType != null && connParams.FindValue != null)
            {
                Enum.TryParse<X509FindType>(connParams.FindType, out var findType);
                FindType = findType;
                string findValue = connParams.FindValue;

                if (findType.Equals(X509FindType.FindBySubjectName))
                {
                    string subjectNamePrefix = "CN=";
                    if (connParams.FindValue.StartsWith("CN="))
                        findValue = connParams.FindValue.Substring(subjectNamePrefix.Length);
                }

                FindValue = findValue;
            }

            if (connParams.ServerCommonName != null)
                ServerCommonName = new string[] { connParams.ServerCommonName };

            if (connParams.StoreName != null)
                StoreName = connParams.StoreName;

            return true;
        }
    }
}
