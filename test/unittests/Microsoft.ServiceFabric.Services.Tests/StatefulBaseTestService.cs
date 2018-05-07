// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Moq;

    /// <summary>
    /// Test class for StatefulService.
    /// </summary>
    public class StatefulBaseTestService : StatefulServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulBaseTestService"/> class.
        /// </summary>
        /// <param name="context">Service context.</param>
        public StatefulBaseTestService(StatefulServiceContext context)
            : this(context, new Mock<IStateProviderReplica2>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulBaseTestService"/> class.
        /// </summary>
        /// <param name="context">Service context.</param>
        /// <param name="mockStateProviderReplica">Reliable state provider replica..</param>
        public StatefulBaseTestService(
            StatefulServiceContext context,
            Mock<IStateProviderReplica2> mockStateProviderReplica)
            : base(context, mockStateProviderReplica.Object)
        {
            this.Replica = mockStateProviderReplica;
            this.Listeners = new List<Mock<ICommunicationListener>>();
            this.ListenOnSecondary = false;
        }

        /// <summary>
        /// Gets the listeners created for the service.
        /// </summary>
        public List<Mock<ICommunicationListener>> Listeners { get; private set; }

        /// <summary>
        /// Gets the current listener for the service.
        /// </summary>
        public Mock<ICommunicationListener> CurrentListener { get; private set; }

        /// <summary>
        /// Gets reliable state provider replica.
        /// </summary>
        public Mock<IStateProviderReplica2> Replica { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether service listens on secondary replicas.
        /// </summary>
        public bool ListenOnSecondary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether listener should throw exception on abort.
        /// </summary>
        public bool EnableListenerExceptionOnAbort { get; set; }

        /// <summary>
        /// Get the service replica listeners that the user service wants to open.
        /// </summary>
        /// <returns>A list of service replica listener that should be opened by the adapter.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(this.CreateCommunicationListener, listenOnSecondary: this.ListenOnSecondary),
            };
        }

        private ICommunicationListener CreateCommunicationListener(ServiceContext context)
        {
            Console.WriteLine("Creating listener");
            var mockListener = new Mock<ICommunicationListener>();
            mockListener.Setup(x => x.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult("Address"));

            if (this.EnableListenerExceptionOnAbort)
            {
                mockListener.Setup(x => x.Abort()).Throws(new Exception("Listener Abort exception."));
            }

            this.CurrentListener = mockListener;
            this.Listeners.Add(this.CurrentListener);
            return this.CurrentListener.Object;
        }
    }
}
