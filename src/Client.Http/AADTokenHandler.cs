// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client.Http.Resources;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed class AADTokenHandler : IBearerTokenHandler
    {
        const string BearerPrefix = "Bearer ";
        readonly AadMetadata aadMetaData;
        string token;

        internal AADTokenHandler(AadMetadata aadMetaData = default) =>
            this.aadMetaData = aadMetaData;

        string Token
        {
            get { return token; }
            set { token = value.StartsWith(BearerPrefix) ? value.Remove(0, BearerPrefix.Length) : value; }
        }

        async Task IBearerTokenHandler.InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (aadSecuritySettings.ClaimsToken != null)
                    Token = aadSecuritySettings.ClaimsToken;
                else if (aadSecuritySettings.GetClaimsToken != null && aadMetaData != null)
                    Token = await aadSecuritySettings.GetClaimsToken.Invoke(aadMetaData, cancellationToken);
            }
            else
                throw new InvalidOperationException(string.Format(SR.ErrorAADTokenHandlerIncorrectSecuritySettings, securitySettings.GetType().Name));
        }

        async Task IBearerTokenHandler.RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is AzureActiveDirectorySecuritySettings aadSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (aadSecuritySettings.ClaimsToken != null)
                    Token = aadSecuritySettings.ClaimsToken;
                else if (aadSecuritySettings.GetClaimsToken != null && aadMetaData != null)
                    Token = await aadSecuritySettings.GetClaimsToken.Invoke(aadMetaData, cancellationToken);
            }
        }

        void IBearerTokenHandler.AddTokenToRequest(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", $"Bearer {Token}");
        }
    }
}
