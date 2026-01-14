// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for AutoScalingTriggerKind.
    /// </summary>
    public enum AutoScalingTriggerKind
    {
        /// <summary>
        /// Indicates that scaling should be performed based on average load of all replicas in the service.
        /// </summary>
        AverageLoad,
    }
}
