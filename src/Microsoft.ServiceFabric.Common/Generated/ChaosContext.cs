// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a map, which is a collection of (string, string) type key-value pairs. The map can be used to record
    /// information about
    /// the Chaos run. There cannot be more than 100 such pairs and each string (key or value) can be at most 4095
    /// characters long.
    /// This map is set by the starter of the Chaos run to optionally store the context about the specific run.
    /// </summary>
    public partial class ChaosContext
    {
        /// <summary>
        /// Initializes a new instance of the ChaosContext class.
        /// </summary>
        /// <param name="map">Describes a map that contains a collection of ChaosContextMapItem's.
        /// </param>
        public ChaosContext(
            IReadOnlyDictionary<string, string> map = default(IReadOnlyDictionary<string, string>))
        {
            this.Map = map;
        }

        /// <summary>
        /// Gets a map that contains a collection of ChaosContextMapItem's.
        /// </summary>
        public IReadOnlyDictionary<string, string> Map { get; }
    }
}
