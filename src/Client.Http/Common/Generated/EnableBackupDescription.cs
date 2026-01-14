// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies the parameters needed to enable periodic backup.
    /// </summary>
    public partial class EnableBackupDescription
    {
        /// <summary>
        /// Initializes a new instance of the EnableBackupDescription class.
        /// </summary>
        /// <param name="backupPolicyName">Name of the backup policy to be used for enabling periodic backups.</param>
        public EnableBackupDescription(
            string backupPolicyName)
        {
            backupPolicyName.ThrowIfNull(nameof(backupPolicyName));
            this.BackupPolicyName = backupPolicyName;
        }

        /// <summary>
        /// Gets name of the backup policy to be used for enabling periodic backups.
        /// </summary>
        public string BackupPolicyName { get; }
    }
}
