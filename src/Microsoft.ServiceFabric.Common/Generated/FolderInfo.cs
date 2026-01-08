// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a image store folder. It includes how many files this folder contains and its image store
    /// relative path.
    /// </summary>
    public partial class FolderInfo
    {
        /// <summary>
        /// Initializes a new instance of the FolderInfo class.
        /// </summary>
        /// <param name="storeRelativePath">The remote location within image store. This path is relative to the image store
        /// root.</param>
        /// <param name="fileCount">The number of files from within the image store folder.</param>
        public FolderInfo(
            string storeRelativePath = default(string),
            string fileCount = default(string))
        {
            this.StoreRelativePath = storeRelativePath;
            this.FileCount = fileCount;
        }

        /// <summary>
        /// Gets the remote location within image store. This path is relative to the image store root.
        /// </summary>
        public string StoreRelativePath { get; }

        /// <summary>
        /// Gets the number of files from within the image store folder.
        /// </summary>
        public string FileCount { get; }
    }
}
