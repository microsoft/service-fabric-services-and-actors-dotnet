// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    sealed class NullMeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        public IMeter<TValueType> CreateMeter(string metricNamespace, string name)
        {
            return new NullMeter<TValueType>();
        }

        public IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1)
        {
            return new NullMeter1D<TValueType>();
        }

        public IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1, string dimension2)
        {
            return new NullMeter2D<TValueType>();
        }

        public IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1, string dimension2, string dimension3)
        {
            return new NullMeter3D<TValueType>();
        }

        public void Dispose() { }
    }
}
