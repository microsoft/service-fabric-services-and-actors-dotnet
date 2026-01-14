// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PropertyValueKind.
    /// </summary>
    public enum PropertyValueKind
    {
        /// <summary>
        /// Indicates the property is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The data inside the property is a binary blob. The value is 1.
        /// </summary>
        Binary,

        /// <summary>
        /// The data inside the property is an int64. The value is 2.
        /// </summary>
        Int64,

        /// <summary>
        /// The data inside the property is a double. The value is 3.
        /// </summary>
        Double,

        /// <summary>
        /// The data inside the property is a string. The value is 4.
        /// </summary>
        String,

        /// <summary>
        /// The data inside the property is a guid. The value is 5.
        /// </summary>
        Guid,
    }
}
