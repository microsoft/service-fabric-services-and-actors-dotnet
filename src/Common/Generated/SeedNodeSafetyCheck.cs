// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a safety check for the seed nodes being performed by service fabric before continuing with node level
    /// operations.
    /// </summary>
    public partial class SeedNodeSafetyCheck : SafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the SeedNodeSafetyCheck class.
        /// </summary>
        public SeedNodeSafetyCheck()
            : base(
                Common.SafetyCheckKind.EnsureSeedNodeQuorum)
        {
        }
    }
}
