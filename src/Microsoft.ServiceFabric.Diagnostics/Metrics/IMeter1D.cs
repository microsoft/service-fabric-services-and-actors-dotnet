// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Defines a one-dimensional meter for recording telemetry values with a single dimension.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value to be recorded.</typeparam>
    interface IMeter1D<TValueType>
    {
        /// <summary>
        /// Records a telemetry value with one dimension.
        /// </summary>
        /// <param name="value">The value to record.</param>
        /// <param name="dimension1">The first dimension value.</param>
        void Record(TValueType value, string dimension1);
    }
}
