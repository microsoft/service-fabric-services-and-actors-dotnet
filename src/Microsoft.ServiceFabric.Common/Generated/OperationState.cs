// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for OperationState.
    /// </summary>
    public enum OperationState
    {
        /// <summary>
        /// The operation state is invalid.
        /// </summary>
        Invalid,

        /// <summary>
        /// The operation is in progress.
        /// </summary>
        Running,

        /// <summary>
        /// The operation is rolling back internal system state because it encountered a fatal error or was cancelled by the
        /// user.  "RollingBack"     does not refer to user state.  For example, if CancelOperation is called on a command of
        /// type PartitionDataLoss, state of "RollingBack" does not mean service data is being restored (assuming the command
        /// has progressed far enough to cause data loss). It means the system is rolling back/cleaning up internal system
        /// state associated with the command.
        /// </summary>
        RollingBack,

        /// <summary>
        /// The operation has completed successfully and is no longer running.
        /// </summary>
        Completed,

        /// <summary>
        /// The operation has failed and is no longer running.
        /// </summary>
        Faulted,

        /// <summary>
        /// The operation was cancelled by the user using CancelOperation, and is no longer running.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The operation was cancelled by the user using CancelOperation, with the force parameter set to true.  It is no
        /// longer running.  Refer to CancelOperation for more details.
        /// </summary>
        ForceCancelled,
    }
}
