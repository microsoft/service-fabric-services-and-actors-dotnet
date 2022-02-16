// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class DefaultActorRequestForwarder : IRequestForwarder
    {
        private static readonly string TraceType = typeof(DefaultActorRequestForwarder).Name;
        private ServiceRemotingPartitionClient remotingClient;
        private string traceId;

        public async Task<IServiceRemotingResponseMessage> ForwardActorRequestAsync(IServiceRemotingRequestMessage request, CancellationToken token)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Forwarding actor request");
            var retVal = await this.remotingClient.InvokeAsync(request, request.GetHeader().MethodName, token);

            return retVal;
        }
    }
}
