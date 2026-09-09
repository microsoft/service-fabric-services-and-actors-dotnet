// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Collections
{
    /// <summary>Specifies whether items returned during enumeration of an <see cref="IReliableCollection{T}"/> are ordered.</summary>
    public enum EnumerationMode : int
    {
        /// <summary>Returns results in arbitrary order.</summary>
        Unordered = 0,

        /// <summary>Returns results in ascending key order.</summary>
        Ordered = 1,
    }
}