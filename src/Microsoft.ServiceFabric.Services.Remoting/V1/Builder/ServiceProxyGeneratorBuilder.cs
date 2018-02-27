// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal sealed class ServiceProxyGeneratorBuilder : ProxyGeneratorBuilder<ServiceProxyGeneratorWith, ServiceProxy>
    {
        public ServiceProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        { }

        protected override ServiceProxyGeneratorWith CreateProxyGenerator(
            Type proxyInterfaceType,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap,
            Type proxyActivatorType)
        {
            var requestBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetRequestBodyTypes());

            var responseBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.Id,
                item => item.Value.GetResponseBodyTypes());

            return new ServiceProxyGeneratorWith(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType),
                requestBodyTypes,
                responseBodyTypes);
        }
    }
}
