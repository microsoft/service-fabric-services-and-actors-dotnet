// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;
    using Microsoft.ServiceFabric.Common.Resources;
    
    /// <summary>
    /// Represents the claim based security credential acquired from STS (security token service).
    /// </summary>
    public class ClaimsSecuritySettings : SecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsSecuritySettings" /> class.
        /// </summary>
        /// <param name="claimsToken">string representation of claims token acquired from STS (security token service).</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public ClaimsSecuritySettings(string claimsToken, RemoteX509SecuritySettings remoteX509SecuritySettings) 
            : base(SecurityType.Claims)
        {
            if (string.IsNullOrWhiteSpace(claimsToken))
            {
                throw new ArgumentException(SR.ErrorLocalClaims);
            }

            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            this.ClaimsToken = claimsToken;
            this.RemoteX509SecuritySettings = remoteX509SecuritySettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsSecuritySettings" /> class.
        /// </summary>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        internal ClaimsSecuritySettings(RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(SecurityType.Claims)
        {
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            this.RemoteX509SecuritySettings = remoteX509SecuritySettings;
        }

        /// <summary>
        /// Gets the string representation of claims token acquired from STS (security token service).
        /// </summary>
        /// <value>
        /// The string representation of claims token acquired from STS (security token service).
        /// </value>
        public string ClaimsToken { get; }

        /// <summary>
        /// Gets the settings to validate remote certificate.
        /// </summary>
        /// <value>
        /// <see cref="RemoteX509SecuritySettings"/> to validate remote certificate.
        /// </value>
        public RemoteX509SecuritySettings RemoteX509SecuritySettings { get; }
    }
}
