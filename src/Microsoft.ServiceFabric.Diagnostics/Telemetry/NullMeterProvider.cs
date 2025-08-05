// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class NullMeterProvider<ValueType> : IMeterProvider<ValueType>
    {
        public IMeter<ValueType> CreateMeter(string name, string metricNamespace = "")
        {
            return new NullMeter<ValueType>();
        }

        public IMeter1D<ValueType> CreateMeter(string name, string dimension1Name, string metricNamespace = "")
        {
            return new NullMeter1D<ValueType>();
        }

        public IMeter2D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string metricNamespace = "")
        {
            return new NullMeter2D<ValueType>();
        }

        public IMeter3D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string dimension3Name, string metricNamespace = "")
        {
            return new NullMeter3D<ValueType>();
        }
    }
}
