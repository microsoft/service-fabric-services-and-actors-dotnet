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
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal interface IStatelessUserServiceInstance
    {
        IReadOnlyDictionary<string, string> Addresses { set; }

        IStatelessServicePartition Partition { set; }

        IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners();

        Task RunAsync(CancellationToken cancellationToken);

        Task OnOpenAsync(CancellationToken cancellationToken);

        Task OnCloseAsync(CancellationToken cancellationToken);

        void OnAbort();
    }
}
