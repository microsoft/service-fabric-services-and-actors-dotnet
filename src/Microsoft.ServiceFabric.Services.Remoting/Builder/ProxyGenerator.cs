// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;

    internal abstract class ProxyGenerator
    {
        private readonly Type proxyInterfaceType;

        protected ProxyGenerator(Type proxyInterfaceType)
        {
            this.proxyInterfaceType = proxyInterfaceType;
        }

        public Type ProxyInterfaceType
        {
            get { return this.proxyInterfaceType; }
        }
    }
}
