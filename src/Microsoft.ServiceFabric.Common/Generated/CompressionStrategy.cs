// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for CompressionStrategy.
    /// </summary>
    public enum CompressionStrategy
    {
        /// <summary>
        /// Use ZIP compression for backups.
        /// </summary>
        ZIP,

        /// <summary>
        /// Use Zstandard compression for backups, which provides better compression ratios.
        /// </summary>
        ZSTANDARD,
    }
}
