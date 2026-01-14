// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The default execution policy. Always restart the service if an exit occurs.
    /// </summary>
    public partial class DefaultExecutionPolicy : ExecutionPolicy
    {
        /// <summary>
        /// Initializes a new instance of the DefaultExecutionPolicy class.
        /// </summary>
        public DefaultExecutionPolicy()
            : base(
                Common.ExecutionPolicyType.Default)
        {
        }
    }
}
