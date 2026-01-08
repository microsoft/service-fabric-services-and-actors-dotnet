// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a safety check performed by service fabric before continuing with the operations. These checks ensure
    /// the availability of the service and the reliability of the state.
    /// </summary>
    public abstract partial class SafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the SafetyCheck class.
        /// </summary>
        /// <param name="kind">The kind of safety check performed by service fabric before continuing with the operations.
        /// These checks ensure the availability of the service and the reliability of the state. Following are the kinds of
        /// safety checks.</param>
        protected SafetyCheck(
            SafetyCheckKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of safety check performed by service fabric before continuing with the operations. These checks
        /// ensure the availability of the service and the reliability of the state. Following are the kinds of safety checks.
        /// </summary>
        public SafetyCheckKind? Kind { get; }
    }
}
