// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper for the safety check object. Safety checks are performed by service fabric before continuing with the
    /// operations. These checks ensure the availability of the service and the reliability of the state.
    /// </summary>
    public partial class SafetyCheckWrapper
    {
        /// <summary>
        /// Initializes a new instance of the SafetyCheckWrapper class.
        /// </summary>
        /// <param name="safetyCheck">Represents a safety check performed by service fabric before continuing with the
        /// operations. These checks ensure the availability of the service and the reliability of the state.</param>
        public SafetyCheckWrapper(
            SafetyCheck safetyCheck = default(SafetyCheck))
        {
            this.SafetyCheck = safetyCheck;
        }

        /// <summary>
        /// Gets represents a safety check performed by service fabric before continuing with the operations. These checks
        /// ensure the availability of the service and the reliability of the state.
        /// </summary>
        public SafetyCheck SafetyCheck { get; }
    }
}
