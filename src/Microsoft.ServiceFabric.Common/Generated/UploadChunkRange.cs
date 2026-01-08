// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about which portion of the file to upload.
    /// </summary>
    public partial class UploadChunkRange
    {
        /// <summary>
        /// Initializes a new instance of the UploadChunkRange class.
        /// </summary>
        /// <param name="startPosition">The start position of the portion of the file. It's represented by the number of
        /// bytes.</param>
        /// <param name="endPosition">The end position of the portion of the file. It's represented by the number of
        /// bytes.</param>
        public UploadChunkRange(
            string startPosition = default(string),
            string endPosition = default(string))
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        /// <summary>
        /// Gets the start position of the portion of the file. It's represented by the number of bytes.
        /// </summary>
        public string StartPosition { get; }

        /// <summary>
        /// Gets the end position of the portion of the file. It's represented by the number of bytes.
        /// </summary>
        public string EndPosition { get; }
    }
}
