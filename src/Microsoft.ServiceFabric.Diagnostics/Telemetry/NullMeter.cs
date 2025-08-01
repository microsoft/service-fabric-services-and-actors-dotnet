// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class NullMeter<ValueType> : IMeter0D<ValueType>
    {
        public NullMeter() { }

        public void Record(ValueType value)
        {
            
        }
    }

    internal class NullMeter1D<ValueType> : IMeter1D<ValueType>
    {
        public NullMeter1D() { }

        public void Record(ValueType value, string dimension1)
        {
            
        }
    }

    internal class NullMeter2D<ValueType> : IMeter2D<ValueType>
    {
        public NullMeter2D() { }

        public void Record(ValueType value, string dimension1, string dimension2)
        {
            
        }
    }

    internal class NullMeter3D<ValueType> : IMeter3D<ValueType>
    {
        public NullMeter3D() { }

        public void Record(ValueType value, string dimension1, string dimension2, string dimension3)
        {
            
        }
    }
}
