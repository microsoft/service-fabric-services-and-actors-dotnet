// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for UpgradeSortOrder.
    /// </summary>
    public enum UpgradeSortOrder
    {
        /// <summary>
        /// Indicates that this sort order is not valid. All Service Fabric enumerations have the invalid type. The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the default sort order (as specified in cluster manifest) will be used. The value is 1.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates that forward numeric sort order (UD names sorted as numbers) will be used. The value is 2.
        /// </summary>
        Numeric,

        /// <summary>
        /// Indicates that forward lexicographical sort order (UD names sorted as strings) will be used. The value is 3.
        /// </summary>
        Lexicographical,

        /// <summary>
        /// Indicates that reverse numeric sort order (UD names sorted as numbers) will be used. The value is 4.
        /// </summary>
        ReverseNumeric,

        /// <summary>
        /// Indicates that reverse lexicographical sort order (UD names sorted as strings) will be used. The value is 5.
        /// </summary>
        ReverseLexicographical,
    }
}
