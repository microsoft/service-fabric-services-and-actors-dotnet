// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal interface IMeterProvider<ValueType>
    {
        IMeter<ValueType> CreateMeter(string metricNamespace, string name);

        IMeter1D<ValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);

        IMeter2D<ValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);

        IMeter3D<ValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
