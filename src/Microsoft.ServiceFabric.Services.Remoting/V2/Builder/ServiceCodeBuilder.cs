// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;

    /// <summary>
    /// Singelton Class for Codegen
    /// </summary>
    internal class ServiceCodeBuilder : CodeBuilder
    {
        internal static readonly InterfaceDetailsStore InterfaceDetailsStore = new InterfaceDetailsStore();
        private static readonly ICodeBuilder Instance = new ServiceCodeBuilder();
        private static readonly object BuildLock = new object();

        private readonly MethodBodyTypesBuilder methodBodyTypesBuilder;
        private readonly MethodDispatcherBuilder<Base.V2.Builder.MethodDispatcherBase> methodDispatcherBuilder;
        private readonly ServiceProxyGeneratorBuilder proxyGeneratorBuilder;

        public ServiceCodeBuilder()
            : base(new ServiceCodeBuilderNames())
        {
            this.methodBodyTypesBuilder = new MethodBodyTypesBuilder(this);
            this.methodDispatcherBuilder = new MethodDispatcherBuilder<Base.V2.Builder.MethodDispatcherBase>(this);
            this.proxyGeneratorBuilder = new ServiceProxyGeneratorBuilder(this);
        }

        public static Base.V2.Builder.MethodDispatcherBase GetOrCreateMethodDispatcher(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                return
                    (Base.V2.Builder.MethodDispatcherBase)Instance.GetOrBuilderMethodDispatcher(serviceInterfaceType).MethodDispatcher;
            }
        }

        public static Base.V2.Builder.MethodDispatcherBase GetOrCreateMethodDispatcherForNonMarkerInterface(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                var codebuilder = (ServiceCodeBuilder)Instance;

                if (codebuilder.TryGetMethodDispatcher(serviceInterfaceType, out var result))
                {
                    return
                        (Base.V2.Builder.MethodDispatcherBase)result.MethodDispatcher;
                }

                result = codebuilder.BuildMethodDispatcherForNonServiceInterface(serviceInterfaceType);
                codebuilder.UpdateMethodDispatcherBuildMap(serviceInterfaceType, result);

                return
                    (Base.V2.Builder.MethodDispatcherBase)result.MethodDispatcher;
            }
        }

        internal static ServiceProxyGenerator GetOrCreateProxyGenerator(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                return (ServiceProxyGenerator)Instance.GetOrBuildProxyGenerator(serviceInterfaceType).ProxyGenerator;
            }
        }

        internal static ServiceProxyGenerator GetOrCreateProxyGeneratorForNonServiceInterface(Type serviceInterfaceType)
        {
            lock (BuildLock)
            {
                var codebuilder = (ServiceCodeBuilder)Instance;

                if (codebuilder.TryGetProxyGenerator(serviceInterfaceType, out var result))
                {
                    return (ServiceProxyGenerator)result.ProxyGenerator;
                }

                result = codebuilder.BuildProxyGeneratorForNonMarkerInterface(serviceInterfaceType);
                codebuilder.UpdateProxyGeneratorMap(serviceInterfaceType, result);

                return (ServiceProxyGenerator)result.ProxyGenerator;
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

        internal ProxyGeneratorBuildResult BuildProxyGeneratorForNonMarkerInterface(Type interfaceType)
        {
            // create all base interfaces that this interface derives from
            var serviceInterfaces = new List<Type>() { };
            serviceInterfaces.AddRange(interfaceType.GetAllBaseInterfaces());

            // create interface descriptions for all interfaces
            var servicenterfaceDescriptions = serviceInterfaces.Select<Type, InterfaceDescription>(
                (t) => ServiceInterfaceDescription.CreateUsingCRCId(t, false));

            var res = this.CreateProxyGeneratorBuildResult(interfaceType, servicenterfaceDescriptions);

            return res;
        }

        protected override MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType)
        {
            var servicenterfaceDescription = ServiceInterfaceDescription.CreateUsingCRCId(interfaceType, true);
            var res = this.BuildMethodDispatcherResult(servicenterfaceDescription);
            return res;
        }

        protected override MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType)
        {
            // Interface Check is already performat at builidng Dispatcher and proxy level.
            var servicenterfaceDescription = ServiceInterfaceDescription.CreateUsingCRCId(interfaceType, false);
            var result = this.methodBodyTypesBuilder.Build(servicenterfaceDescription);
            InterfaceDetailsStore.UpdateKnownTypeDetail(servicenterfaceDescription, result);
            return result;
        }

        protected override ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType)
        {
            // create all service interfaces that this interface derives from
            var serviceInterfaces = new List<Type>() { interfaceType };
            serviceInterfaces.AddRange(interfaceType.GetServiceInterfaces());

            // create interface descriptions for all interfaces
            var servicenterfaceDescriptions = serviceInterfaces.Select<Type, InterfaceDescription>(
                (t) => ServiceInterfaceDescription.CreateUsingCRCId(t, true));

            var res = this.CreateProxyGeneratorBuildResult(interfaceType, servicenterfaceDescriptions);
            return res;
        }

        private MethodDispatcherBuildResult BuildMethodDispatcherForNonServiceInterface(Type interfaceType)
        {
            var servicenterfaceDescription = ServiceInterfaceDescription.CreateUsingCRCId(interfaceType, false);
            return this.BuildMethodDispatcherResult(servicenterfaceDescription);
        }

        private MethodDispatcherBuildResult BuildMethodDispatcherResult(ServiceInterfaceDescription servicenterfaceDescription)
        {
            return this.methodDispatcherBuilder.Build(servicenterfaceDescription);
        }

        private ProxyGeneratorBuildResult CreateProxyGeneratorBuildResult(
            Type interfaceType,
            IEnumerable<InterfaceDescription> servicenterfaceDescriptions)
        {
           return this.proxyGeneratorBuilder.Build(interfaceType, servicenterfaceDescriptions);
        }
    }
}
