// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Defines a three-dimensional meter for recording telemetry values with three dimensions.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value to be recorded.</typeparam>
    interface IMeter3D<TValueType> : IDisposable
    {
        /// <summary>
        /// Records a telemetry value with three dimensions.
        /// </summary>
        /// <param name="value">The value to record.</param>
        /// <param name="dimension1">The first dimension value.</param>
        /// <param name="dimension2">The second dimension value.</param>
        /// <param name="dimension3">The third dimension value.</param>
        void Record(TValueType value, string dimension1, string dimension2, string dimension3);
    }
}
