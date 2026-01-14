// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ChaosStatus.
    /// </summary>
    public enum ChaosStatus
    {
        /// <summary>
        /// Indicates an invalid Chaos status. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that Chaos is not stopped. The value is one.
        /// </summary>
        Running,

        /// <summary>
        /// Indicates that Chaos is not scheduling further faults. The value is two.
        /// </summary>
        Stopped,
    }
}
