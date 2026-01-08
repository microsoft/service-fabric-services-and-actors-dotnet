// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Properties of a DiagnosticsSink.
    /// </summary>
    public abstract partial class DiagnosticsSinkProperties
    {
        /// <summary>
        /// Initializes a new instance of the DiagnosticsSinkProperties class.
        /// </summary>
        /// <param name="kind">The kind of DiagnosticsSink.</param>
        /// <param name="name">Name of the sink. This value is referenced by DiagnosticsReferenceDescription</param>
        /// <param name="description">A description of the sink.</param>
        protected DiagnosticsSinkProperties(
            DiagnosticsSinkKind? kind,
            string name = default(string),
            string description = default(string))
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets name of the sink. This value is referenced by DiagnosticsReferenceDescription
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a description of the sink.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the kind of DiagnosticsSink.
        /// </summary>
        public DiagnosticsSinkKind? Kind { get; }
    }
}
