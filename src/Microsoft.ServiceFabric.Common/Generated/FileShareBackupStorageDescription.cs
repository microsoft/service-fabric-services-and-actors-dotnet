// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for file share storage used for storing or enumerating backups.
    /// </summary>
    public partial class FileShareBackupStorageDescription : BackupStorageDescription
    {
        /// <summary>
        /// Initializes a new instance of the FileShareBackupStorageDescription class.
        /// </summary>
        /// <param name="path">UNC path of the file share where to store or enumerate backups from.</param>
        /// <param name="friendlyName">Friendly name for this backup storage.</param>
        /// <param name="primaryUserName">Primary user name to access the file share.</param>
        /// <param name="primaryPassword">Primary password to access the share location.</param>
        /// <param name="secondaryUserName">Secondary user name to access the file share.</param>
        /// <param name="secondaryPassword">Secondary password to access the share location</param>
        public FileShareBackupStorageDescription(
            string path,
            string friendlyName = default(string),
            string primaryUserName = default(string),
            string primaryPassword = default(string),
            string secondaryUserName = default(string),
            string secondaryPassword = default(string))
            : base(
                Common.BackupStorageKind.FileShare,
                friendlyName)
        {
            path.ThrowIfNull(nameof(path));
            this.Path = path;
            this.PrimaryUserName = primaryUserName;
            this.PrimaryPassword = primaryPassword;
            this.SecondaryUserName = secondaryUserName;
            this.SecondaryPassword = secondaryPassword;
        }

        /// <summary>
        /// Gets UNC path of the file share where to store or enumerate backups from.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets primary user name to access the file share.
        /// </summary>
        public string PrimaryUserName { get; }

        /// <summary>
        /// Gets primary password to access the share location.
        /// </summary>
        public string PrimaryPassword { get; }

        /// <summary>
        /// Gets secondary user name to access the file share.
        /// </summary>
        public string SecondaryUserName { get; }

        /// <summary>
        /// Gets secondary password to access the share location
        /// </summary>
        public string SecondaryPassword { get; }
    }
}
