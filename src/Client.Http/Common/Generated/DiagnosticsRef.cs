// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Reference to sinks in DiagnosticsDescription.
    /// </summary>
    public partial class DiagnosticsRef
    {
        /// <summary>
        /// Initializes a new instance of the DiagnosticsRef class.
        /// </summary>
        /// <param name="enabled">Status of whether or not sinks are enabled.</param>
        /// <param name="sinkRefs">List of sinks to be used if enabled. References the list of sinks in
        /// DiagnosticsDescription.</param>
        public DiagnosticsRef(
            bool? enabled = default(bool?),
            IEnumerable<string> sinkRefs = default(IEnumerable<string>))
        {
            this.Enabled = enabled;
            this.SinkRefs = sinkRefs;
        }

        /// <summary>
        /// Gets status of whether or not sinks are enabled.
        /// </summary>
        public bool? Enabled { get; }

        /// <summary>
        /// Gets list of sinks to be used if enabled. References the list of sinks in DiagnosticsDescription.
        /// </summary>
        public IEnumerable<string> SinkRefs { get; }
    }
}
