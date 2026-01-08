// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the disk
    /// </summary>
    public partial class DiskInfo
    {
        /// <summary>
        /// Initializes a new instance of the DiskInfo class.
        /// </summary>
        /// <param name="capacity">the disk size in bytes</param>
        /// <param name="availableSpace">the available disk space in bytes</param>
        public DiskInfo(
            string capacity = default(string),
            string availableSpace = default(string))
        {
            this.Capacity = capacity;
            this.AvailableSpace = availableSpace;
        }

        /// <summary>
        /// Gets the disk size in bytes
        /// </summary>
        public string Capacity { get; }

        /// <summary>
        /// Gets the available disk space in bytes
        /// </summary>
        public string AvailableSpace { get; }
    }
}
