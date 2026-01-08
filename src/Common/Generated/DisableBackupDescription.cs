// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// It describes the body parameters while disabling backup of a backup entity(Application/Service/Partition).
    /// </summary>
    public partial class DisableBackupDescription
    {
        /// <summary>
        /// Initializes a new instance of the DisableBackupDescription class.
        /// </summary>
        /// <param name="cleanBackup">Boolean flag to delete backups. It can be set to true for deleting all the backups which
        /// were created for the backup entity that is getting disabled for backup.</param>
        public DisableBackupDescription(
            bool? cleanBackup)
        {
            cleanBackup.ThrowIfNull(nameof(cleanBackup));
            this.CleanBackup = cleanBackup;
        }

        /// <summary>
        /// Gets boolean flag to delete backups. It can be set to true for deleting all the backups which were created for the
        /// backup entity that is getting disabled for backup.
        /// </summary>
        public bool? CleanBackup { get; }
    }
}
