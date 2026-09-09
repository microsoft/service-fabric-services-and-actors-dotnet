// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for CapacityReleaseAction.
    /// </summary>
    public enum CapacityReleaseAction
    {
        /// <summary>
        /// Capacity release does not change the service target.
        /// </summary>
        None,

        /// <summary>
        /// Configures minor capacity release to target the service minimum and major capacity release to target zero.
        /// </summary>
        DropToZero,

        /// <summary>
        /// Configures minor capacity release to leave the service target unchanged and major capacity release to target the
        /// service minimum.
        /// </summary>
        DropToMin,
    }
}
