// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System.Threading;

    /// <summary>
    /// Represents wrapper object that is used to get the result of querying the cancellation token for a
    /// particular method call via Async api's.
    /// </summary>
    internal class CancellationTokenResult
    {
        public CancellationTokenSource CancellationTknSource { get; set; }

        public bool CancellationTokenValid { get; set; }
    }
}
