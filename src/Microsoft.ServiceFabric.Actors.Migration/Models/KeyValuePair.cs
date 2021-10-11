// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Models
{
    /// <summary>
    /// KeyValuePair
    /// </summary>
    public class KeyValuePair
    {
        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets or Sets Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
