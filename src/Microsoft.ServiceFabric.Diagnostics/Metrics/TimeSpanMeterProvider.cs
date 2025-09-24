// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Implementation of <see cref="ServiceMeterProvider{TValueType}"/> for timespan metrics.
    /// Creates meters for recording timespan telemetry values with various dimension configurations.
    /// </summary>
    sealed class TimeSpanMeterProvider : ServiceMeterProvider<TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanMeterProvider"/> class.
        /// </summary>
        /// <param name="serviceContext">The Service Fabric ServiceContext provides system dimensions used by IMeter implementations to emit default metrics.</param>
        internal TimeSpanMeterProvider(ServiceContext serviceContext) : base(serviceContext) { }

        /// <summary>
        /// Creates a meter for recording timespan telemetry values without additional dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <returns>A meter instance for recording timespan values without additional dimensions.</returns>
        public override IMeter<TimeSpan> CreateMeter(string metricNamespace, string name)
        {
            return new TimeSpanMeter(CreateNativeMeter(metricNamespace, name, Array.Empty<string>()), systemDimensionValues);
        }

        /// <summary>
        /// Creates a one-dimensional meter for recording timespan telemetry values with a single dimension.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <returns>A meter instance for recording timespan values with one additional dimension.</returns>
        public override IMeter1D<TimeSpan> CreateMeter(string metricNamespace, string name, string dimension1Name)
        {
            return new TimeSpanMeter1D(CreateNativeMeter(metricNamespace, name, new[] { dimension1Name }), systemDimensionValues);
        }

        /// <summary>
        /// Creates a two-dimensional meter for recording timespan telemetry values with two dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <param name="dimension2Name">The name of the second dimension.</param>
        /// <returns>A meter instance for recording timespan values with two additional dimensions.</returns>
        public override IMeter2D<TimeSpan> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name)
        {
            return new TimeSpanMeter2D(CreateNativeMeter(metricNamespace, name, new[] { dimension1Name, dimension2Name }), systemDimensionValues);
        }

        /// <summary>
        /// Creates a three-dimensional meter for recording timespan telemetry values with three dimensions.
        /// </summary>
        /// <param name="metricNamespace">The namespace for the metric.</param>
        /// <param name="name">The name of the metric.</param>
        /// <param name="dimension1Name">The name of the first dimension.</param>
        /// <param name="dimension2Name">The name of the second dimension.</param>
        /// <param name="dimension3Name">The name of the third dimension.</param>
        /// <returns>A meter instance for recording timespan values with three additional dimensions.</returns>
        public override IMeter3D<TimeSpan> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name)
        {
            return new TimeSpanMeter3D(CreateNativeMeter(metricNamespace, name, new[] { dimension1Name, dimension2Name, dimension3Name }), systemDimensionValues);
        }
    }
}
