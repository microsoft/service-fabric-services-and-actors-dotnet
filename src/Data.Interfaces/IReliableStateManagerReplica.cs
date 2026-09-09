// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Defines a reliable state manager replica that manages <see cref="IReliableState"/> and participates in the Service
    /// Fabric replica lifecycle.
    /// </summary>
    public interface IReliableStateManagerReplica : IStateProviderReplica, IReliableStateManager
    {
    }
}
