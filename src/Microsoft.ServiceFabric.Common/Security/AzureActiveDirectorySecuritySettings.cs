// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common.Resources;

    /// <summary>
    /// Represents the claim based security credential acquired from Azure Active Directory.
    /// </summary>
    public sealed class AzureActiveDirectorySecuritySettings : ClaimsSecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureActiveDirectorySecuritySettings" /> class.
        /// Use this constructor if you have the Azure Active Directory Metadata information to get the claism token from Azure Active Directory.
        /// </summary>
        /// <param name="claimsToken">string representation of claims token acquired from STS (security token service).</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public AzureActiveDirectorySecuritySettings(string claimsToken, RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(claimsToken, remoteX509SecuritySettings)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureActiveDirectorySecuritySettings" /> class.
        /// Use this constructor if you don't have the Azure Active Directory Metadata information, provided delegate will be invoked with AAD metadata of the cluster
        /// when a claims token is needed by IServiceFabricClient.
        /// </summary>
        /// <param name="getClaimsToken">Delegate to get string representation of claims token acquired from Azure Active Directory using <see cref="AadMetadata"/>.</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public AzureActiveDirectorySecuritySettings(Func<AadMetadata, CancellationToken, Task<string>> getClaimsToken, RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(remoteX509SecuritySettings)
        {
            getClaimsToken.ThrowIfNull(nameof(getClaimsToken));
            this.GetClaimsToken = getClaimsToken;
        }

        /// <summary>
        /// Gets the delegate to get string representation of claims token acquired from Azure Active Directory using <see cref = "AadMetadata" />.
        /// </summary>
        public Func<AadMetadata, CancellationToken, Task<string>> GetClaimsToken { get; }
    }
}
