// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    /// <summary>
    /// Represents the communication listener and its properties for a Stateless Service instance.
    /// Endpoints given out by the communication listener are associated with the name of the communication listener.
    /// </summary>
    public sealed class ServiceInstanceListener
    {
        /// <summary>
        /// Provides the default name of the service instance listener.
        /// </summary>
        /// <value>
        /// <see cref="string"/> value of the name of the default service instance listener.
        /// </value>
        public const string DefaultName = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstanceListener"/> class.
        /// </summary>
        /// <param name="createCommunicationListener">Factory method for creating the communication listener.</param>
        /// <param name="name">Name of the communication listener. This parameter is optional if the Stateless Service has only one communication listener. If it is not given, the Name is set to DefaultName.</param>
        public ServiceInstanceListener(
            Func<StatelessServiceContext, ICommunicationListener> createCommunicationListener,
            string name = DefaultName)
        {
            CreateCommunicationListener = createCommunicationListener ?? throw new ArgumentNullException(nameof(createCommunicationListener));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the name of the communication listener.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the factory method for creating the communication listener.
        /// </summary>
        /// <remarks>
        /// The factory method takes in a <see cref="StatelessServiceContext"/> and returns an <see cref="ICommunicationListener"/>.
        /// </remarks>
        public Func<StatelessServiceContext, ICommunicationListener> CreateCommunicationListener { get; }

        static internal CommunicationListenerInfo Instantiate(ServiceInstanceListener listener, StatelessServiceContext context)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            ICommunicationListener communicationListener = listener.CreateCommunicationListener(context);
            if (communicationListener == null)
                return null;

            string name = listener.Name.Equals(DefaultName) ? "default" : listener.Name;
            var original = new CommunicationListenerInfo(name, communicationListener);
            var trace = new Trace(typeof(ServiceInstanceListener), context, ServiceEventSource.Instance);
            var tracer = new TracingCommunicationListener(original, trace);
            return new CommunicationListenerInfo(name, tracer);
        }
    }
}
