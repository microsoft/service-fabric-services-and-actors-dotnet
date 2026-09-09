// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;

    /// <summary>
    /// Represents reliable state managed by an <see cref="IReliableStateManager"/>.
    /// </summary>
    public interface IReliableState
    {
        /// <summary>
        /// Gets the name of this reliable state.
        /// </summary>
        /// <remarks>
        /// The name uniquely identifies this reliable state within its owning <see cref="IReliableStateManager"/>,
        /// across <see cref="IReliableState"/> types, including unrelated types.
        /// </remarks>
        Uri Name { get; }
    }
}
