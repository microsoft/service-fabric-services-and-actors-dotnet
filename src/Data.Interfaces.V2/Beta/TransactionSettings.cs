// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.ServiceFabric.Data.Beta
{
        
    /// <summary>
    /// Defines isolation levels for single-entity reads on a <see cref="ReplicaRole.Primary"/> replica within an <see cref="ITransaction"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// (Beta) Not for production use - API is subject to change in the future.
    /// </para>
    /// <para>
    /// Multi-entity reads, such as count and enumeration, and all reads on <see cref="ReplicaRole.ActiveSecondary"/> replicas
    /// always use <see cref="Snapshot"/> regardless of this setting.
    /// </para>
    /// </remarks>
    public enum IsolationLevel
    {
        /// <summary>
        /// Holds read locks on the entities read on the primary until the transaction completes, preventing concurrent
        /// modification of those entities. This is the default.
        /// </summary>
        ReadRepeatable = 0,

        /// <summary>
        /// Reads each entity from a consistent snapshot established when snapshot reading begins, without acquiring read locks.
        /// </summary>
        Snapshot = 1
    }
    
}
