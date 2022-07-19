// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Migration state response.
    /// </summary>
    [DataContract]
    public class EnumerationResponse
    {
        /// <summary>
        /// Gets the key value pairs.
        /// </summary>
        [DataMember]
        public List<KeyValuePair> KeyValuePairs { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the end of response is reached or not.
        /// </summary>
        [DataMember]
        public bool EndSequenceNumberReached { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the actorids are resolved from state storage key.
        /// </summary>
        [DataMember]
        public bool ResolveActorIdsForStateKVPairs { get; set; }

                /// <summary>
        /// Gets the hash for the keys in the reponse.
        /// </summary>
        [DataMember]
        public byte[] KeyHash { get; internal set; }

        /// <summary>
        /// Gets the hash for the values in the response.
        /// </summary>
        [DataMember]
        public byte[] ValueHash { get; internal set; }
    }
}
