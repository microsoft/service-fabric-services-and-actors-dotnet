// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal interface IMeterProvider<ValueType>
    {
        IMeter<ValueType> CreateMeter(string name, string metricNamespace = "");

        IMeter1D<ValueType> CreateMeter(string name, string dimension1Name, string metricNamespace = "");

        IMeter2D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string metricNamespace = "");

        IMeter3D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string dimension3Name, string metricNamespace = "");
    }
}
