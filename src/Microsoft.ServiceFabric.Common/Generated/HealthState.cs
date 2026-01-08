// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for HealthState.
    /// </summary>
    public enum HealthState
    {
        /// <summary>
        /// Indicates an invalid health state. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the health state is okay. The value is 1.
        /// </summary>
        Ok,

        /// <summary>
        /// Indicates the health state is at a warning level. The value is 2.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicates the health state is at an error level. Error health state should be investigated, as they can impact the
        /// correct functionality of the cluster. The value is 3.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates an unknown health status. The value is 65535.
        /// </summary>
        Unknown,
    }
}
