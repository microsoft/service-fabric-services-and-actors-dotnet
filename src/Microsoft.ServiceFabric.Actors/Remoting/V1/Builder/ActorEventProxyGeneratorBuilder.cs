// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;

    internal class ActorEventProxyGeneratorBuilder : ProxyGeneratorBuilder<ActorEventProxyGeneratorWith, ActorEventProxy
    >
    {
        public ActorEventProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
        }

        protected override ActorEventProxyGeneratorWith CreateProxyGenerator(
            Type proxyInterfaceType,
            IDictionary<InterfaceDescription, Services.Remoting.Builder.MethodBodyTypesBuildResult>
                methodBodyTypesResultsMap,
            Type proxyActivatorType)
        {
            var requestBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetRequestBodyTypes());

            var responseBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetResponseBodyTypes());

            return new ActorEventProxyGeneratorWith(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType),
                requestBodyTypes,
                responseBodyTypes);
        }
    }
}
