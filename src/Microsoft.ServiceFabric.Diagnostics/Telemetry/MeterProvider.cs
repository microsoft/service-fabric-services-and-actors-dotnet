// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class MeterProvider
    {
        readonly bool metricsConfigEnabled;
        readonly string nodeName;
        readonly string runtimeVersion;

        internal MeterProvider(IConfigStore2 configStore)
        {
            nodeName = configStore.ReadUnencryptedString("FabricNode", "InstanceName");
            // runtimeVersion is of the form "10.x.x:0:0" before formatting -- we only want the 10.x.x part
            runtimeVersion = configStore.ReadUnencryptedString("FabricNode", "NodeVersion").Split(':')[0];
            metricsConfigEnabled = configStore.ReadUnencryptedString("Telemetry/Metrics", "IsEnabled").ToLowerInvariant() == "true";
        }

        public IMeter0D<ValueType> CreateMeter<ValueType>(string name)
        {
            if (metricsConfigEnabled){
                return new Meter<ValueType>(name, nodeName, runtimeVersion);
            }
            return new NullMeter<ValueType>();
        }

        public IMeter1D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name)
        {
            if (metricsConfigEnabled){
                return new Meter1D<ValueType>(name, dimension1Name, nodeName, runtimeVersion);
            }
            return new NullMeter1D<ValueType>();
        }

        public IMeter2D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name)
        {
            if (metricsConfigEnabled){
                return new Meter2D<ValueType>(name, dimension1Name, dimension2Name, nodeName, runtimeVersion);
            }
            return new NullMeter2D<ValueType>();
        }

        public IMeter3D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name, string dimension3Name)
        {
            if (metricsConfigEnabled){
                return new Meter3D<ValueType>(name, dimension1Name, dimension2Name, dimension3Name, nodeName, runtimeVersion);
            }
            return new NullMeter3D<ValueType>();
        }
    }
}
