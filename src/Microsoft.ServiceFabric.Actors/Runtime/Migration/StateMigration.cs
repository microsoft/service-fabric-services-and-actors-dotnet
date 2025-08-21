// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    /// <summary>
    /// Indicates actor service migration state.
    /// </summary>
    public enum StateMigration
    {
        /// <summary>
        /// Actor service is not migration ready.
        /// </summary>
        None = 0,

        /// <summary>
        /// Actor service is migration ready and configured as source service.
        /// </summary>
        Source = 1,

        /// <summary>
        /// Actor service is migration ready and configured as target service.
        /// </summary>
        Target = 2,
    }
}
