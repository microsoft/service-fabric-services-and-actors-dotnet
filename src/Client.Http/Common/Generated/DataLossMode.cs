// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for DataLossMode.
    /// </summary>
    public enum DataLossMode
    {
        /// <summary>
        /// Reserved.  Do not pass into API.
        /// </summary>
        Invalid,

        /// <summary>
        /// PartialDataLoss option will cause a quorum of replicas to go down, triggering an OnDataLoss event in the system for
        /// the given partition.
        /// </summary>
        PartialDataLoss,

        /// <summary>
        /// FullDataLoss option will drop all the replicas which means that all the data will be lost.
        /// </summary>
        FullDataLoss,
    }
}
