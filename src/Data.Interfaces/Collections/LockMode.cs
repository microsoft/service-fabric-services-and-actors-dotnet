// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Collections
{
    /// <summary>
    /// Specifies the lock mode a read operation on an <see cref="IReliableCollection{T}"/> acquires, controlling how the read
    /// interacts with concurrent <see cref="ITransaction"/>s.
    /// </summary>
    public enum LockMode : int
    {
        /// <summary>
        /// Uses the default read locking behavior for the operation.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Acquires an update-intent lock on resources the transaction intends to update later, preventing a common
        /// form of deadlock that occurs when multiple transactions read, lock, and then attempt to update the same resources.
        /// </summary>
        Update = 1,
    }
}
