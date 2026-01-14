// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Common.Security;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed class DefaultTokenHandler : IBearerTokenHandler
    {
        Task IBearerTokenHandler.InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken) =>
            // Do Nothing, no Security Token to initialize for NoneClaimsHandler.
            Task.CompletedTask;

        Task IBearerTokenHandler.RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken) =>
            // Do Nothing, no claims to add for NoneClaimsHandler.
            Task.CompletedTask;

        void IBearerTokenHandler.AddTokenToRequest(HttpRequestMessage request)
        {
            // Do Nothing, no claims to add for NoneClaimsHandler
        }
    }
}
