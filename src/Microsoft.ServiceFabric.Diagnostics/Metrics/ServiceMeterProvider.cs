// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal abstract class ServiceMeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly protected IDictionary<string, string> systemDimensions = new ConcurrentDictionary<string, string>();
        readonly protected bool metricsEnabled;

        protected ServiceMeterProvider(ServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext), "Service context cannot be null.");
            }

            this.systemDimensions.Add("ReplicaOrInstanceId", serviceContext.ReplicaOrInstanceId.ToString());
            this.systemDimensions.Add("PartitionId", serviceContext.PartitionId.ToString());
            this.systemDimensions.Add("ServiceTypeName", serviceContext.ServiceTypeName);
            this.systemDimensions.Add("ServiceName", serviceContext.ServiceName.ToString());
            this.systemDimensions.Add("ApplicationName", serviceContext.CodePackageActivationContext.ApplicationName);
            this.systemDimensions.Add("ApplicationTypeName", serviceContext.CodePackageActivationContext.ApplicationTypeName);
        }

        public abstract IMeter<TValueType> CreateMeter(string metricNamespace, string name);
        public abstract IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);
        public abstract IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);
        public abstract IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
