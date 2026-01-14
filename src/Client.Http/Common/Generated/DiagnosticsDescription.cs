// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the diagnostics options available
    /// </summary>
    public partial class DiagnosticsDescription
    {
        /// <summary>
        /// Initializes a new instance of the DiagnosticsDescription class.
        /// </summary>
        /// <param name="sinks">List of supported sinks that can be referenced.</param>
        /// <param name="enabled">Status of whether or not sinks are enabled.</param>
        /// <param name="defaultSinkRefs">The sinks to be used if diagnostics is enabled. Sink choices can be overridden at the
        /// service and code package level.</param>
        public DiagnosticsDescription(
            IEnumerable<DiagnosticsSinkProperties> sinks = default(IEnumerable<DiagnosticsSinkProperties>),
            bool? enabled = default(bool?),
            IEnumerable<string> defaultSinkRefs = default(IEnumerable<string>))
        {
            this.Sinks = sinks;
            this.Enabled = enabled;
            this.DefaultSinkRefs = defaultSinkRefs;
        }

        /// <summary>
        /// Gets list of supported sinks that can be referenced.
        /// </summary>
        public IEnumerable<DiagnosticsSinkProperties> Sinks { get; }

        /// <summary>
        /// Gets status of whether or not sinks are enabled.
        /// </summary>
        public bool? Enabled { get; }

        /// <summary>
        /// Gets the sinks to be used if diagnostics is enabled. Sink choices can be overridden at the service and code package
        /// level.
        /// </summary>
        public IEnumerable<string> DefaultSinkRefs { get; }
    }
}
