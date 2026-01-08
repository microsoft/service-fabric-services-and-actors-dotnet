// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ExecutionPolicyType.
    /// </summary>
    public enum ExecutionPolicyType
    {
        /// <summary>
        /// Indicates the default execution policy, always restart the service if an exit occurs.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates that the service will perform its desired operation and complete successfully. If the service encounters
        /// failure, it will restarted based on restart policy specified. If the service completes its operation successfully,
        /// it will not be restarted again.
        /// </summary>
        RunToCompletion,
    }
}
