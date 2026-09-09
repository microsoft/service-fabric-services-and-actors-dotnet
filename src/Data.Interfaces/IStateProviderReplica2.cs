// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the additional member a <see cref="IStateProviderReplica">reliable state provider replica</see> must implement for Service Fabric to interact with it.
    /// </summary>
    public interface IStateProviderReplica2 : IStateProviderReplica
    {
        /// <summary>
        /// Sets the callback invoked after the framework restores the replica's state following data loss.
        /// </summary>
        /// <remarks>
        /// This callback runs only after a successful restore during <see cref="IStateProviderReplica.OnDataLossAsync"/>
        /// processing. Exceptions thrown by the callback are reported as a replica health error and propagated to the caller.
        /// </remarks>
        Func<CancellationToken, Task> OnRestoreCompletedAsync { set; }
    }
}
