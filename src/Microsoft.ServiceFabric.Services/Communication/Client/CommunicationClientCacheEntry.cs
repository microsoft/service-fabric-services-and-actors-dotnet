// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// This represents the cache entry that stores the communication channel to an endpoint of a replica or instance.
    /// </summary>
    /// <typeparam name="TCommunicationClient">The type of the communication client.</typeparam>
    internal class CommunicationClientCacheEntry<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        private ResolvedServiceEndpoint endpoint;

        // The endpoint that a replica or instance returns is a JSON string of the form {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        // This property caches the address corresponding the listener name given in the ListenerName property.
        private string address;
        private ResolvedServicePartition rsp;

        // These two properties track the communication channel created for the address. Channel is
        // tracked as a strong reference till the cache's eviction policy kicks in, after which it is
        // tracked as a weak reference.
        private TCommunicationClient client;
        private WeakReference clientWRef;

        // If this property is true, it means the cache entry is still in the cache. The cache clean up code sets
        // this to false before purging the entry from cache.
        private bool isInCache;

        public CommunicationClientCacheEntry()
        {
            this.Endpoint = null;
            this.Semaphore = new SemaphoreSlim(1, 1);
            this.client = default(TCommunicationClient);
            this.clientWRef = null;
            this.rsp = null;
            this.address = null;
            this.IsInCache = true;
        }

        /// <summary>
        /// Gets the lock that protects the members of the cache entry. Users of the this object
        /// should acquire this lock before accessing the properties of the object.
        /// </summary>
        public SemaphoreSlim Semaphore
        {
            get;
            private set;
        }

        public string ListenerName
        {
            get;
            set;
        }

        public ResolvedServicePartition Rsp
        {
            get
            {
                return this.rsp;
            }

            set
            {
                this.rsp = value;
                this.address = null;
            }
        }

        public TCommunicationClient Client
        {
            get
            {
                if (this.client != null)
                {
                    return this.client;
                }

                if (this.clientWRef != null)
                {
                    return (TCommunicationClient)this.clientWRef.Target;
                }
                else
                {
                    return default(TCommunicationClient);
                }
            }

            set
            {
                this.clientWRef = object.ReferenceEquals(value, default(TCommunicationClient)) ? null : new WeakReference(value);
                if (object.ReferenceEquals(value, default(TCommunicationClient)))
                {
                    this.client = default(TCommunicationClient);
                    this.clientWRef = null;
                }
                else
                {
                    this.client = value;
                    this.clientWRef = new WeakReference(value);
                }
            }
        }

        public ResolvedServiceEndpoint Endpoint
        {
            get
            {
                return this.endpoint;
            }

            set
            {
                this.endpoint = value;
            }
        }

        public bool IsInCache
        {
            get
            {
                return this.isInCache;
            }

            set
            {
                this.isInCache = value;
            }
        }

        /// <summary>
        /// The IsInCache and IsCommunicationClientValid properties are used to synchronize the code using
        /// the cache client entry and the cache clean up code - to ensure that a valid client isn't cleaned up
        /// and also a client entry that is removed from cache is not used by the communication factory.
        /// </summary>
        public bool IsCommunicationClientValid()
        {
            var isAlive = false;
            if (this.client != null)
            {
                // This is the first time the cache clean up is looking at this entry.
                // Release the strong reference.
                this.client = default(TCommunicationClient);
                isAlive = true;
            }

            if (!isAlive && this.clientWRef != null)
            {
                isAlive = this.clientWRef.IsAlive;
            }

            return isAlive;
        }

        public string GetEndpoint()
        {
            if (this.address == null)
            {
                if (string.IsNullOrEmpty(this.Endpoint.Address))
                {
                    throw new FabricException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ErrorParttionInstanceInvalidAddress,
                            this.rsp.Info.Id));
                }

                this.address = this.GetEndpointAddressForNamedEndpoint();
            }

            return this.address;
        }

        private string GetEndpointAddressForNamedEndpoint()
        {
            if (!ServiceEndpointCollection.TryParseEndpointsString(this.Endpoint.Address, out var endpointCollection))
            {
                // Client has not specified an explicit name for the endpoint, so parse failure is ok.
                // return the endpoint address.
                if (this.ListenerName == null)
                {
                    return this.Endpoint.Address;
                }
                else
                {
                    throw new FabricInvalidAddressException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ErrorInvalidPartitionEndpointAddress,
                            this.Endpoint.Address,
                            this.rsp.Info.Id));
                }
            }

            string parsedEndpointAddress;

            // Client has not specified a named endpoint, give the first endpoint in the collection.
            if (this.ListenerName == null)
            {
                if (endpointCollection.TryGetFirstEndpointAddress(out parsedEndpointAddress))
                {
                    return parsedEndpointAddress;
                }
                else
                {
                    throw new FabricInvalidAddressException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ErrorInvalidPartitionEndpointAddress,
                            this.Endpoint.Address,
                            this.rsp.Info.Id));
                }
            }

            if (!endpointCollection.TryGetEndpointAddress(this.ListenerName, out parsedEndpointAddress))
            {
                throw new FabricInvalidAddressException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SR.ErrorPartitionNamedEndpointNotFound,
                        this.ListenerName,
                        this.Endpoint.Address,
                        this.rsp.Info.Id));
            }

            return parsedEndpointAddress;
        }
    }
}
