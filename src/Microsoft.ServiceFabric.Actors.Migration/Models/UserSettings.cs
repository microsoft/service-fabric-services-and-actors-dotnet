// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Models
{
    /// <summary>
    /// Class represneting user settings set for migration
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettings"/> class.
        /// </summary>
        /// <param name="workerCount">Number of parallel workers</param>
        /// <param name="downtimeThreshold">Downtime Threshold</param>
        /// <param name="chunkSize">Size of data per enumeration</param>
        /// <param name="itemsPerEnumeration">Items per enumeration</param>
        /// <param name="kvsEndpoint">Endpoint of source service</param>
        public UserSettings(int workerCount, long downtimeThreshold, long chunkSize, long itemsPerEnumeration, string kvsEndpoint)
        {
            this.WorkerCount = workerCount;
            this.DowntimeThreshold = downtimeThreshold;
            this.ChunkSize = chunkSize;
            this.ItemsPerEnumeration = itemsPerEnumeration;
            this.KvsEndpoint = kvsEndpoint;
        }

        /// <summary>
        /// Gets Number of parallel workers
        /// </summary>
        public int WorkerCount { get; private set; }

        /// <summary>
        /// Gets Downtime Threshold
        /// </summary>
        public long DowntimeThreshold { get; private set; }

        /// <summary>
        /// Gets Size of data per enumeration
        /// </summary>
        public long ChunkSize { get; private set; }

        /// <summary>
        /// Gets get Items per enumeration
        /// </summary>
        public long ItemsPerEnumeration { get; private set; }

        /// <summary>
        /// Gets Endpoint of source KVS service
        /// </summary>
        public string KvsEndpoint { get; private set; }
    }
}
