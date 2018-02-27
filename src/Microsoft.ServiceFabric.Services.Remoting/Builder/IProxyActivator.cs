// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    /// <summary>
    /// Interface to create <see cref="ProxyBase"/> objects.
    /// </summary>
    public interface IProxyActivator
    {
        /// <summary>
        /// Create the instance of the generated proxy type.
        /// </summary>
        /// <returns>An instance of the generated proxy as <see cref="ProxyBase"/></returns>
        Builder.ProxyBase CreateInstance();
    }
}
