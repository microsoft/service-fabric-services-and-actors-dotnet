// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class Int64MeterProvider : ServiceMeterProvider<long>
    {
        public Int64MeterProvider(ServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        public override IMeter<long> CreateMeter(string metricNamespace, string name)
        {
            return new Int64Meter(CreateNativeMeter(metricNamespace, name, new List<string>()), systemDimensionValues);
        }

        public override IMeter1D<long> CreateMeter(string metricNamespace, string name, string dimension1Name)
        {
            return new Int64Meter1D(CreateNativeMeter(metricNamespace, name, new List<string>() { dimension1Name }), systemDimensionValues);
        }

        public override IMeter2D<long> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name)
        {
            return new Int64Meter2D(CreateNativeMeter(metricNamespace, name, new List<string>() { dimension1Name, dimension2Name }), systemDimensionValues);
        }

        public override IMeter3D<long> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name)
        {
            return new Int64Meter3D(CreateNativeMeter(metricNamespace, name, new List<string>() { dimension1Name, dimension2Name, dimension3Name }), systemDimensionValues);
        }
    }
}
