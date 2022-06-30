// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// KeyValuePair
    /// </summary>
    [DataContract]
    public class KeyValuePair
    {
        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        [DataMember]
        public long Version { get; set; }

        /// <summary>
        /// Gets or Sets Key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember]
        public byte[] Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDeleted
        /// </summary>
        [DataMember]
        public bool IsDeleted { get; set; }
    }
}
