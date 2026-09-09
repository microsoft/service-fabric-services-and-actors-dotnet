// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Data.Beta
{
    /// <summary>
    /// Defines a reliable state manager replica that additionally supports creating transactions with a configurable
    /// <see cref="IsolationLevel"/> for single-entity reads on the primary.
    /// </summary>
    /// <remarks>
    /// (Beta) Not for production use - API is subject to change in the future.
    /// </remarks>
    public interface IReliableStateManager2 : IReliableStateManagerReplica2
    {
        /// <summary>
        /// Returns a new, started <see cref="ITransaction"/> that can be used to group operations to be performed atomically,
        /// using the specified <see cref="IsolationLevel"/> for single-entity reads on the primary.
        /// </summary>
        /// <inheritdoc path="/remarks" cref="IReliableStateManager.CreateTransaction"/>
        ITransaction CreateTransaction(IsolationLevel singleEntityIsolationLevelForPrimaryReads);
    }
}
