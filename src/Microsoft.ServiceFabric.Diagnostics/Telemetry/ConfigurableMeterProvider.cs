// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal abstract class ConfigurableMeterProvider<ValueType> : IMeterProvider<ValueType>
    {

        readonly protected IDictionary<string, string> systemDimensions = new ConcurrentDictionary<string, string> ();
        readonly protected bool metricsEnabled;

        protected ConfigurableMeterProvider(ConfigStoreWrapper configStoreWrapped, ServiceContext serviceContext = null)
        {
            if (configStoreWrapped == null)
            {
                throw new System.ArgumentNullException(nameof(configStoreWrapped), "Config store cannot be null.");
            }

            this.systemDimensions.Add("NodeName", configStoreWrapped.ReadConfig("FabricNode", "InstanceName") ?? string.Empty);
            this.systemDimensions.Add("RuntimeVersion", configStoreWrapped.ReadConfig("FabricNode", "NodeVersion")?.Split(':')[0] ?? string.Empty);
            this.metricsEnabled = (configStoreWrapped.ReadConfig("Telemetry/Metrics", "IsEnabled")??"false").ToLowerInvariant() == "true";

            if (serviceContext != null)
            {
                this.systemDimensions.Add("ReplicaOrInstanceId", serviceContext.ReplicaOrInstanceId.ToString());
                this.systemDimensions.Add("PartitionId", serviceContext.PartitionId.ToString());
                this.systemDimensions.Add("ServiceTypeName", serviceContext.ServiceTypeName);
                this.systemDimensions.Add("ServiceName", serviceContext.ServiceName.ToString());
                this.systemDimensions.Add("ApplicationName", serviceContext.CodePackageActivationContext.ApplicationName);
                this.systemDimensions.Add("ApplicationTypeName", serviceContext.CodePackageActivationContext.ApplicationTypeName);
            }
        }

        public abstract IMeter<ValueType> CreateMeter(string name, string metricNamespace = "");
        public abstract IMeter1D<ValueType> CreateMeter(string name, string dimension1Name, string metricNamespace = "");
        public abstract IMeter2D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string metricNamespace = "");
        public abstract IMeter3D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string dimension3Name, string metricNamespace = "");
    }
}
