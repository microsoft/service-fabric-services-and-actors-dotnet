// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a image store file.
    /// </summary>
    public partial class FileInfo
    {
        /// <summary>
        /// Initializes a new instance of the FileInfo class.
        /// </summary>
        /// <param name="fileSize">The size of file in bytes.</param>
        /// <param name="fileVersion">Information about the version of image store file.</param>
        /// <param name="modifiedDate">The date and time when the image store file was last modified.</param>
        /// <param name="storeRelativePath">The file path relative to the image store root path.</param>
        public FileInfo(
            string fileSize = default(string),
            FileVersion fileVersion = default(FileVersion),
            DateTime? modifiedDate = default(DateTime?),
            string storeRelativePath = default(string))
        {
            this.FileSize = fileSize;
            this.FileVersion = fileVersion;
            this.ModifiedDate = modifiedDate;
            this.StoreRelativePath = storeRelativePath;
        }

        /// <summary>
        /// Gets the size of file in bytes.
        /// </summary>
        public string FileSize { get; }

        /// <summary>
        /// Gets information about the version of image store file.
        /// </summary>
        public FileVersion FileVersion { get; }

        /// <summary>
        /// Gets the date and time when the image store file was last modified.
        /// </summary>
        public DateTime? ModifiedDate { get; }

        /// <summary>
        /// Gets the file path relative to the image store root path.
        /// </summary>
        public string StoreRelativePath { get; }
    }
}
