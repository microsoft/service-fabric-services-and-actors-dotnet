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
    /// Interface for Claims Handler for ServiceFabricHttpClient.
    /// </summary>
    internal interface IBearerTokenHandler
    {
        /// <summary>
        /// Initializes Bearer Token to be used by ServiceFabricHttpClient.
        /// </summary>
        /// <param name="securitySettings">Claims Security Settings.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the operation..</param>
        /// <returns>Task for the current operation.</returns>
        Task InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes Bearer  Token to be used by ServiceFabricHttpClient.
        /// </summary>
        /// <param name="securitySettings">Claims Security Settings.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the operation..</param>
        /// /// <returns>Task for the current operation.</returns>
        Task RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken);

        /// <summary>
        /// Add Bearer token to HttpRequestMessage .
        /// </summary>
        /// <param name="request">Http Request to ad claims to.</param>
        void AddTokenToRequest(HttpRequestMessage request);
    }
}
