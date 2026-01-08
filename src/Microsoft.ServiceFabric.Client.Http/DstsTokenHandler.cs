// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Http.Resources;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Token Handler for DstsSecuritySettings
    /// </summary>
    internal class DstsTokenHandler : IBearerTokenHandler
    {
        private readonly TokenServiceMetadata metaData;
        private string token;

        public DstsTokenHandler()
        {
        }

        public DstsTokenHandler(TokenServiceMetadata aadMetaData)
        {
            this.metaData = aadMetaData;
        }

        /// <inheritdoc />
        public async Task InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (dstsSecuritySettings.ClaimsToken != null)
                {
                    this.token = dstsSecuritySettings.ClaimsToken;
                }
                else if (dstsSecuritySettings.GetClaimsToken != null && this.metaData != null)
                {
                    this.token = await dstsSecuritySettings.GetClaimsToken.Invoke(this.metaData, cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format(SR.ErrorAADTokenHandlerIncorrectSecuritySettings, securitySettings.GetType().Name));
            }
        }

        /// <inheritdoc />
        public async Task RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (dstsSecuritySettings.ClaimsToken != null)
                {
                    this.token = dstsSecuritySettings.ClaimsToken;
                }
                else if (dstsSecuritySettings.GetClaimsToken != null && this.metaData != null)
                {
                    this.token = await dstsSecuritySettings.GetClaimsToken.Invoke(this.metaData, cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public void AddTokenToRequest(HttpRequestMessage request)
        {
             request.Headers.TryAddWithoutValidation("Authorization", this.token);
        }
    }
}
