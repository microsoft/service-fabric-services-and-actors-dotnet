// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about current reconfiguration like phase, type, previous configuration role of replica and
    /// reconfiguration start date time.
    /// </summary>
    public partial class ReconfigurationInformation
    {
        /// <summary>
        /// Initializes a new instance of the ReconfigurationInformation class.
        /// </summary>
        /// <param name="previousConfigurationRole">Replica role before reconfiguration started. Possible values include:
        /// 'Unknown', 'None', 'Primary', 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary',
        /// 'PrimaryAuxiliary'
        /// 
        /// The role of a replica of a stateful service.
        /// </param>
        /// <param name="reconfigurationPhase">Current phase of ongoing reconfiguration. If no reconfiguration is taking place
        /// then this value will be "None". Possible values include: 'Unknown', 'None', 'Phase0', 'Phase1', 'Phase2', 'Phase3',
        /// 'Phase4', 'AbortPhaseZero'
        /// 
        /// The reconfiguration phase of a replica of a stateful service.
        /// </param>
        /// <param name="reconfigurationType">Type of current ongoing reconfiguration. If no reconfiguration is taking place
        /// then this value will be "None". Possible values include: 'Unknown', 'SwapPrimary', 'Failover', 'Other'
        /// 
        /// The type of reconfiguration for replica of a stateful service.
        /// </param>
        /// <param name="reconfigurationStartTimeUtc">Start time (in UTC) of the ongoing reconfiguration. If no reconfiguration
        /// is taking place then this value will be zero date-time.</param>
        public ReconfigurationInformation(
            ReplicaRole? previousConfigurationRole = default(ReplicaRole?),
            ReconfigurationPhase? reconfigurationPhase = default(ReconfigurationPhase?),
            ReconfigurationType? reconfigurationType = default(ReconfigurationType?),
            DateTime? reconfigurationStartTimeUtc = default(DateTime?))
        {
            this.PreviousConfigurationRole = previousConfigurationRole;
            this.ReconfigurationPhase = reconfigurationPhase;
            this.ReconfigurationType = reconfigurationType;
            this.ReconfigurationStartTimeUtc = reconfigurationStartTimeUtc;
        }

        /// <summary>
        /// Gets replica role before reconfiguration started. Possible values include: 'Unknown', 'None', 'Primary',
        /// 'IdleSecondary', 'ActiveSecondary', 'IdleAuxiliary', 'ActiveAuxiliary', 'PrimaryAuxiliary'
        /// 
        /// The role of a replica of a stateful service.
        /// </summary>
        public ReplicaRole? PreviousConfigurationRole { get; }

        /// <summary>
        /// Gets current phase of ongoing reconfiguration. If no reconfiguration is taking place then this value will be
        /// "None". Possible values include: 'Unknown', 'None', 'Phase0', 'Phase1', 'Phase2', 'Phase3', 'Phase4',
        /// 'AbortPhaseZero'
        /// 
        /// The reconfiguration phase of a replica of a stateful service.
        /// </summary>
        public ReconfigurationPhase? ReconfigurationPhase { get; }

        /// <summary>
        /// Gets type of current ongoing reconfiguration. If no reconfiguration is taking place then this value will be "None".
        /// Possible values include: 'Unknown', 'SwapPrimary', 'Failover', 'Other'
        /// 
        /// The type of reconfiguration for replica of a stateful service.
        /// </summary>
        public ReconfigurationType? ReconfigurationType { get; }

        /// <summary>
        /// Gets start time (in UTC) of the ongoing reconfiguration. If no reconfiguration is taking place then this value will
        /// be zero date-time.
        /// </summary>
        public DateTime? ReconfigurationStartTimeUtc { get; }
    }
}
