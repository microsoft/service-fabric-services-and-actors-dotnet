// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Defines a reliable state manager replica that additionally notifies the service after its state has been restored
    /// following data loss, through <see cref="IStateProviderReplica2.OnRestoreCompletedAsync"/>.
    /// </summary>
    public interface IReliableStateManagerReplica2 : IReliableStateManagerReplica, IStateProviderReplica2
    {
    }
}
