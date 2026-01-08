// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes basic retention policy.
    /// </summary>
    public partial class BasicRetentionPolicyDescription : RetentionPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the BasicRetentionPolicyDescription class.
        /// </summary>
        /// <param name="retentionDuration">It is the minimum duration for which a backup created, will remain stored in the
        /// storage and might get deleted after that span of time. It should be specified in ISO8601 format.</param>
        /// <param name="minimumNumberOfBackups">It is the minimum number of backups to be retained at any point of time. If
        /// specified with a non zero value, backups will not be deleted even if the backups have gone past retention duration
        /// and have number of backups less than or equal to it.</param>
        public BasicRetentionPolicyDescription(
            TimeSpan? retentionDuration,
            int? minimumNumberOfBackups = default(int?))
            : base(
                Common.RetentionPolicyType.Basic)
        {
            retentionDuration.ThrowIfNull(nameof(retentionDuration));
            minimumNumberOfBackups?.ThrowIfLessThan("minimumNumberOfBackups", 0);
            this.RetentionDuration = retentionDuration;
            this.MinimumNumberOfBackups = minimumNumberOfBackups;
        }

        /// <summary>
        /// Gets it is the minimum duration for which a backup created, will remain stored in the storage and might get deleted
        /// after that span of time. It should be specified in ISO8601 format.
        /// </summary>
        public TimeSpan? RetentionDuration { get; }

        /// <summary>
        /// Gets it is the minimum number of backups to be retained at any point of time. If specified with a non zero value,
        /// backups will not be deleted even if the backups have gone past retention duration and have number of backups less
        /// than or equal to it.
        /// </summary>
        public int? MinimumNumberOfBackups { get; }
    }
}
