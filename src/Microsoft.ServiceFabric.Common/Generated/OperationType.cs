// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for OperationType.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// The operation state is invalid.
        /// </summary>
        Invalid,

        /// <summary>
        /// An operation started using the StartDataLoss API.
        /// </summary>
        PartitionDataLoss,

        /// <summary>
        /// An operation started using the StartQuorumLoss API.
        /// </summary>
        PartitionQuorumLoss,

        /// <summary>
        /// An operation started using the StartPartitionRestart API.
        /// </summary>
        PartitionRestart,

        /// <summary>
        /// An operation started using the StartNodeTransition API.
        /// </summary>
        NodeTransition,
    }
}
