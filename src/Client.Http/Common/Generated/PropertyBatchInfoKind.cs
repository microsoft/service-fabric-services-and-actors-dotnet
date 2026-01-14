// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PropertyBatchInfoKind.
    /// </summary>
    public enum PropertyBatchInfoKind
    {
        /// <summary>
        /// Indicates the property batch info is invalid. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// The property batch succeeded.
        /// </summary>
        Successful,

        /// <summary>
        /// The property batch failed.
        /// </summary>
        Failed,
    }
}
