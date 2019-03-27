// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Fabric;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// This class represents the endpoints of a Reliable service. Each endpoint has a listener name and the address of that listener.
    /// </summary>
    [DataContract]
    public sealed class ServiceEndpointCollection
    {
        [DataMember(Name = "Endpoints")]
        private Dictionary<string, string> endpoints;

        private object endpointsLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceEndpointCollection"/> class that is empty.
        /// </summary>
        public ServiceEndpointCollection()
        {
            this.endpoints = new Dictionary<string, string>();
            this.endpointsLock = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceEndpointCollection"/> class with a single endpoint, identified by the listener name.
        /// </summary>
        /// <param name="listenerName">Listener name of the endpoint.</param>
        /// <param name="endpointAddress">Address of the endpoint.</param>
        public ServiceEndpointCollection(string listenerName, string endpointAddress)
            : this()
        {
            this.endpoints[listenerName] = endpointAddress;
        }

        /// <summary>
        /// Constructs an EndpointsCollection from a string version of the endpoints. String form of EndpointsCollection is of the form
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}.
        /// </summary>
        /// <param name="endpointsString">string form of endpointsCollection.</param>
        /// <param name="serviceEndpoints">ServiceEndpointCollection object if the string can be parsed to a valid ServiceEndpointCollection object.</param>
        /// <returns>True if the string can be parsed to a valid EndpointsCollection, False otherwise.</returns>
        public static bool TryParseEndpointsString(string endpointsString, out ServiceEndpointCollection serviceEndpoints)
        {
            if (endpointsString == string.Empty)
            {
                serviceEndpoints = new ServiceEndpointCollection();
                return true;
            }

            serviceEndpoints = null;
            var deserializer = new DataContractJsonSerializer(
                typeof(ServiceEndpointCollection),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
            try
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(endpointsString));
                serviceEndpoints = (ServiceEndpointCollection)deserializer.ReadObject(stream);
                return true;
            }
            catch (Exception)
            {
                // Catch any exception that occurs during deserialization
            }

            return false;
        }

        /// <summary>
        /// Adds the endpoints in the input EndpointsCollection class to the EndpointsCollection.
        /// </summary>
        /// <param name="newEndpoints">input EndpointsCollection.</param>
        public void AddEndpoints(ServiceEndpointCollection newEndpoints)
        {
            lock (this.endpointsLock)
            {
                foreach (var item in newEndpoints.endpoints)
                {
                    this.AddEndpointCallerHoldsLock(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Adds an endpoint to the EndpointsCollection.
        /// </summary>
        /// <param name="listenerName">Listener name of the endpoint.</param>
        /// <param name="endpointAddress">Address of the endpoint.</param>
        public void AddEndpoint(string listenerName, string endpointAddress)
        {
            lock (this.endpointsLock)
            {
                this.AddEndpointCallerHoldsLock(listenerName, endpointAddress);
            }
        }

        /// <summary>
        /// Gets the first endpoint address in the EndpointsCollection.
        /// </summary>
        /// <param name="endpointAddress">First endpoint in the EndpointsCollection.</param>
        /// <returns>True if there is at-least one endpoint in the EndpointsCollection, false otherwise.</returns>
        public bool TryGetFirstEndpointAddress(out string endpointAddress)
        {
            endpointAddress = null;
            lock (this.endpointsLock)
            {
                if (this.endpoints.Count == 0)
                {
                    return false;
                }

                var enumerator = this.endpoints.GetEnumerator();
                enumerator.MoveNext();
                endpointAddress = enumerator.Current.Value;
                return true;
            }
        }

        /// <summary>
        /// Gets the endpoint identified by the listener name.
        /// </summary>
        /// <param name="listenerName">Listener name.</param>
        /// <param name="endpointAddress">Address of the endpoint if an endpoint with that listener name exists.</param>
        /// <returns>True if an endpoint with the listener name exists, False otherwise.</returns>
        public bool TryGetEndpointAddress(string listenerName, out string endpointAddress)
        {
            endpointAddress = null;
            lock (this.endpointsLock)
            {
                if (!this.endpoints.ContainsKey(listenerName))
                {
                    return false;
                }

                endpointAddress = this.endpoints[listenerName];
                return true;
            }
        }

        /// <summary>
        /// Converts the endpointsCollection to a JSON string of the form
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}.
        /// </summary>
        /// <returns>String form of the endpointsCollection.</returns>
        public override string ToString()
        {
            if (this.endpoints.Count != 0)
            {
                var serializer = new DataContractJsonSerializer(
                    typeof(ServiceEndpointCollection),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });

                var stream = new MemoryStream();
                serializer.WriteObject(stream, this);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a ReadOnlyDictionary of the endpointsCollection.
        /// </summary>
        /// <returns>EndpointsCollection as a ReadOnlyDictionary.</returns>
        public IReadOnlyDictionary<string, string> ToReadOnlyDictionary()
        {
            return new ReadOnlyDictionary<string, string>(this.endpoints);
        }

        private void AddEndpointCallerHoldsLock(string listenerName, string endpointAddress)
        {
            if (this.endpoints.ContainsKey(listenerName))
            {
                if (listenerName.Length == 0)
                {
                    throw new FabricElementAlreadyExistsException(SR.ErrorListenerNameNotSpecified);
                }
                else
                {
                    throw new FabricElementAlreadyExistsException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ErrorListenerAlreadyExists,
                            listenerName));
                }
            }

            this.endpoints[listenerName] = endpointAddress;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext c)
        {
            // Deserialization doesnt invoke the constructor, so the lock object must be
            // initialized explicitly.
            this.endpointsLock = new object();
        }
    }
}
