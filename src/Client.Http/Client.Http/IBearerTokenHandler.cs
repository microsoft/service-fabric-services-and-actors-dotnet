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
    interface IBearerTokenHandler
    {
        Task InitializeTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken);
        Task RefreshTokenAsync(SecuritySettings securitySettings, CancellationToken cancellationToken);
        void AddTokenToRequest(HttpRequestMessage request);
    }
}
