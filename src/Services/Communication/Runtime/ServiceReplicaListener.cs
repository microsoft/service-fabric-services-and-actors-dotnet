// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    /// <summary>
    /// Represents the communication listener and its properties for a Stateful Service replica.
    /// Endpoints given out by the communication listener are associated with the name of the communication listener.
    /// </summary>
    public sealed class ServiceReplicaListener
    {
        /// <summary>
        /// Provides the default name of the service replica listener.
        /// </summary>
        /// <value>
        /// <see cref="string"/> value of the name of the default service replica listener.
        /// </value>
        public const string DefaultName = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceReplicaListener"/> class.
        /// </summary>
        /// <param name="createCommunicationListener">Factory method for creating the communication listener</param>
        /// <param name="name">Name of the communication listener. This parameter is optional, if the service has only one communication listener</param>
        /// <param name="listenOnSecondary">Specifies if the communication listener needs to be opened when the replica becomes Active secondary. This parameter is optional</param>
        public ServiceReplicaListener(
            Func<StatefulServiceContext, ICommunicationListener> createCommunicationListener,
            string name = DefaultName,
            bool listenOnSecondary = false)
        {
            CreateCommunicationListener = createCommunicationListener ?? throw new ArgumentNullException(nameof(createCommunicationListener));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ListenOnSecondary = listenOnSecondary;
        }

        /// <summary>
        /// Gets the name of the communication listener.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this communication listener should be opened when the replica becomes an <see cref="ReplicaRole.ActiveSecondary"/>.
        /// When false, the communication listener is opened only when the replica becomes <see cref="ReplicaRole.Primary"/>.
        /// The default value for this property is <languageKeyword>false</languageKeyword>.
        /// </summary>
        /// <remarks>
        /// This flag can be set when the primary replica is too busy to serve reads and writes efficiently and the application can tolerate reading stale (but consistent) data,
        /// then data can be read from secondary replica.
        /// </remarks>
        public bool ListenOnSecondary { get; }

        /// <summary>
        /// Gets the factory method for creating the communication listener.
        /// </summary>
        /// <remarks>
        /// The factory method takes in a <see cref="StatefulServiceContext"/> and returns an <see cref="ICommunicationListener"/>.
        /// </remarks>
        public Func<StatefulServiceContext, ICommunicationListener> CreateCommunicationListener { get; }

        internal static CommunicationListenerInfo Instantiate(ServiceReplicaListener listener, StatefulServiceContext context)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            ICommunicationListener communicationListener = listener.CreateCommunicationListener(context);
            if (communicationListener == null)
                return null;

            string name = listener.Name.Equals(DefaultName) ? "default" : listener.Name;
            var original = new CommunicationListenerInfo(name, communicationListener);
            var trace = new Trace(typeof(ServiceReplicaListener), context, ServiceEventSource.Instance);
            var tracer = new TracingCommunicationListener(original, trace);
            return new CommunicationListenerInfo(name, tracer);
        }
    }
}
