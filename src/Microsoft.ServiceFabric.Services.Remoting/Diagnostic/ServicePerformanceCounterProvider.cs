// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Diagnostic
{
    extern alias Microsoft_ServiceFabric_Internal;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using FabricAverageCount64PerformanceCounterWriter = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricAverageCount64PerformanceCounterWriter;
    using FabricNumberOfItems64PerformanceCounterWriter = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricNumberOfItems64PerformanceCounterWriter;
    using FabricPerformanceCounterDefinition = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterDefinition;
    using FabricPerformanceCounterSet = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSet;
    using FabricPerformanceCounterSetDefinition = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSetDefinition;
    using FabricPerformanceCounterSetInstance = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSetInstance;

    internal class ServicePerformanceCounterProvider : IDisposable
    {
#pragma warning disable SA1401 // Fields should be private

        internal FabricAverageCount64PerformanceCounterWriter ServiceRequestProcessingTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter ServiceRequestDeserializationTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter ServiceResponseSerializationTimeCounterWriter;
        internal FabricNumberOfItems64PerformanceCounterWriter ServiceOutstandingRequestsCounterWriter;

#pragma warning restore SA1401 // Fields should be private

        private static readonly string TraceType = "ServicePerformanceCounterProvider";
        private static readonly int MaxDigits = 10;

        private static FabricPerformanceCounterSet serviceCounterSet;
        private static FabricPerformanceCounterSet serviceMethodCounterSet;

        private readonly bool generateCounterforServiceCategory;
        private readonly FabricPerformanceCounterSetInstance serviceCounterSetInstance;
        private readonly Guid partitionId;
        private readonly string counterInstanceDifferentiator;

        private Dictionary<long, CounterInstanceData> serviceMethodCounterInstanceData;
        private IEnumerable<ServiceInterfaceDescription> serviceInterfaceDescriptions;

        static ServicePerformanceCounterProvider()
        {
            InitializeAvailableCounterTypes();
        }

        public ServicePerformanceCounterProvider(
            Guid partitionId,
            long replicaOrInstanceId,
            List<ServiceInterfaceDescription> interfaceDescriptions,
            bool generateCounterforServiceCategory = true)
        {
            this.partitionId = partitionId;
            this.serviceInterfaceDescriptions = interfaceDescriptions;
            this.generateCounterforServiceCategory = generateCounterforServiceCategory;
            var ticks = (long)((DateTime.UtcNow.Ticks) % Math.Pow(10, MaxDigits));
            this.counterInstanceDifferentiator = string.Concat(
                replicaOrInstanceId,
                "_",
                ticks.ToString("D"));
            var serviceCounterInstanceName = string.Concat(
                this.partitionId.ToString("D"),
                "_",
                this.counterInstanceDifferentiator);
            try
            {
                // Create ServiceCounterSetInstance
                if (generateCounterforServiceCategory)
                {
                    this.serviceCounterSetInstance =
                        serviceCounterSet.CreateCounterSetInstance(serviceCounterInstanceName);
                    this.CreateserviceCounterWriters(serviceCounterInstanceName);
                }
            }
            catch (Exception ex)
            {
                // Instance creation failed, Be done.
                ServiceTrace.Source.WriteWarning(
                    TraceType,
                    "Data for performance counter instance {0} of categoryName {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                    serviceCounterInstanceName,
                    ServiceRemotingPerformanceCounters.ServiceCategoryName,
                    ex);
                return;
            }

            this.InitializeServiceMethodInfo();
        }

        public void Dispose()
        {
            ServiceTrace.Source.WriteInfo(TraceType, "Disposing Service Remoting Performance Counters");

            if (this.serviceCounterSetInstance != null)
            {
                // Remove Counter Instance.
                this.serviceCounterSetInstance.Dispose();
            }

            if (this.serviceMethodCounterInstanceData != null)
            {
                foreach (var counterInstanceData in this.serviceMethodCounterInstanceData.Values)
                {
                    if (counterInstanceData.CounterWriters != null)
                    {
                        if (counterInstanceData.CounterWriters.ServiceMethodCounterSetInstance != null)
                        {
                            // Remove Counter Instance.
                            counterInstanceData.CounterWriters.ServiceMethodCounterSetInstance.Dispose();
                        }
                    }
                }
            }
        }

        internal void OnServiceMethodFinish(
            int interfaceId,
            int methodId,
            TimeSpan executionTime,
            Exception ex = null)
        {
            if (this.serviceMethodCounterInstanceData == null)
            {
                return;
            }

            var interfaceMethodKey = GetInterfaceMethodKey(interfaceId, methodId);

            if (!this.serviceMethodCounterInstanceData.TryGetValue(interfaceMethodKey, out var outVal))
            {
                return;
            }

            var counterWriters = outVal.CounterWriters;
            if (counterWriters == null)
            {
                return;
            }

            // Call the counter writers to update the counter values
            if (counterWriters.ServiceMethodFrequencyCounterWriter != null)
            {
                counterWriters.ServiceMethodFrequencyCounterWriter.UpdateCounterValue(1);
            }

            if (counterWriters.ServiceMethodExceptionFrequencyCounterWriter != null && ex != null)
            {
                counterWriters.ServiceMethodExceptionFrequencyCounterWriter.UpdateCounterValue(1);
            }

            if (counterWriters.ServiceMethodExecTimeCounterWriter != null)
            {
                counterWriters.ServiceMethodExecTimeCounterWriter.UpdateCounterValue(
                    (long)executionTime.TotalMilliseconds);
            }
        }

        private static void InitializeAvailableCounterTypes()
        {
            var servicePerformanceCounters = new ServiceRemotingPerformanceCounters();
            var counterSetDefinitions = servicePerformanceCounters.GetCounterSets();

            serviceCounterSet = CreateCounterSet(
                counterSetDefinitions,
                ServiceRemotingPerformanceCounters.ServiceCategoryName);
            serviceMethodCounterSet = CreateCounterSet(
                counterSetDefinitions,
                ServiceRemotingPerformanceCounters.ServiceMethodCategoryName);
        }

        private static FabricPerformanceCounterSet CreateCounterSet(
            Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>> counterSetDefinitions,
            string categoryName)
        {
            var counterSetDefinition = counterSetDefinitions.Single(kvp => (kvp.Key.Name == categoryName));
            FabricPerformanceCounterSet counterSet = null;
            try
            {
                // Create CounterSet for this categoryName.
                counterSet = new FabricPerformanceCounterSet(counterSetDefinition.Key, counterSetDefinition.Value);
            }
            catch (Exception ex)
            {
                ServiceTrace.Source.WriteWarning(
                    TraceType,
                    "Data for performance counter categoryName {0} will not be provided because an exception occurred during its initialization. Exception info: {1}",
                    counterSetDefinition.Key.Name,
                    ex);
                return null;
            }

            DumpCounterSetInfo(counterSet, counterSetDefinition.Value);

            return counterSet;
        }

        private static void DumpCounterSetInfo(
            FabricPerformanceCounterSet counterSet,
            IEnumerable<FabricPerformanceCounterDefinition> activeCounters)
        {
            var sb = new StringBuilder();

            sb.Append(string.Format(
                "Created performance counter category {0} with following counters.",
                counterSet.CounterSetDefinition.Name));
            sb.AppendLine();
            foreach (var counter in activeCounters)
            {
                sb.Append(string.Format("CounterName : {0}", counter.Name));
                sb.AppendLine();
            }

            ServiceTrace.Source.WriteInfo(TraceType, sb.ToString());
        }

        private static long GetInterfaceMethodKey(int interfaceId, int methodId)
        {
            var key = (ulong)methodId;
            key = key | (((ulong)interfaceId) << 32);
            return (long)key;
        }

        private void InitializeServiceMethodInfo()
        {
            this.serviceMethodCounterInstanceData = new Dictionary<long, CounterInstanceData>();
            var methodInfoList = new List<KeyValuePair<long, MethodInfo>>();
            foreach (var interfaceDescription in this.serviceInterfaceDescriptions)
            {
                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    var kvp = new KeyValuePair<long, MethodInfo>(
                        GetInterfaceMethodKey(interfaceDescription.Id, methodDescription.Id),
                        methodDescription.MethodInfo);
                    methodInfoList.Add(kvp);
                }
            }

            // Compute the counter instance names for all the actor methods
            var percCounterInstanceNameBuilder = new PerformanceCounterInstanceNameBuilder(
                this.partitionId,
                this.counterInstanceDifferentiator);
            var counterInstanceNames = percCounterInstanceNameBuilder.GetMethodCounterInstanceNames(methodInfoList);
            foreach (var kvp in counterInstanceNames)
            {
                this.serviceMethodCounterInstanceData[kvp.Key] = new CounterInstanceData { InstanceName = kvp.Value };
                this.serviceMethodCounterInstanceData[kvp.Key].CounterWriters =
                    this.InitializeMethodCounterInstanceData(kvp.Value);
            }
        }

        private MethodSpecificCounterWriters InitializeMethodCounterInstanceData(string instanceName)
        {
            var tempCounterWriters = new MethodSpecificCounterWriters();

            try
            {
                tempCounterWriters.ServiceMethodCounterSetInstance =
                    serviceMethodCounterSet.CreateCounterSetInstance(instanceName);
            }
            catch (Exception ex)
            {
                // Instance creation failed, Be done.
                ServiceTrace.Source.WriteWarning(
                    TraceType,
                    "Data for performance counter instance {0} of category {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                    instanceName,
                    ServiceRemotingPerformanceCounters.ServiceMethodCategoryName,
                    ex);
                return null;
            }

            tempCounterWriters.ServiceMethodFrequencyCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricNumberOfItems64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricNumberOfItems64PerformanceCounterWriter(
                        inst,
                        ServiceRemotingPerformanceCounters.ServiceMethodInvocationsPerSecCounterName));
            tempCounterWriters.ServiceMethodExceptionFrequencyCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricNumberOfItems64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricNumberOfItems64PerformanceCounterWriter(
                        inst,
                        ServiceRemotingPerformanceCounters.ServiceMethodExceptionsPerSecCounterName));
            tempCounterWriters.ServiceMethodExecTimeCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricAverageCount64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricAverageCount64PerformanceCounterWriter(
                        inst,
                        ServiceRemotingPerformanceCounters.ServiceMethodExecTimeMillisecCounterName,
                        ServiceRemotingPerformanceCounters.ServiceMethodExecTimeMillisecBaseCounterName));
            return tempCounterWriters;
        }

        private void CreateserviceCounterWriters(string serviceCounterInstanceName)
        {
            if (this.
                    serviceCounterSetInstance != null)
            {
                this.
                    ServiceRequestProcessingTimeCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () =>
                        new
                            FabricAverageCount64PerformanceCounterWriter(
                                this.serviceCounterSetInstance,
                                ServiceRemotingPerformanceCounters.ServiceRequestProcessingTimeMillisecCounterName,
                                ServiceRemotingPerformanceCounters.ServiceRequestProcessingTimeMillisecBaseCounterName));

                this.ServiceRequestDeserializationTimeCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () =>
                        new
                            FabricAverageCount64PerformanceCounterWriter(
                                this.serviceCounterSetInstance,
                                ServiceRemotingPerformanceCounters.ServiceRequestDeserializationTimeMillisecCounterName,
                                ServiceRemotingPerformanceCounters.ServiceRequestDeserializationTimeMillisecBaseCounterName));

                this.ServiceResponseSerializationTimeCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () =>
                        new
                            FabricAverageCount64PerformanceCounterWriter(
                                this.serviceCounterSetInstance,
                                ServiceRemotingPerformanceCounters.ServiceResponseSerializationTimeMillisecCounterName,
                                ServiceRemotingPerformanceCounters.ServiceResponseSerializationTimeMillisecBaseCounterName));

                this.ServiceOutstandingRequestsCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricNumberOfItems64PerformanceCounterWriter),
                    () =>
                        new
                            FabricNumberOfItems64PerformanceCounterWriter(
                                this.serviceCounterSetInstance,
                                ServiceRemotingPerformanceCounters.ServiceOutstandingRequestsCounterName));
            }
        }

        private T CreateMethodCounterWriter<T>(
            string instanceName,
            Type counterWriterType,
            FabricPerformanceCounterSetInstance instance,
            Func<FabricPerformanceCounterSetInstance, T> counterWriterCreationCallback)
        {
            var retVal = default(T);
            try
            {
                retVal = counterWriterCreationCallback(instance);
            }
            catch (Exception ex)
            {
                this.LogCounterInstanceCreationResult(
                    counterWriterType,
                    instanceName,
                    ex);
            }

            return retVal;
        }

        private T CreateCounterWriter<T>(
            string serviceCounterInstanceName,
            Type writerType,
            Func<T> writerCreationCallback)
        {
            var result = default(T);
            Exception ex = null;
            try
            {
                result = writerCreationCallback();
            }
            catch (Exception e)
            {
                ex = e;
            }

            this.LogCounterInstanceCreationResult(writerType, serviceCounterInstanceName, ex);
            return result;
        }

        private void LogCounterInstanceCreationResult(Type counterWriterType, string instanceName, Exception e)
        {
            if (e == null)
            {
                // Success
                ServiceTrace.Source.WriteInfo(
                    TraceType,
                    "Performance counter writer {0} enabled for counter instance {1}.",
                    counterWriterType,
                    instanceName);
            }
            else
            {
                // Failure
                ServiceTrace.Source.WriteWarning(
                    TraceType,
                    "Performance counter writer {0} for instance {1} has been disabled because an exception occurred during its initialization. Exception info: {2}",
                    counterWriterType,
                    instanceName,
                    e);
            }
        }

        private class CounterInstanceData
        {
#pragma warning disable SA1401 // Fields should be private
            internal MethodSpecificCounterWriters CounterWriters;
            internal string InstanceName;
#pragma warning restore SA1401 // Fields should be private
        }

        private class MethodSpecificCounterWriters
        {
#pragma warning disable SA1401 // Fields should be private
            internal FabricPerformanceCounterSetInstance ServiceMethodCounterSetInstance;
            internal FabricNumberOfItems64PerformanceCounterWriter ServiceMethodFrequencyCounterWriter;
            internal FabricNumberOfItems64PerformanceCounterWriter ServiceMethodExceptionFrequencyCounterWriter;
            internal FabricAverageCount64PerformanceCounterWriter ServiceMethodExecTimeCounterWriter;
#pragma warning restore SA1401 // Fields should be private
        }
    }
}
