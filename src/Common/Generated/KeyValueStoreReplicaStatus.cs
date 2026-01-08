// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Key value store related information for the replica.
    /// </summary>
    public partial class KeyValueStoreReplicaStatus : ReplicaStatusBase
    {
        /// <summary>
        /// Initializes a new instance of the KeyValueStoreReplicaStatus class.
        /// </summary>
        /// <param name="databaseRowCountEstimate">Value indicating the estimated number of rows in the underlying
        /// database.</param>
        /// <param name="databaseLogicalSizeEstimate">Value indicating the estimated size of the underlying database.</param>
        /// <param name="copyNotificationCurrentKeyFilter">Value indicating the latest key-prefix filter applied to enumeration
        /// during the callback. Null if there is no pending callback.</param>
        /// <param name="copyNotificationCurrentProgress">Value indicating the latest number of keys enumerated during the
        /// callback. 0 if there is no pending callback.</param>
        /// <param name="statusDetails">Value indicating the current status details of the replica.</param>
        public KeyValueStoreReplicaStatus(
            string databaseRowCountEstimate = default(string),
            string databaseLogicalSizeEstimate = default(string),
            string copyNotificationCurrentKeyFilter = default(string),
            string copyNotificationCurrentProgress = default(string),
            string statusDetails = default(string))
            : base(
                Common.ReplicaKind.KeyValueStore)
        {
            this.DatabaseRowCountEstimate = databaseRowCountEstimate;
            this.DatabaseLogicalSizeEstimate = databaseLogicalSizeEstimate;
            this.CopyNotificationCurrentKeyFilter = copyNotificationCurrentKeyFilter;
            this.CopyNotificationCurrentProgress = copyNotificationCurrentProgress;
            this.StatusDetails = statusDetails;
        }

        /// <summary>
        /// Gets value indicating the estimated number of rows in the underlying database.
        /// </summary>
        public string DatabaseRowCountEstimate { get; }

        /// <summary>
        /// Gets value indicating the estimated size of the underlying database.
        /// </summary>
        public string DatabaseLogicalSizeEstimate { get; }

        /// <summary>
        /// Gets value indicating the latest key-prefix filter applied to enumeration during the callback. Null if there is no
        /// pending callback.
        /// </summary>
        public string CopyNotificationCurrentKeyFilter { get; }

        /// <summary>
        /// Gets value indicating the latest number of keys enumerated during the callback. 0 if there is no pending callback.
        /// </summary>
        public string CopyNotificationCurrentProgress { get; }

        /// <summary>
        /// Gets value indicating the current status details of the replica.
        /// </summary>
        public string StatusDetails { get; }
    }
}
