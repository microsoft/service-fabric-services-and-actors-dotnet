// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    ////using System.Collections.Generic;
    ////using System.Fabric;
    ////using System.Threading;
    ////using System.Threading.Tasks;
    ////using Grpc.Core;
    ////using Microsoft.ServiceFabric.Actors.Generator;
    ////using Microsoft.ServiceFabric.Actors.Runtime;
    ////using Microsoft.ServiceFabric.Services.Communication.Runtime;

    ////internal class GrpcCommunicationListener : ICommunicationListener
    ////{
    ////    public GrpcCommunicationListener(StatefulServiceContext context, ActorTypeInformation actorTypeInformation, IEnumerable<ServerServiceDefinition> serviceDefinitions)
    ////    {
    ////        var host = FabricRuntime.GetNodeContext().IPAddressOrFQDN;
    ////        var endpoint = context.CodePackageActivationContext.GetEndpoint(ActorNameFormat.GetActorKvsMigrationEndpointName(actorTypeInformation.ImplementationType));

    ////        // TODO: Use Secure server
    ////        this.ServerPort = new ServerPort(host, endpoint.Port, ServerCredentials.Insecure);
    ////        this.ServiceDefinitions = serviceDefinitions;
    ////    }

    ////    private ServerPort ServerPort { get; set; } = null;

    ////    private Server Server { get; set; } = null;

    ////    private IEnumerable<ServerServiceDefinition> ServiceDefinitions { get; set; } = null;

    ////    Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
    ////    {
    ////        this.Server = new Server()
    ////        {
    ////            Ports = { this.ServerPort },
    ////        };

    ////        foreach (var service in this.ServiceDefinitions)
    ////        {
    ////            this.Server.Services.Add(service);
    ////        }

    ////        this.Server.Start();
    ////        return Task.FromResult($"http://{this.ServerPort.Host}:{this.ServerPort.Port}");
    ////    }

    ////    async Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
    ////    {
    ////        if (this.Server == null)
    ////        {
    ////            return;
    ////        }

    ////        await this.Server.ShutdownAsync();
    ////        this.Server = null;
    ////    }

    ////    void ICommunicationListener.Abort()
    ////    {
    ////        this.Server.KillAsync().Wait();
    ////        this.Server = null;
    ////    }
    ////}
}
