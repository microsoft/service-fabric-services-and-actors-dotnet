// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for NodeTransitionType.
    /// </summary>
    public enum NodeTransitionType
    {
        /// <summary>
        /// Reserved.  Do not pass into API.
        /// </summary>
        Invalid,

        /// <summary>
        /// Transition a stopped node to up.
        /// </summary>
        Start,

        /// <summary>
        /// Transition an up node to stopped.
        /// </summary>
        Stop,
    }
}
