// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    /// <summary>
    /// This enumeration specifies how the target replica or instance should be chosen
    /// when creating a communication channel for a particular partition.
    /// </summary>
    public enum TargetReplicaSelector
    {
        /// <summary>
        /// This specifies the default option for this enum.
        /// If the service partition is stateful, this indicates that communication channel
        /// should be established to the primary replica.
        /// If the service partition is stateless, this indicates that the communication channel
        /// should be established to a random stateless instance.
        /// </summary>
        Default = 0,

        /// <summary>
        /// This specifies the default value of this enum for stateless service partitions. This indicates
        /// that the communication channel should be established to a random stateless instance.
        /// </summary>
        RandomInstance = 0,

        /// <summary>
        /// This specifies the default value of this enum for stateful service partitions. This indicates
        /// that the communication channel should be established to the primary replica.
        /// </summary>
        PrimaryReplica = 0,

        /// <summary>
        /// For stateful service partitions, this indicates that communication channel can be
        /// established for to any replica chosen in random - (i.e) primary or secondary.
        /// This is not valid for stateless service partitions
        /// </summary>
        RandomReplica = 1,

        /// <summary>
        /// For stateful service partitions, this indicates that communication channel can be
        /// established for to any secondary replica chosen in random.
        /// This is not valid for stateless service partitions
        /// </summary>
        RandomSecondaryReplica = 2,
    }
}
