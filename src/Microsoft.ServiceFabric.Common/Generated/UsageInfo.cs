// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about how much space and how many files in the file system the ImageStore is using in this category
    /// </summary>
    public partial class UsageInfo
    {
        /// <summary>
        /// Initializes a new instance of the UsageInfo class.
        /// </summary>
        /// <param name="usedSpace">the size of all files in this category</param>
        /// <param name="fileCount">the number of all files in this category</param>
        public UsageInfo(
            string usedSpace = default(string),
            string fileCount = default(string))
        {
            this.UsedSpace = usedSpace;
            this.FileCount = fileCount;
        }

        /// <summary>
        /// Gets the size of all files in this category
        /// </summary>
        public string UsedSpace { get; }

        /// <summary>
        /// Gets the number of all files in this category
        /// </summary>
        public string FileCount { get; }
    }
}
