// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about an image store upload session. A session is associated with a relative path in the image store.
    /// </summary>
    public partial class UploadSessionInfo
    {
        /// <summary>
        /// Initializes a new instance of the UploadSessionInfo class.
        /// </summary>
        /// <param name="storeRelativePath">The remote location within image store. This path is relative to the image store
        /// root.</param>
        /// <param name="sessionId">A unique ID of the upload session. A session ID can be reused only if the session was
        /// committed or removed.</param>
        /// <param name="modifiedDate">The date and time when the upload session was last modified.</param>
        /// <param name="fileSize">The size in bytes of the uploading file.</param>
        /// <param name="expectedRanges">List of chunk ranges that image store has not received yet.</param>
        public UploadSessionInfo(
            string storeRelativePath = default(string),
            Guid? sessionId = default(Guid?),
            DateTime? modifiedDate = default(DateTime?),
            string fileSize = default(string),
            IEnumerable<UploadChunkRange> expectedRanges = default(IEnumerable<UploadChunkRange>))
        {
            this.StoreRelativePath = storeRelativePath;
            this.SessionId = sessionId;
            this.ModifiedDate = modifiedDate;
            this.FileSize = fileSize;
            this.ExpectedRanges = expectedRanges;
        }

        /// <summary>
        /// Gets the remote location within image store. This path is relative to the image store root.
        /// </summary>
        public string StoreRelativePath { get; }

        /// <summary>
        /// Gets a unique ID of the upload session. A session ID can be reused only if the session was committed or removed.
        /// </summary>
        public Guid? SessionId { get; }

        /// <summary>
        /// Gets the date and time when the upload session was last modified.
        /// </summary>
        public DateTime? ModifiedDate { get; }

        /// <summary>
        /// Gets the size in bytes of the uploading file.
        /// </summary>
        public string FileSize { get; }

        /// <summary>
        /// Gets list of chunk ranges that image store has not received yet.
        /// </summary>
        public IEnumerable<UploadChunkRange> ExpectedRanges { get; }
    }
}
