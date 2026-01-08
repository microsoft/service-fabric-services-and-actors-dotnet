// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the claim based security credential acquired from Dsts secure token service..
    /// </summary>
    public sealed class DstsClaimsSecuritySettings : ClaimsSecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DstsClaimsSecuritySettings" /> class.
        /// Use this constructor if you have the Metadata information to get the claims token from Secure Token Service..
        /// </summary>
        /// <param name="claimsToken">string representation of claims token acquired from STS (security token service).</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public DstsClaimsSecuritySettings(string claimsToken, RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(claimsToken, remoteX509SecuritySettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DstsClaimsSecuritySettings" /> class.
        /// Use this constructor if you don't have the Metadata information, provided delegate will be invoked with  metadata of the cluster
        /// when a claims token is needed by IServiceFabricClient.
        /// </summary>
        /// <param name="getClaimsToken">Delegate to get string representation of claims token acquired from STS (security token service) using <see cref="TokenServiceMetadata"/>.</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public DstsClaimsSecuritySettings(Func<TokenServiceMetadata, CancellationToken, Task<string>> getClaimsToken, RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(remoteX509SecuritySettings)
        {
            getClaimsToken.ThrowIfNull(nameof(getClaimsToken));
            this.GetClaimsToken = getClaimsToken;
        }

        /// <summary>
        /// Gets the delegate to get string representation of claims token acquired from STS (security token service) using <see cref = "TokenServiceMetadata" />.
        /// </summary>
        public Func<TokenServiceMetadata, CancellationToken, Task<string>> GetClaimsToken { get; }
    }
}
