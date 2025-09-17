// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    /// <summary>
    /// Defines the interface that provides settings for exception serialization.
    /// </summary>
    public interface IExceptionSerializerSettings
    {
        /// <summary>
        /// Gets maximal exception depth to be serialized for remoting transfer.
        /// </summary>
        /// <returns>Maximum exception depth for remoting transfer.</returns>
        int GetRemotingExceptionDepth();
    }
}