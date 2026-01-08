// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Handles adding of No Claims for Service Fabric Http Client, used when SecurityType is X509 or Windows.
    /// </summary>
    internal class DefaultTokenHandler : IBearerTokenHandler
    {
        /// <inheritdoc />
        public Task InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            // Do Nothing, no Security Token to initialize for NoneClaimsHandler.
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken)
        {
            // Do Nothing, no claims to add for NoneClaimsHandler.
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void AddTokenToRequest(HttpRequestMessage request)
        {
            // Do Nothing, no claims to add for NoneClaimsHandler
        }
    }
}
