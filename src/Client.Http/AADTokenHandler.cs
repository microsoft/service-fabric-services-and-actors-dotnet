// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Http.Resources;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Token Handler for AzureActiveDirectorySecuritySettings
    /// </summary>
    internal class AADTokenHandler : IBearerTokenHandler
    {
        private const string BearerPrefix = "Bearer ";
        private readonly AadMetadata aadMetaData;
        private string token;

        public AADTokenHandler()
        {
        }

        public AADTokenHandler(AadMetadata aadMetaData)
        {
            this.aadMetaData = aadMetaData;
        }

        private string Token
        {
            get
            {
                return this.token;
            }

            set
            {
                this.token = value.StartsWith(BearerPrefix) ? value.Remove(0, BearerPrefix.Length) : value;
            }
        }

        /// <inheritdoc />
        public async Task InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (aadSecuritySettings.ClaimsToken != null)
                {
                    this.Token = aadSecuritySettings.ClaimsToken;
                }
                else if (aadSecuritySettings.GetClaimsToken != null && this.aadMetaData != null)
                {
                    this.Token = await aadSecuritySettings.GetClaimsToken.Invoke(this.aadMetaData, cancellationToken);
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
            if (securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (aadSecuritySettings.ClaimsToken != null)
                {
                    this.Token = aadSecuritySettings.ClaimsToken;
                }
                else if (aadSecuritySettings.GetClaimsToken != null && this.aadMetaData != null)
                {
                    this.Token = await aadSecuritySettings.GetClaimsToken.Invoke(this.aadMetaData, cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public void AddTokenToRequest(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", $"Bearer {this.Token}");
        }
    }
}
