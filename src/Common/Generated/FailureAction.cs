// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for FailureAction.
    /// </summary>
    public enum FailureAction
    {
        /// <summary>
        /// Indicates the failure action is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The upgrade will start rolling back automatically. The value is 1.
        /// </summary>
        Rollback,

        /// <summary>
        /// The upgrade will switch to UnmonitoredManual upgrade mode. The value is 2.
        /// </summary>
        Manual,
    }
}
