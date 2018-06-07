// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ActorManagerAdapter
    {
        internal IActorManager ActorManager { get; set; }

        public Task OpenAsync(IServicePartition servicePartition, CancellationToken cancellationToken)
        {
            return this.ActorManager.OpenAsync(servicePartition, cancellationToken);
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (this.ActorManager != null)
            {
                try
                {
                    await this.ActorManager.CloseAsync(cancellationToken);
                }
                catch (Exception)
                {
                    this.ActorManager.Abort();
                }

                this.ActorManager = null;
            }
        }

        public void Abort()
        {
            if (this.ActorManager != null)
            {
                this.ActorManager.Abort();
                this.ActorManager = null;
            }
        }
    }
}
