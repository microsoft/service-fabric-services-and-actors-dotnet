// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;

    internal class ActorProxyGeneratorBuilder : ProxyGeneratorBuilder<ActorProxyGeneratorWith, ActorProxy>
    {
        public ActorProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
        }

        protected override ActorProxyGeneratorWith CreateProxyGenerator(
            Type proxyInterfaceType,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult>
                methodBodyTypesResultsMap,
            Type proxyActivatorType)
        {
            var requestBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetRequestBodyTypes());

            var responseBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetResponseBodyTypes());

            return new ActorProxyGeneratorWith(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType),
                requestBodyTypes,
                responseBodyTypes);
        }
    }
}
