using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Console1
{
    class MyRemotingClientFactory : Microsoft.ServiceFabric.Services.Remoting.V2.Client.IServiceRemotingClientFactory
    {
        private string endpoint;

        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

        public MyRemotingClientFactory(string endpoint)
        {
            this.endpoint = endpoint;
        }
        public async Task<IServiceRemotingClient>  GetClientAsync(Uri serviceUri, ServicePartitionKey partitionKey, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            var client = new FabricTransportServiceRemotingClientPublic(endpoint);
            await client.OpenAsync(CancellationToken.None);
            return client;
        }

        public async Task<IServiceRemotingClient> GetClientAsync(ResolvedServicePartition previousRsp, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            var client = new FabricTransportServiceRemotingClientPublic(endpoint);
            await client.OpenAsync(CancellationToken.None);
            return client;
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return new DataContractRemotingMessageFactory();
        }

        public Task<OperationRetryControl> ReportOperationExceptionAsync(IServiceRemotingClient client, ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            var op = new OperationRetryControl();
            return Task.FromResult(op);
        }
    }
}
