// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for resetting a Service Fabric application.
    /// </summary>
    public partial class ResetApplicationDescription
    {
        /// <summary>
        /// Initializes a new instance of the ResetApplicationDescription class.
        /// </summary>
        /// <param name="resetMode">Specifies how the application replicas are reset. The default value is 'Graceful'. Possible
        /// values include: 'Graceful', 'Immediate'</param>
        public ResetApplicationDescription(
            ApplicationResetMode? resetMode = Common.ApplicationResetMode.Graceful)
        {
            this.ResetMode = resetMode;
        }

        /// <summary>
        /// Gets specifies how the application replicas are reset. The default value is 'Graceful'. Possible values include:
        /// 'Graceful', 'Immediate'
        /// </summary>
        public ApplicationResetMode? ResetMode { get; }
    }
}
