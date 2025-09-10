// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;

namespace Microsoft.ServiceFabric.Services
{
    static class IFuzzExtensions
    {
        internal static CommunicationListenerInfo CommunicationListenerInfo(this IFuzz fuzzy)
        {
            string name = fuzzy.String();
            var listener = Mock.Of<ICommunicationListener>();
            return new CommunicationListenerInfo(name, listener);
        }

        internal static ServiceReplicaListener ServiceReplicaListener(this IFuzz fuzzy)
        {
            var createCommunicationListener = Mock.Of<Func<StatefulServiceContext, ICommunicationListener>>();
            string name = fuzzy.String();
            bool listenOnSecondary = fuzzy.Boolean();
            return new ServiceReplicaListener(createCommunicationListener, name, listenOnSecondary);
        }

        internal static Type Type(this IFuzz fuzzy)
        {
            var mock = new Mock<Type>();
            mock.SetupGet(_ => _.Name).Returns(fuzzy.String());
            return mock.Object;
        }
    }
}
