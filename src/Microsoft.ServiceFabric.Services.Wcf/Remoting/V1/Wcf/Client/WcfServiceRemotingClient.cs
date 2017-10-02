// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Client
{
    using System;
    using System.Fabric;
    using System.Globalization;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Wcf;

    internal class WcfServiceRemotingClient : IServiceRemotingClient
    {
        private readonly WcfCommunicationClient<IServiceRemotingContract> wcfClient;

        public WcfCommunicationClient<IServiceRemotingContract> WcfClient
        {
            get { return this.wcfClient; }
        }

        public WcfServiceRemotingClient(WcfCommunicationClient<IServiceRemotingContract> wcfClient)
        {
            this.wcfClient = wcfClient;
        }

        /// <summary>
        /// Gets or Sets the Resolved service partition which was used when this client was created.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServicePartition" /> object</value>
        public ResolvedServicePartition ResolvedServicePartition
        {
            get { return this.wcfClient.ResolvedServicePartition; }

            set { this.wcfClient.ResolvedServicePartition = value; }
        }

        /// <summary>
        /// Gets or Sets the name of the listener in the replica or instance to which the client is
        /// connected to.
        /// </summary>
        public string ListenerName
        {
            get { return this.wcfClient.ListenerName; }

            set { this.wcfClient.ListenerName = value; }
        }

        /// <summary>
        /// Gets or Sets the service endpoint to which the client is connected to.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServiceEndpoint" /></value>
        public ResolvedServiceEndpoint Endpoint
        {
            get { return this.wcfClient.Endpoint; }

            set { this.wcfClient.Endpoint = value; }
        }

        async public Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders headers, byte[] requestBody)
        {
            try
            {
                return await this.wcfClient.Channel.RequestResponseAsync(headers, requestBody).ContinueWith(
                    t => t.GetAwaiter().GetResult(), 
                    TaskScheduler.Default);

                // the code above (TaskScheduler.Default) for dispatches the responses on different thread
                // so that if the user code blocks, we do not stop the response receive pump in WCF
            }
            catch(FaultException<RemoteExceptionInformation> faultException)
            {
                Exception remoteException;
                if (RemoteExceptionInformation.ToException(faultException.Detail, out remoteException))
                {
                    throw new AggregateException(remoteException);
                }

                throw new ServiceException(remoteException.GetType().FullName,string.Format(
                                CultureInfo.InvariantCulture,
                                Microsoft.ServiceFabric.Services.Wcf.SR.ErrorDeserializationFailure,
                                remoteException.ToString()));
            }
        }

        public void SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            this.wcfClient.Channel.OneWayMessage(messageHeaders, requestBody);
        }
    }
}