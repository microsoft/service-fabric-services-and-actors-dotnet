// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.FabricTransport.Client;

    internal class FabricTransportServiceRemotingCallback : IServiceRemotingCallbackClient, IDisposable
    {
        private readonly TimeSpan defaultTimeout = TimeSpan.FromMinutes(2);
        private FabricTransportCallbackClient transportCallbackClient;
        private DataContractSerializer serializer = new DataContractSerializer(typeof(ServiceRemotingMessageHeaders));
        private string clientId;

        public FabricTransportServiceRemotingCallback(FabricTransportCallbackClient transportCallbackClient)
        {
            this.transportCallbackClient = transportCallbackClient;
            this.clientId = this.transportCallbackClient.GetClientId();
        }

        public Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var header = ServiceRemotingMessageHeaders.Serialize(this.serializer, messageHeaders);
            return this.transportCallbackClient.RequestResponseAsync(header, requestBody);
        }

        public void OneWayMessage(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var header = ServiceRemotingMessageHeaders.Serialize(this.serializer, messageHeaders);

            this.transportCallbackClient.OneWayMessage(header, requestBody);
        }

        public string GetClientId()
        {
            return this.clientId;
        }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // No other managed resources to dispose.

                    if (this.transportCallbackClient != null)
                    {
                        this.transportCallbackClient.Dispose();
                    }
                }

                this.disposedValue = true;
            }
        }

        ~FabricTransportServiceRemotingCallback()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
