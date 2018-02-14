// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System.Fabric;

    internal class ResolvedServicePartitionClient
    {
        public ResolvedServicePartition Rsp { get; set; }

        public ICommunicationClient Client { get; set; }

        public ResolvedServicePartitionClient()
        {
            this.Rsp = null;
            this.Client = null;
        }

        public ResolvedServicePartitionClient(ResolvedServicePartitionClient other)
        {
            this.Rsp = other.Rsp;
            this.Client = other.Client;
        }
    }
}
