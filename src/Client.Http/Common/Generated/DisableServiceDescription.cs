// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for disabling a service.
    /// </summary>
    public partial class DisableServiceDescription
    {
        /// <summary>
        /// Initializes a new instance of the DisableServiceDescription class.
        /// </summary>
        /// <param name="disableServiceFlag">Specifies the behavior when disabling a service. Possible values include:
        /// 'Invalid', 'RemoveData'</param>
        /// <param name="forceDisable">Indicates whether the service should be force-disabled, bypassing graceful replica
        /// shutdown. Force-disabling a stateful service can leave persisted state on disk that is not properly cleaned up,
        /// because replicas are terminated without a graceful shutdown.</param>
        public DisableServiceDescription(
            DisableServiceFlag? disableServiceFlag,
            bool? forceDisable = false)
        {
            disableServiceFlag.ThrowIfNull(nameof(disableServiceFlag));
            this.DisableServiceFlag = disableServiceFlag;
            this.ForceDisable = forceDisable;
        }

        /// <summary>
        /// Gets specifies the behavior when disabling a service. Possible values include: 'Invalid', 'RemoveData'
        /// </summary>
        public DisableServiceFlag? DisableServiceFlag { get; }

        /// <summary>
        /// Gets indicates whether the service should be force-disabled, bypassing graceful replica shutdown. Force-disabling a
        /// stateful service can leave persisted state on disk that is not properly cleaned up, because replicas are terminated
        /// without a graceful shutdown.
        /// </summary>
        public bool? ForceDisable { get; }
    }
}
