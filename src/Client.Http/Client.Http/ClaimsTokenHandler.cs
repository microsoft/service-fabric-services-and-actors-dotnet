// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Common.Security;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed class ClaimsTokenHandler : IBearerTokenHandler
    {
        string token;

        Task IBearerTokenHandler.InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is ClaimsSecuritySettings claimsSecuritySettings)
                token = claimsSecuritySettings.ClaimsToken;
            else
                throw new InvalidOperationException(string.Format(SR.ErrorClaimsTokenHandlerIncorrectSecuritySettings, securitySettings.GetType().Name));
            return Task.CompletedTask;
        }

        Task IBearerTokenHandler.RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            if (securitySettings is ClaimsSecuritySettings claimsSecuritySettings)
                token = claimsSecuritySettings.ClaimsToken;
            else
                throw new InvalidOperationException(string.Format(SR.ErrorClaimsTokenHandlerIncorrectSecuritySettings, securitySettings.GetType().Name));
            return Task.CompletedTask;
        }

        void IBearerTokenHandler.AddTokenToRequest(HttpRequestMessage request) =>
            request.Headers.Add("Authorization", token);
    }
}
