// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the ImageStore's resource usage
    /// </summary>
    public partial class ImageStoreInfo
    {
        /// <summary>
        /// Initializes a new instance of the ImageStoreInfo class.
        /// </summary>
        /// <param name="diskInfo">disk capacity and available disk space on the node where the ImageStore primary is
        /// placed.</param>
        /// <param name="usedByMetadata">the ImageStore's file system usage for metadata.</param>
        /// <param name="usedByStaging">The ImageStore's file system usage for staging files that are being uploaded.</param>
        /// <param name="usedByCopy">the ImageStore's file system usage for copied application and cluster packages. [Removing
        /// application and cluster
        /// packages](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-deleteimagestorecontent) will free up this
        /// space.</param>
        /// <param name="usedByRegister">the ImageStore's file system usage for registered and cluster packages. [Unregistering
        /// application](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-unprovisionapplicationtype) and
        /// [cluster packages](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-unprovisionapplicationtype) will
        /// free up this space.</param>
        public ImageStoreInfo(
            DiskInfo diskInfo = default(DiskInfo),
            UsageInfo usedByMetadata = default(UsageInfo),
            UsageInfo usedByStaging = default(UsageInfo),
            UsageInfo usedByCopy = default(UsageInfo),
            UsageInfo usedByRegister = default(UsageInfo))
        {
            this.DiskInfo = diskInfo;
            this.UsedByMetadata = usedByMetadata;
            this.UsedByStaging = usedByStaging;
            this.UsedByCopy = usedByCopy;
            this.UsedByRegister = usedByRegister;
        }

        /// <summary>
        /// Gets disk capacity and available disk space on the node where the ImageStore primary is placed.
        /// </summary>
        public DiskInfo DiskInfo { get; }

        /// <summary>
        /// Gets the ImageStore's file system usage for metadata.
        /// </summary>
        public UsageInfo UsedByMetadata { get; }

        /// <summary>
        /// Gets the ImageStore's file system usage for staging files that are being uploaded.
        /// </summary>
        public UsageInfo UsedByStaging { get; }

        /// <summary>
        /// Gets the ImageStore's file system usage for copied application and cluster packages. [Removing application and
        /// cluster packages](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-deleteimagestorecontent) will free
        /// up this space.
        /// </summary>
        public UsageInfo UsedByCopy { get; }

        /// <summary>
        /// Gets the ImageStore's file system usage for registered and cluster packages. [Unregistering
        /// application](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-unprovisionapplicationtype) and
        /// [cluster packages](https://docs.microsoft.com/rest/api/servicefabric/sfclient-api-unprovisionapplicationtype) will
        /// free up this space.
        /// </summary>
        public UsageInfo UsedByRegister { get; }
    }
}
