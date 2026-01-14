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
    sealed class DstsTokenHandler : IBearerTokenHandler
    {
        readonly TokenServiceMetadata metaData;
        string token;

        internal DstsTokenHandler(TokenServiceMetadata aadMetaData = default) =>
            metaData = aadMetaData;

        async Task IBearerTokenHandler.InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (dstsSecuritySettings.ClaimsToken != null)
                    token = dstsSecuritySettings.ClaimsToken;
                else if (dstsSecuritySettings.GetClaimsToken != null && metaData != null)
                    token = await dstsSecuritySettings.GetClaimsToken.Invoke(metaData, cancellationToken);
            }
            else
                throw new InvalidOperationException(string.Format(SR.ErrorAADTokenHandlerIncorrectSecuritySettings, securitySettings.GetType().Name));
        }

        async Task IBearerTokenHandler.RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is DstsClaimsSecuritySettings dstsSecuritySettings)
            {
                // Use ClaimsToken if provided by the user directly else use the delegate to get the ClaimsToken from user.
                if (dstsSecuritySettings.ClaimsToken != null)
                    token = dstsSecuritySettings.ClaimsToken;
                else if (dstsSecuritySettings.GetClaimsToken != null && metaData != null)
                    token = await dstsSecuritySettings.GetClaimsToken.Invoke(metaData, cancellationToken);
            }
        }

        void IBearerTokenHandler.AddTokenToRequest(HttpRequestMessage request)
        {
             request.Headers.TryAddWithoutValidation("Authorization", token);
        }
    }
}
