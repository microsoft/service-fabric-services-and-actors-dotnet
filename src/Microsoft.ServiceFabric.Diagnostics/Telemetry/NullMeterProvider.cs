// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class NullMeterProvider
    {
        public IMeter<ValueType> CreateMeter<ValueType>(string name)
        {
            return new NullMeter<ValueType>();
        }

        public IMeter1D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name)
        {
            return new NullMeter1D<ValueType>();
        }

        public IMeter2D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name)
        {
            return new NullMeter2D<ValueType>();
        }

        public IMeter3D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name, string dimension3Name)
        {
            return new NullMeter3D<ValueType>();
        }
    }
}
