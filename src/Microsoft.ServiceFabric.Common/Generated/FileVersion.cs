// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the version of image store file.
    /// </summary>
    public partial class FileVersion
    {
        /// <summary>
        /// Initializes a new instance of the FileVersion class.
        /// </summary>
        /// <param name="versionNumber">The current image store version number for the file is used in image store for checking
        /// whether it need to be updated.</param>
        /// <param name="epochDataLossNumber">The epoch data loss number of image store replica when this file entry was
        /// updated or created.</param>
        /// <param name="epochConfigurationNumber">The epoch configuration version number of the image store replica when this
        /// file entry was created or updated.</param>
        public FileVersion(
            string versionNumber = default(string),
            string epochDataLossNumber = default(string),
            string epochConfigurationNumber = default(string))
        {
            this.VersionNumber = versionNumber;
            this.EpochDataLossNumber = epochDataLossNumber;
            this.EpochConfigurationNumber = epochConfigurationNumber;
        }

        /// <summary>
        /// Gets the current image store version number for the file is used in image store for checking whether it need to be
        /// updated.
        /// </summary>
        public string VersionNumber { get; }

        /// <summary>
        /// Gets the epoch data loss number of image store replica when this file entry was updated or created.
        /// </summary>
        public string EpochDataLossNumber { get; }

        /// <summary>
        /// Gets the epoch configuration version number of the image store replica when this file entry was created or updated.
        /// </summary>
        public string EpochConfigurationNumber { get; }
    }
}
