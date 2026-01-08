// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the backup suspension details.
    /// </summary>
    public partial class BackupSuspensionInfo
    {
        /// <summary>
        /// Initializes a new instance of the BackupSuspensionInfo class.
        /// </summary>
        /// <param name="isSuspended">Indicates whether periodic backup is suspended at this level or not.</param>
        /// <param name="suspensionInheritedFrom">Specifies the scope at which the backup suspension was applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'</param>
        public BackupSuspensionInfo(
            bool? isSuspended = default(bool?),
            BackupSuspensionScope? suspensionInheritedFrom = default(BackupSuspensionScope?))
        {
            this.IsSuspended = isSuspended;
            this.SuspensionInheritedFrom = suspensionInheritedFrom;
        }

        /// <summary>
        /// Gets indicates whether periodic backup is suspended at this level or not.
        /// </summary>
        public bool? IsSuspended { get; }

        /// <summary>
        /// Gets specifies the scope at which the backup suspension was applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'
        /// </summary>
        public BackupSuspensionScope? SuspensionInheritedFrom { get; }
    }
}
