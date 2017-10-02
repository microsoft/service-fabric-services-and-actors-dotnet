// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Fabric;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Moq;

    public class StatefulBaseTestService : StatefulServiceBase
    {
        public List<Mock<ICommunicationListener>> Listeners { get; private set; }
        public Mock<ICommunicationListener> CurrentListener { get; private set; }

        public Mock<IStateProviderReplica2> Replica { get; private set; }
        public bool ListenOnSecondary { get; set; }
        public bool EnableListenerExceptionOnAbort { get; set; }

        public StatefulBaseTestService(StatefulServiceContext context)
            : this(context, new Mock<IStateProviderReplica2>())
        {
        }

        public StatefulBaseTestService(StatefulServiceContext context, Mock<IStateProviderReplica2> mockStateProviderReplica)
            :base(context, mockStateProviderReplica.Object)
        {
            this.Replica = mockStateProviderReplica;
            this.Listeners = new List<Mock<ICommunicationListener>>();
            this.ListenOnSecondary = false;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(this.CreateCommunicationListener, listenOnSecondary:this.ListenOnSecondary),
            };
        }
        
        private ICommunicationListener CreateCommunicationListener(ServiceContext context)
        {
            Console.WriteLine("Creating listener");
            var mockListener = new Mock<ICommunicationListener>();
            mockListener.Setup(x=>x.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult("Address"));

            if(this.EnableListenerExceptionOnAbort)
            {
                mockListener.Setup(x => x.Abort()).Throws(new Exception("Listener Abort exception."));
            }

            this.CurrentListener = mockListener;
            this.Listeners.Add(this.CurrentListener);
            return this.CurrentListener.Object;
        }
    }
}
