// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for RestartPolicy.
    /// </summary>
    public enum RestartPolicy
    {
        /// <summary>
        /// Service will be restarted when it encounters a failure.
        /// </summary>
        OnFailure,

        /// <summary>
        /// Service will never be restarted. If the service encounters a failure, it will move to Failed state.
        /// </summary>
        Never,
    }
}
