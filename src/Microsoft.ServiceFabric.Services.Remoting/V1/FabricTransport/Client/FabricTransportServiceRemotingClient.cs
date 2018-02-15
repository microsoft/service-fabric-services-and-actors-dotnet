// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Client
{
    using System;
    using System.Fabric;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport;
    using Microsoft.ServiceFabric.FabricTransport.Client;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;

    internal class FabricTransportServiceRemotingClient : IServiceRemotingClient
    {
        private readonly DataContractSerializer serializer =
            new DataContractSerializer(typeof(ServiceRemotingMessageHeaders));

        private readonly FabricTransportClient nativeClient;
        private readonly FabricTransportSettings settings;
        private ResolvedServicePartition resolvedServicePartition;
        private ResolvedServiceEndpoint resolvedServiceEndpoint;
        private string listenerName;

        public FabricTransportServiceRemotingClient(
            FabricTransportClient nativeClient,
            FabricTransportRemotingClientConnectionHandler remotingClientConnectionHandler)
        {
            this.settings = nativeClient.Settings;
            this.ConnectionAddress = nativeClient.ConnectionAddress;
            this.IsValid = true;
            this.nativeClient = nativeClient;
            this.RemotingClientConnectionHandler = remotingClientConnectionHandler;
        }


        ~FabricTransportServiceRemotingClient()
        {
            if (this.nativeClient != null)
            {
                this.nativeClient.Dispose();
            }
        }

        public FabricTransportRemotingClientConnectionHandler RemotingClientConnectionHandler { get; private set; }

        public string ConnectionAddress { get; private set; }

        public bool IsValid { get; private set; }

        ResolvedServicePartition ICommunicationClient.ResolvedServicePartition
        {
            get { return this.resolvedServicePartition; }
            set
            {
                this.resolvedServicePartition = value;
                if (this.RemotingClientConnectionHandler != null)
                {
                    this.RemotingClientConnectionHandler.ResolvedServicePartition = value;
                }
            }
        }

        string ICommunicationClient.ListenerName
        {
            get { return this.listenerName; }
            set
            {
                this.listenerName = value;
                if (this.RemotingClientConnectionHandler != null)
                {
                    this.RemotingClientConnectionHandler.ListenerName = value;
                }
            }
        }

        ResolvedServiceEndpoint ICommunicationClient.Endpoint
        {
            get { return this.resolvedServiceEndpoint; }
            set
            {
                this.resolvedServiceEndpoint = value;
                if (this.RemotingClientConnectionHandler != null)
                {
                    this.RemotingClientConnectionHandler.Endpoint = value;
                }
            }
        }

        Task<byte[]> IServiceRemotingClient.RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody)
        {
            var header = ServiceRemotingMessageHeaders.Serialize(this.serializer, messageHeaders);
            return this.nativeClient.RequestResponseAsync(header, requestBody,
                this.settings.OperationTimeout).ContinueWith(t =>
                {
                    var retval = t.GetAwaiter().GetResult();
                    if (retval.IsException)
                    {
                        var isDeserialzied =
                            RemoteExceptionInformation.ToException(new RemoteExceptionInformation(retval.GetBody()),
                                out var e);

                        if (isDeserialzied)
                        {
                            throw new AggregateException(e);
                        }
                        else
                        {
                            throw new ServiceException(e.GetType().FullName, string.Format(
                                CultureInfo.InvariantCulture,
                                Remoting.SR.ErrorDeserializationFailure,
                                e.ToString()));
                        }
                    }
                    return retval.GetBody();
                });
        }

        void IServiceRemotingClient.SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var header = ServiceRemotingMessageHeaders.Serialize(this.serializer, messageHeaders);
            this.nativeClient.SendOneWay(header, requestBody);
        }

        public void Abort()
        {
            this.IsValid = false;
            this.nativeClient.Abort();
        }
    }
}
