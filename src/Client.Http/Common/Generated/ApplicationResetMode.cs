// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationResetMode.
    /// </summary>
    public enum ApplicationResetMode
    {
        /// <summary>
        /// Closes the existing replicas through the normal graceful shutdown sequence before recreating them. This is the
        /// default behavior.
        /// </summary>
        Graceful,

        /// <summary>
        /// Recreates the replicas immediately, without waiting for the graceful shutdown sequence. Use this when a replica is
        /// unable to close gracefully.
        /// </summary>
        Immediate,
    }
}
