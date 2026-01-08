// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Copies image store content internally
    /// </summary>
    [Cmdlet(VerbsCommon.Copy, "SFImageStoreContent")]
    public partial class CopyImageStoreContentCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets RemoteSource. The relative path of source image store content to be copied from.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string RemoteSource { get; set; }

        /// <summary>
        /// Gets or sets RemoteDestination. The relative path of destination image store content to be copied to.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string RemoteDestination { get; set; }

        /// <summary>
        /// Gets or sets SkipFiles. The list of the file names to be skipped for copying.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public IEnumerable<string> SkipFiles { get; set; }

        /// <summary>
        /// Gets or sets CheckMarkFile. Indicates whether to check mark file during copying. The property is true if checking
        /// mark file is required, false otherwise. The mark file is used to check whether the folder is well constructed. If
        /// the property is true and mark file does not exist, the copy is skipped.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public bool? CheckMarkFile { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var imageStoreCopyDescription = new ImageStoreCopyDescription(
            remoteSource: this.RemoteSource,
            remoteDestination: this.RemoteDestination,
            skipFiles: this.SkipFiles,
            checkMarkFile: this.CheckMarkFile);

            this.ServiceFabricClient.ImageStore.CopyImageStoreContentAsync(
                imageStoreCopyDescription: imageStoreCopyDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
