// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class NullMeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        public IMeter<TValueType> CreateMeter(string metricNamespace, string name)
        {
            return new NullMeter<TValueType>();
        }

        public IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name)
        {
            return new NullMeter1D<TValueType>();
        }

        public IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name)
        {
            return new NullMeter2D<TValueType>();
        }

        public IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name)
        {
            return new NullMeter3D<TValueType>();
        }
    }
}
