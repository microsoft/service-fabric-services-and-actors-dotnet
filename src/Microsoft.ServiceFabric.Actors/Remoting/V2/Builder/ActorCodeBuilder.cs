// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Actors.Remoting.Description;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;

    internal class ActorCodeBuilder : CodeBuilder
    {
        internal static readonly InterfaceDetailsStore InterfaceDetailsStore = new InterfaceDetailsStore();
        private static readonly ICodeBuilder Instance = new ActorCodeBuilder();
        private static readonly object BuildLock = new object();
        private readonly ICodeBuilder eventCodeBuilder;
        private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;
        private readonly MethodDispatcherBuilder<ActorMethodDispatcherBase> methodDispatcherBuilder;
        private readonly ActorProxyGeneratorBuilder proxyGeneratorBuilder;

        public ActorCodeBuilder()
            : base(new ActorCodeBuilderNames("V2"))
        {
            this.eventCodeBuilder = new ActorCodeBuilder.ActorEventCodeBuilder();
            this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
            this.methodDispatcherBuilder = new MethodDispatcherBuilder<ActorMethodDispatcherBase>(this);
            this.proxyGeneratorBuilder = new ActorProxyGeneratorBuilder(this);
        }

        public static ActorProxyGenerator GetOrCreateProxyGenerator(Type actorInterfaceType)
        {
            lock (BuildLock)
            {
                return (ActorProxyGenerator)Instance.GetOrBuildProxyGenerator(actorInterfaceType).ProxyGenerator;
            }
        }

        public static ActorMethodDispatcherBase GetOrCreateMethodDispatcher(Type actorInterfaceType)
        {
            lock (BuildLock)
            {
                return (ActorMethodDispatcherBase)Instance.GetOrBuilderMethodDispatcher(actorInterfaceType).MethodDispatcher;
            }
        }

        public static ActorEventProxyGenerator GetOrCreateEventProxyGenerator(Type actorEventInterfaceType)
        {
            var eventCodeBuilder = ((ActorCodeBuilder)Instance).eventCodeBuilder;
            lock (BuildLock)
            {
                return (ActorEventProxyGenerator)eventCodeBuilder.GetOrBuildProxyGenerator(actorEventInterfaceType).ProxyGenerator;
            }
        }

        internal static bool TryGetKnownTypes(int interfaceId, out InterfaceDetails interfaceDetails)
        {
            return InterfaceDetailsStore.TryGetKnownTypes(interfaceId, out interfaceDetails);
        }

        internal static bool TryGetKnownTypes(string interfaceName, out InterfaceDetails interfaceDetails)
        {
            return InterfaceDetailsStore.TryGetKnownTypes(interfaceName, out interfaceDetails);
        }

        protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
        {
            var actorInterfaceDescription = ActorInterfaceDescription.CreateUsingCRCId(interfaceType);
            var res = this.methodDispatcherBuilder.Build(actorInterfaceDescription);
            return res;
        }

        // We also need this to have ActorEventProxy invoking V1 Api for Compat Mode
        protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
        {
            var actorInterfaceDescriptions = ActorInterfaceDescription.CreateUsingCRCId(interfaceType);
            var result = this.methodBodyTypesBuilder.Build(actorInterfaceDescriptions);
            InterfaceDetailsStore.UpdateKnownTypeDetail(actorInterfaceDescriptions, result);
            return result;
        }

        protected override ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType)
        {
            // get all event interfaces supported by this actorInterface and build method dispatchers for those
            var actorEventInterfaces = interfaceType.GetActorEventInterfaces();
            var actorEventDispatchers = actorEventInterfaces.Select(
                t => this.eventCodeBuilder.GetOrBuilderMethodDispatcher(t).MethodDispatcher);
            var actorMethodDispatcherBases =
                actorEventDispatchers.Cast<ActorMethodDispatcherBase>();

            // register them with the event subscriber manager
            ActorEventSubscriberManager.Instance.RegisterEventDispatchers(actorMethodDispatcherBases);

            // create all actor interfaces that this interface derives from
            var actorInterfaces = new List<Type>() { interfaceType };
            actorInterfaces.AddRange(interfaceType.GetActorInterfaces());

            // create interface descriptions for all interfaces
            var actorInterfaceDescriptions = actorInterfaces.Select<Type, InterfaceDescription>(
                t => ActorInterfaceDescription.CreateUsingCRCId(t));

            var res = this.proxyGeneratorBuilder.Build(interfaceType, actorInterfaceDescriptions);
            return res;
        }

        private class ActorEventCodeBuilder : CodeBuilder
        {
            private readonly MethodDispatcherBuilder<ActorMethodDispatcherBase> methodDispatcherBuilder;
            private readonly ActorEventProxyGeneratorBuilder proxyGeneratorBuilder;
            private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;

            public ActorEventCodeBuilder()
                : base(new ActorEventCodeBuilderNames("V2"))
            {
                this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
                this.methodDispatcherBuilder = new MethodDispatcherBuilder<ActorMethodDispatcherBase>(this);
                this.proxyGeneratorBuilder = new ActorEventProxyGeneratorBuilder(this);
            }

            protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
            {
                var actorEventInterfaceDescription = ActorEventInterfaceDescription.CreateUsingCRCId(interfaceType);
                var res = this.methodDispatcherBuilder.Build(actorEventInterfaceDescription);
                return res;
            }

            // This needed to support Server Compact mode where some clients will be in V1 Stack
            protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
            {
                var actorEventInterfaceDescription = ActorEventInterfaceDescription.CreateUsingCRCId(interfaceType);
                var res = this.methodBodyTypesBuilder.Build(actorEventInterfaceDescription);
                InterfaceDetailsStore.UpdateKnownTypeDetail(actorEventInterfaceDescription, res);
                return res;
            }

            protected override ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType)
            {
                // get all event interfaces supported by this actorInterface and build method dispatchers for those
                var actorEventInterfaces = new[] { interfaceType };

                // create interface descriptions for all interfaces
                var actorEventInterfaceDescriptions = actorEventInterfaces.Select<Type, InterfaceDescription>(
                    t => ActorEventInterfaceDescription.CreateUsingCRCId(t));

                return this.proxyGeneratorBuilder.Build(interfaceType, actorEventInterfaceDescriptions);
            }
        }
    }
}
