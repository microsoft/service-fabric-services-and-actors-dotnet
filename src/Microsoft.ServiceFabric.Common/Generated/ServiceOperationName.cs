// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceOperationName.
    /// </summary>
    public enum ServiceOperationName
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Unknown,

        /// <summary>
        /// The service replica or instance is not going through any life-cycle changes.
        /// </summary>
        None,

        /// <summary>
        /// The service replica or instance is being opened.
        /// </summary>
        Open,

        /// <summary>
        /// The service replica is changing roles.
        /// </summary>
        ChangeRole,

        /// <summary>
        /// The service replica or instance is being closed.
        /// </summary>
        Close,

        /// <summary>
        /// The service replica or instance is being aborted.
        /// </summary>
        Abort,
    }
}
