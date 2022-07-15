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
    [KnownType(typeof(List<KeyValuePair>))]
    public class EnumerationResponse
    {
        /// <summary>
        /// Gets or sets the key value pairs.
        /// </summary>
        [DataMember]
        public List<KeyValuePair> KeyValuePairs { get; set; }

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
    }
}
