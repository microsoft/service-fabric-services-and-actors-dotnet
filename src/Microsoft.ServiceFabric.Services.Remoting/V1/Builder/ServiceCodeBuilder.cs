// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;

    internal class ServiceCodeBuilder : CodeBuilder
    {
        private static readonly ICodeBuilder Instance = new ServiceCodeBuilder();
        private static readonly object BuildLock = new object();

        private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;
        private readonly MethodDispatcherBuilder<ServiceMethodDispatcherBase> methodDispatcherBuilder;
        private readonly ServiceProxyGeneratorBuilder proxyGeneratorBuilder;

        public ServiceCodeBuilder()
            : base(new ServiceCodeBuilderNames())
        {
            this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
            this.methodDispatcherBuilder = new MethodDispatcherBuilder<ServiceMethodDispatcherBase>(this);
            this.proxyGeneratorBuilder = new ServiceProxyGeneratorBuilder(this);
        }

        public static ServiceProxyGeneratorWith GetOrCreateProxyGenerator(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                return (ServiceProxyGeneratorWith)Instance.GetOrBuildProxyGenerator(serviceInterfaceType).ProxyGenerator;
            }
        }

        public static ServiceMethodDispatcherBase GetOrCreateMethodDispatcher(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                return (ServiceMethodDispatcherBase)Instance.GetOrBuilderMethodDispatcher(serviceInterfaceType).MethodDispatcher;
            }
        }

        protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
        {
            return this.methodDispatcherBuilder.Build(ServiceInterfaceDescription.Create(interfaceType));
        }

        protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
        {
            return this.methodBodyTypesBuilder.Build(ServiceInterfaceDescription.Create(interfaceType));
        }

        protected override ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType)
        {
            // create all service interfaces that this interface derives from
            var serviceInterfaces = new List<Type>() { interfaceType };
            serviceInterfaces.AddRange(interfaceType.GetServiceInterfaces());

            // create interface descriptions for all interfaces
            var servicenterfaceDescriptions = serviceInterfaces.Select<Type, InterfaceDescription>(
                (t) => ServiceInterfaceDescription.Create(t));

            return this.proxyGeneratorBuilder.Build(interfaceType, servicenterfaceDescriptions);
        }
    }
}
