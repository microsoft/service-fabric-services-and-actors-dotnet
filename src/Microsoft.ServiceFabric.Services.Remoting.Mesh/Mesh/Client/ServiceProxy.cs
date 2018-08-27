// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.Client
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;

    /// <summary>
    /// Provides the base implementation for the proxy to the remoted IService interfaces.
    /// </summary>
    public abstract class ServiceProxy : ProxyBase, IServiceProxy
    {
        private Base.V2.Client.ServiceRemotingPartitionClient partitionClientV2;

        /// <summary>
        /// Gets the interface type that is being remoted.
        /// </summary>
        /// <value>Service interface type</value>
        public Type ServiceInterfaceType { get; private set; }

        /// <summary>
        /// Gets the V2 Service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy</value>
        public Base.V2.Client.IServiceRemotingPartitionClient ServicePartitionClient2
        {
            get { return this.partitionClientV2; }
        }

        // V2 APi
        internal void Initialize(
            Remoting.Mesh.Builder.ServiceProxyGenerator proxyGenerator,
            Base.V2.Client.ServiceRemotingPartitionClient client,
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory)
        {
            this.partitionClientV2 = client;
            this.ServiceInterfaceType = proxyGenerator.ProxyInterfaceType;
            this.InitializeV2(serviceRemotingMessageBodyFactory);
        }

        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            var headers = new ServiceRemotingRequestMessageHeader()
            {
                InterfaceId = interfaceId,
                MethodId = methodId,
                MethodName = methodName,
            };
            return this.partitionClientV2.InvokeAsync(
                new ServiceRemotingRequestMessage(headers, requestMsgBodyValue),
                methodName,
                cancellationToken);
        }

        internal override void InvokeImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue)
        {
            // no - op as events/one way messages are not supported for services
        }
    }
}
