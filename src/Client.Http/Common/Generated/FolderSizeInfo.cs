// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information of a image store folder size
    /// </summary>
    public partial class FolderSizeInfo
    {
        /// <summary>
        /// Initializes a new instance of the FolderSizeInfo class.
        /// </summary>
        /// <param name="storeRelativePath">The remote location within image store. This path is relative to the image store
        /// root.</param>
        /// <param name="folderSize">The size of folder in bytes.</param>
        public FolderSizeInfo(
            string storeRelativePath = default(string),
            string folderSize = default(string))
        {
            this.StoreRelativePath = storeRelativePath;
            this.FolderSize = folderSize;
        }

        /// <summary>
        /// Gets the remote location within image store. This path is relative to the image store root.
        /// </summary>
        public string StoreRelativePath { get; }

        /// <summary>
        /// Gets the size of folder in bytes.
        /// </summary>
        public string FolderSize { get; }
    }
}
