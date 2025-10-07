// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Provides factory methods for creating telemetry meters with different number of dimensions.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value to be recorded. Currently intended to support integer and timespan meters.</typeparam>
    interface IMeterProvider<TValueType>
    {
        /// <summary>
        /// Creates a meter for recording telemetry values without dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <returns>A meter instance for recording values without dimensions.</returns>
        IMeter<TValueType> CreateMeter(string metricNamespace, string name);

        /// <summary>
        /// Creates a one-dimensional meter for recording telemetry values with a single dimension.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <returns>A meter instance for recording values with one dimension.</returns>
        IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);

        /// <summary>
        /// Creates a two-dimensional meter for recording telemetry values with two dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <param name="dimension2Name">The name of the second dimension.</param>
        /// <returns>A meter instance for recording values with two dimensions.</returns>
        IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);

        /// <summary>
        /// Creates a three-dimensional meter for recording telemetry values with three dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <param name="dimension2Name">The name of the second dimension.</param>
        /// <param name="dimension3Name">The name of the third dimension.</param>
        /// <returns>A meter instance for recording values with three dimensions.</returns>
        IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
