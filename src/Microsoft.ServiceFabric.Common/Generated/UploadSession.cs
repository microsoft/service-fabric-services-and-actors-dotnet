// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a image store upload session
    /// </summary>
    public partial class UploadSession
    {
        /// <summary>
        /// Initializes a new instance of the UploadSession class.
        /// </summary>
        /// <param name="uploadSessions">When querying upload session by upload session ID, the result contains only one upload
        /// session. When querying upload session by image store relative path, the result might contain multiple upload
        /// sessions.</param>
        public UploadSession(
            IEnumerable<UploadSessionInfo> uploadSessions = default(IEnumerable<UploadSessionInfo>))
        {
            this.UploadSessions = uploadSessions;
        }

        /// <summary>
        /// Gets when querying upload session by upload session ID, the result contains only one upload session. When querying
        /// upload session by image store relative path, the result might contain multiple upload sessions.
        /// </summary>
        public IEnumerable<UploadSessionInfo> UploadSessions { get; }
    }
}
