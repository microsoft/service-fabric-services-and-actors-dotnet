// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class NullMeter<TValueType> : IMeter<TValueType>
    {
        public void Record(TValueType value)
        {

        }
    }

    internal class NullMeter1D<TValueType> : IMeter1D<TValueType>
    {
        public void Record(TValueType value, string dimension1)
        {

        }
    }

    internal class NullMeter2D<TValueType> : IMeter2D<TValueType>
    {
        public void Record(TValueType value, string dimension1, string dimension2)
        {

        }
    }

    internal class NullMeter3D<TValueType> : IMeter3D<TValueType>
    {
        public void Record(TValueType value, string dimension1, string dimension2, string dimension3)
        {

        }
    }
}
