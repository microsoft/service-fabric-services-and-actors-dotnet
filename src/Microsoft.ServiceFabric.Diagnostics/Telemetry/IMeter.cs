// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal interface IMeter0D<ValueType>
    {
        void Record(ValueType value);
    }

    internal interface IMeter1D<ValueType>
    {
        void Record(ValueType value, string dimension1);
    }

    internal interface IMeter2D<ValueType>
    {
        void Record(ValueType value, string dimension1, string dimension2);
    }

    internal interface IMeter3D<ValueType>
    {
        void Record(ValueType value, string dimension1, string dimension2, string dimension3);
    }
}
