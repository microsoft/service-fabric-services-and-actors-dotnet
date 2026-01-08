// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the progress of a restore operation on a partition.
    /// </summary>
    public partial class RestoreProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the RestoreProgressInfo class.
        /// </summary>
        /// <param name="restoreState">Represents the current state of the partition restore operation.
        /// . Possible values include: 'Invalid', 'Accepted', 'RestoreInProgress', 'SecondariesInProgress', 'Success',
        /// 'Failure', 'Timeout'</param>
        /// <param name="timeStampUtc">Timestamp when operation succeeded or failed.</param>
        /// <param name="restoredEpoch">Describes the epoch at which the partition is restored.</param>
        /// <param name="restoredLsn">Restored LSN.</param>
        /// <param name="failureError">Denotes the failure encountered in performing restore operation.</param>
        public RestoreProgressInfo(
            RestoreState? restoreState = default(RestoreState?),
            DateTime? timeStampUtc = default(DateTime?),
            Epoch restoredEpoch = default(Epoch),
            string restoredLsn = default(string),
            FabricErrorError failureError = default(FabricErrorError))
        {
            this.RestoreState = restoreState;
            this.TimeStampUtc = timeStampUtc;
            this.RestoredEpoch = restoredEpoch;
            this.RestoredLsn = restoredLsn;
            this.FailureError = failureError;
        }

        /// <summary>
        /// Gets represents the current state of the partition restore operation.
        /// . Possible values include: 'Invalid', 'Accepted', 'RestoreInProgress', 'SecondariesInProgress', 'Success',
        /// 'Failure', 'Timeout'
        /// </summary>
        public RestoreState? RestoreState { get; }

        /// <summary>
        /// Gets timestamp when operation succeeded or failed.
        /// </summary>
        public DateTime? TimeStampUtc { get; }

        /// <summary>
        /// Gets the epoch at which the partition is restored.
        /// </summary>
        public Epoch RestoredEpoch { get; }

        /// <summary>
        /// Gets restored LSN.
        /// </summary>
        public string RestoredLsn { get; }

        /// <summary>
        /// Gets denotes the failure encountered in performing restore operation.
        /// </summary>
        public FabricErrorError FailureError { get; }
    }
}
