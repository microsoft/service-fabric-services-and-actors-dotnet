// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for MoveCost.
    /// </summary>
    public enum MoveCost
    {
        /// <summary>
        /// Zero move cost. This value is zero.
        /// </summary>
        Zero,

        /// <summary>
        /// Specifies the move cost of the service as Low. The value is 1.
        /// </summary>
        Low,

        /// <summary>
        /// Specifies the move cost of the service as Medium. The value is 2.
        /// </summary>
        Medium,

        /// <summary>
        /// Specifies the move cost of the service as High. The value is 3.
        /// </summary>
        High,

        /// <summary>
        /// Specifies the move cost of the service as VeryHigh. The value is 4.
        /// </summary>
        VeryHigh,
    }
}
