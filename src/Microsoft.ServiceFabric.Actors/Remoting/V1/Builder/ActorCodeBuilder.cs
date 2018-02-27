// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Actors.Remoting.Description;
    using Microsoft.ServiceFabric.Actors.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal class ActorCodeBuilder : CodeBuilder
    {
        private static ICodeBuilder Singleton = new ActorCodeBuilder();
        private static object BuildLock = new object();

        private readonly ICodeBuilder eventCodeBuilder;
        private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;
        private readonly Services.Remoting.V1.Builder.MethodDispatcherBuilder<ActorMethodDispatcherBase> methodDispatcherBuilder;
        private readonly ActorProxyGeneratorBuilder proxyGeneratorBuilder;

        public ActorCodeBuilder()
            : base(new ActorCodeBuilderNames())
        {
            this.eventCodeBuilder = new ActorEventCodeBuilder();
            this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
            this.methodDispatcherBuilder = new Services.Remoting.V1.Builder.MethodDispatcherBuilder<ActorMethodDispatcherBase>(this);
            this.proxyGeneratorBuilder = new ActorProxyGeneratorBuilder(this);
        }

        public static ActorProxyGeneratorWith GetOrCreateProxyGenerator(Type actorInterfaceType)
        {
            lock (BuildLock)
            {
                return (ActorProxyGeneratorWith)Singleton.GetOrBuildProxyGenerator(actorInterfaceType).ProxyGenerator;
            }
        }

        public static ActorMethodDispatcherBase GetOrCreateMethodDispatcher(Type actorInterfaceType)
        {
            lock (BuildLock)
            {
                return (ActorMethodDispatcherBase)Singleton.GetOrBuilderMethodDispatcher(actorInterfaceType).MethodDispatcher;
            }
        }

        public static ActorEventProxyGeneratorWith GetOrCreateEventProxyGenerator(Type actorEventInterfaceType)
        {
            var eventCodeBuilder = ((ActorCodeBuilder)Singleton).eventCodeBuilder;
            lock (BuildLock)
            {
                return (ActorEventProxyGeneratorWith)eventCodeBuilder.GetOrBuildProxyGenerator(actorEventInterfaceType).ProxyGenerator;
            }
        }

        protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
        {
            return this.methodDispatcherBuilder.Build(ActorInterfaceDescription.Create(interfaceType));
        }

        protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
        {
            return this.methodBodyTypesBuilder.Build(ActorInterfaceDescription.Create(interfaceType));
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
            ActorEventSubscriberManager.Singleton.RegisterEventDispatchers(actorMethodDispatcherBases);

            // create all actor interfaces that this interface derives from
            var actorInterfaces = new List<Type>() { interfaceType };
            actorInterfaces.AddRange(interfaceType.GetActorInterfaces());

            // create interface descriptions for all interfaces
            var actorInterfaceDescriptions = actorInterfaces.Select<Type, InterfaceDescription>(
                t => ActorInterfaceDescription.Create(t));

            return this.proxyGeneratorBuilder.Build(interfaceType, actorInterfaceDescriptions);
        }

        private class ActorEventCodeBuilder : CodeBuilder
        {
            private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;
            private readonly Services.Remoting.V1.Builder.MethodDispatcherBuilder<ActorMethodDispatcherBase> methodDispatcherBuilder;
            private readonly ActorEventProxyGeneratorBuilder proxyGeneratorBuilder;

            public ActorEventCodeBuilder() :
                base(new ActorEventCodeBuilderNames())
            {
                this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
                this.methodDispatcherBuilder = new Services.Remoting.V1.Builder.MethodDispatcherBuilder<ActorMethodDispatcherBase>(this);
                this.proxyGeneratorBuilder = new ActorEventProxyGeneratorBuilder(this);
            }

            protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
            {
                return this.methodDispatcherBuilder.Build(ActorEventInterfaceDescription.Create(interfaceType));
            }

            protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
            {
                return this.methodBodyTypesBuilder.Build(ActorEventInterfaceDescription.Create(interfaceType));
            }

            protected override ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType)
            {
                // get all event interfaces supported by this actorInterface and build method dispatchers for those
                var actorEventInterfaces = new[] { interfaceType };

                // create interface descriptions for all interfaces
                var actorEventInterfaceDescriptions = actorEventInterfaces.Select<Type, InterfaceDescription>(
                    t => ActorEventInterfaceDescription.Create(t));

                return this.proxyGeneratorBuilder.Build(interfaceType, actorEventInterfaceDescriptions);
            }
        }
    }
}
