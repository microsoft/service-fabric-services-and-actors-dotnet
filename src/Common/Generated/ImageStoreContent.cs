// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the image store content.
    /// </summary>
    public partial class ImageStoreContent
    {
        /// <summary>
        /// Initializes a new instance of the ImageStoreContent class.
        /// </summary>
        /// <param name="storeFiles">The list of image store file info objects represents files found under the given image
        /// store relative path.</param>
        /// <param name="storeFolders">The list of image store folder info objects represents subfolders found under the given
        /// image store relative path.</param>
        public ImageStoreContent(
            IEnumerable<FileInfo> storeFiles = default(IEnumerable<FileInfo>),
            IEnumerable<FolderInfo> storeFolders = default(IEnumerable<FolderInfo>))
        {
            this.StoreFiles = storeFiles;
            this.StoreFolders = storeFolders;
        }

        /// <summary>
        /// Gets the list of image store file info objects represents files found under the given image store relative path.
        /// </summary>
        public IEnumerable<FileInfo> StoreFiles { get; }

        /// <summary>
        /// Gets the list of image store folder info objects represents subfolders found under the given image store relative
        /// path.
        /// </summary>
        public IEnumerable<FolderInfo> StoreFolders { get; }
    }
}
