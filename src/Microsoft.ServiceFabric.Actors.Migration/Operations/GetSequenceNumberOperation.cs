// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class GetSequenceNumberOperation
    {
        private KvsActorStateProvider kvsActorStateProvider;
        private object first;
        private HttpRequest request;

        public GetSequenceNumberOperation(KvsActorStateProvider kvsActorStateProvider, object first, HttpRequest request)
        {
            this.kvsActorStateProvider = kvsActorStateProvider;
            this.first = first;
            this.request = request;
        }

        internal Task<long> ExecuteAsync(CancellationToken none)
        {
            throw new NotImplementedException();
        }
    }
}
