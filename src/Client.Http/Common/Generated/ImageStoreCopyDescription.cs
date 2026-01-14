// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about how to copy image store content from one image store relative path to another image store
    /// relative path.
    /// </summary>
    public partial class ImageStoreCopyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ImageStoreCopyDescription class.
        /// </summary>
        /// <param name="remoteSource">The relative path of source image store content to be copied from.</param>
        /// <param name="remoteDestination">The relative path of destination image store content to be copied to.</param>
        /// <param name="skipFiles">The list of the file names to be skipped for copying.</param>
        /// <param name="checkMarkFile">Indicates whether to check mark file during copying. The property is true if checking
        /// mark file is required, false otherwise. The mark file is used to check whether the folder is well constructed. If
        /// the property is true and mark file does not exist, the copy is skipped.</param>
        public ImageStoreCopyDescription(
            string remoteSource,
            string remoteDestination,
            IEnumerable<string> skipFiles = default(IEnumerable<string>),
            bool? checkMarkFile = default(bool?))
        {
            remoteSource.ThrowIfNull(nameof(remoteSource));
            remoteDestination.ThrowIfNull(nameof(remoteDestination));
            this.RemoteSource = remoteSource;
            this.RemoteDestination = remoteDestination;
            this.SkipFiles = skipFiles;
            this.CheckMarkFile = checkMarkFile;
        }

        /// <summary>
        /// Gets the relative path of source image store content to be copied from.
        /// </summary>
        public string RemoteSource { get; }

        /// <summary>
        /// Gets the relative path of destination image store content to be copied to.
        /// </summary>
        public string RemoteDestination { get; }

        /// <summary>
        /// Gets the list of the file names to be skipped for copying.
        /// </summary>
        public IEnumerable<string> SkipFiles { get; }

        /// <summary>
        /// Gets indicates whether to check mark file during copying. The property is true if checking mark file is required,
        /// false otherwise. The mark file is used to check whether the folder is well constructed. If the property is true and
        /// mark file does not exist, the copy is skipped.
        /// </summary>
        public bool? CheckMarkFile { get; }
    }
}
