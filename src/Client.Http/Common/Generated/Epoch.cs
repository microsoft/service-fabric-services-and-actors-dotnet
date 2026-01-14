// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An Epoch is a configuration number for the partition as a whole. When the configuration of the replica set changes,
    /// for example when the Primary replica changes, the operations that are replicated from the new Primary replica are
    /// said to be a new Epoch from the ones which were sent by the old Primary replica.
    /// </summary>
    public partial class Epoch
    {
        /// <summary>
        /// Initializes a new instance of the Epoch class.
        /// </summary>
        /// <param name="configurationVersion">The current configuration number of this Epoch. The configuration number is an
        /// increasing value that is updated whenever the configuration of this replica set changes.</param>
        /// <param name="dataLossVersion">The current data loss number of this Epoch. The data loss number property is an
        /// increasing value which is updated whenever data loss is suspected, as when loss of a quorum of replicas in the
        /// replica set that includes the Primary replica.</param>
        public Epoch(
            string configurationVersion = default(string),
            string dataLossVersion = default(string))
        {
            this.ConfigurationVersion = configurationVersion;
            this.DataLossVersion = dataLossVersion;
        }

        /// <summary>
        /// Gets the current configuration number of this Epoch. The configuration number is an increasing value that is
        /// updated whenever the configuration of this replica set changes.
        /// </summary>
        public string ConfigurationVersion { get; }

        /// <summary>
        /// Gets the current data loss number of this Epoch. The data loss number property is an increasing value which is
        /// updated whenever data loss is suspected, as when loss of a quorum of replicas in the replica set that includes the
        /// Primary replica.
        /// </summary>
        public string DataLossVersion { get; }
    }
}
