// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    internal sealed class ServiceProxyGeneratorBuilder : ProxyGeneratorBuilder<ServiceProxyGenerator, ServiceProxy>
    {
        public ServiceProxyGeneratorBuilder(ICodeBuilder codeBuilder) : base(codeBuilder)
        {
        }

        protected  override ServiceProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
            Type proxyActivatorType)
        {
            return new ServiceProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType));
        }
    }
}
