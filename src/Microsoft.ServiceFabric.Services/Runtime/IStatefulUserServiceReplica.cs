// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal interface IStatefulUserServiceReplica
    {
        IReadOnlyDictionary<string, string> Addresses { set; }

        IStatefulServicePartition Partition { set; }

        IStateProviderReplica2 CreateStateProviderReplica();

        IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners();

        Task RunAsync(CancellationToken cancellationToken);

        Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken);

        Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken);

        Task OnCloseAsync(CancellationToken cancellationToken);

        void OnAbort();
    }
}
