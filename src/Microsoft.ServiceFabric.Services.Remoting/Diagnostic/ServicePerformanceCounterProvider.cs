// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Diagnostic
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;

    internal class ServicePerformanceCounterProvider : IDisposable
    {
        private readonly ServiceTypeInformation serviceTypeInformation;
        private readonly FabricPerformanceCounterSetInstance serviceCounterSetInstance;
        private readonly Guid partitionId;
        private readonly string counterInstanceDifferentiator;
        private static readonly string TraceType = "ServicePerformanceCounterProvider";

        private static FabricPerformanceCounterSet ServiceCounterSet;
        private Dictionary<long, CounterInstanceData> serviceMethodCounterInstanceData;
        private static FabricPerformanceCounterSet ServiceMethodCounterSet;

        internal FabricAverageCount64PerformanceCounterWriter serviceRequestProcessingTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter serviceRequestDeserializationTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter serviceResponseSerializationTimeCounterWriter;
        internal FabricNumberOfItems64PerformanceCounterWriter serviceOutstandingRequestsCounterWriter;
        private static readonly int MaxDigits = 10;

        static ServicePerformanceCounterProvider()
        {
            InitializeAvailableCounterTypes();
        }

        public ServicePerformanceCounterProvider(Guid partitionId, long replicaOrInstanceId,
            ServiceTypeInformation serviceTypeInformation)
        {
            this.partitionId = partitionId;
            this.serviceTypeInformation = serviceTypeInformation;
            var ticks = (long) ((DateTime.UtcNow.Ticks)%Math.Pow(10, MaxDigits));
            this.counterInstanceDifferentiator = String.Concat(replicaOrInstanceId,
                "_",
                ticks.ToString("D"));
            var serviceCounterInstanceName = String.Concat(this.partitionId.ToString("D"), "_",
                this.counterInstanceDifferentiator);
            try
            {
                //Create ServiceCounterSetInstance
                this.serviceCounterSetInstance =
                    ServiceCounterSet.CreateCounterSetInstance(serviceCounterInstanceName);
            }
            catch (Exception ex)
            {
                //Instance creation failed, Be done.
                ServiceTrace.Source.WriteWarning(TraceType,
                    "Data for performance counter instance {0} of categoryName {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                    serviceCounterInstanceName, ServiceRemotingPerformanceCounters.ServiceCategoryName, ex);
                return;
            }
            this.CreateserviceCounterWriters(serviceCounterInstanceName);
            this.InitializeServiceMethodInfo();
        }


        internal void OnServiceMethodFinish(int interfaceId,
            int methodId,
            TimeSpan executionTime,
            Exception ex = null)
        {
            if (this.serviceMethodCounterInstanceData == null)
            {
                return;
            }
            var interfaceMethodKey = GetInterfaceMethodKey(interfaceId, methodId);

            var counterWriters = this.serviceMethodCounterInstanceData[interfaceMethodKey].CounterWriters;
            if (counterWriters == null)
            {
                return;
            }
            // Call the counter writers to update the counter values
            if (null != counterWriters.ServiceMethodFrequencyCounterWriter)
            {
                counterWriters.ServiceMethodFrequencyCounterWriter.UpdateCounterValue(1);
            }
            if (null != counterWriters.ServiceMethodExceptionFrequencyCounterWriter && ex != null)
            {
                counterWriters.ServiceMethodExceptionFrequencyCounterWriter.UpdateCounterValue(1);
            }
            if (null != counterWriters.ServiceMethodExecTimeCounterWriter)
            {
                counterWriters.ServiceMethodExecTimeCounterWriter.UpdateCounterValue(
                    (long) executionTime.TotalMilliseconds);
            }
        }


        private static void InitializeAvailableCounterTypes()
        {
            var servicePerformanceCounters = new ServiceRemotingPerformanceCounters();
            var counterSetDefinitions = servicePerformanceCounters.GetCounterSets();

            ServiceCounterSet = CreateCounterSet(counterSetDefinitions,
                ServiceRemotingPerformanceCounters.ServiceCategoryName);
            ServiceMethodCounterSet = CreateCounterSet(counterSetDefinitions,
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
                //Create CounterSet for this categoryName.
                counterSet = new FabricPerformanceCounterSet(counterSetDefinition.Key, counterSetDefinition.Value);
            }
            catch (Exception ex)
            {
                ServiceTrace.Source.WriteWarning(TraceType,
                    "Data for performance counter categoryName {0} will not be provided because an exception occurred during its initialization. Exception info: {1}",
                    counterSetDefinition.Key.Name, ex);
                return null;
            }
            DumpCounterSetInfo(counterSet, counterSetDefinition.Value);

            return counterSet;
        }


        private static void DumpCounterSetInfo(FabricPerformanceCounterSet counterSet,
            IEnumerable<FabricPerformanceCounterDefinition> activeCounters)
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("Created performance counter category {0} with following counters.",
                counterSet.CounterSetDefinition.Name));
            sb.AppendLine();
            foreach (var counter in activeCounters)
            {
                sb.Append(String.Format("CounterName : {0}", counter.Name));
                sb.AppendLine();
            }
            ServiceTrace.Source.WriteInfo(TraceType, sb.ToString());
        }

        private void InitializeServiceMethodInfo()
        {
            this.serviceMethodCounterInstanceData = new Dictionary<long, CounterInstanceData>();
            var methodInfoList = new List<KeyValuePair<long, MethodInfo>>();
            foreach (var serviceInterfaceType in this.serviceTypeInformation.InterfaceTypes)
            {
                var interfaceDescription = ServiceInterfaceDescription.Create(serviceInterfaceType
                    );
                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    var kvp = new KeyValuePair<long, MethodInfo>(
                        GetInterfaceMethodKey(interfaceDescription.Id, methodDescription.Id),
                        methodDescription.MethodInfo);
                    methodInfoList.Add(kvp);
                }
            }

            // Compute the counter instance names for all the actor methods
            var percCounterInstanceNameBuilder = new PerformanceCounterInstanceNameBuilder(this.partitionId,
                this.counterInstanceDifferentiator);
            var counterInstanceNames = percCounterInstanceNameBuilder.GetMethodCounterInstanceNames(methodInfoList);
            foreach (var kvp in counterInstanceNames)
            {
                this.serviceMethodCounterInstanceData[kvp.Key] = new CounterInstanceData {InstanceName = kvp.Value};
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
                    ServiceMethodCounterSet.CreateCounterSetInstance(instanceName);
            }
            catch (Exception ex)
            {
                //Instance creation failed, Be done.
                ServiceTrace.Source.WriteWarning(TraceType,
                    "Data for performance counter instance {0} of category {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                    instanceName, ServiceRemotingPerformanceCounters.ServiceMethodCategoryName, ex);
                return null;
            }

            tempCounterWriters.ServiceMethodFrequencyCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricNumberOfItems64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricNumberOfItems64PerformanceCounterWriter(inst,
                        ServiceRemotingPerformanceCounters.ServiceMethodInvocationsPerSecCounterName));
            tempCounterWriters.ServiceMethodExceptionFrequencyCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricNumberOfItems64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricNumberOfItems64PerformanceCounterWriter(inst,
                        ServiceRemotingPerformanceCounters.ServiceMethodExceptionsPerSecCounterName));
            tempCounterWriters.ServiceMethodExecTimeCounterWriter = this.CreateMethodCounterWriter(
                instanceName,
                typeof(FabricAverageCount64PerformanceCounterWriter),
                tempCounterWriters.ServiceMethodCounterSetInstance,
                inst =>
                    new FabricAverageCount64PerformanceCounterWriter(inst,
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
                    serviceRequestProcessingTimeCounterWriter = this.CreateCounterWriter(
                        serviceCounterInstanceName,
                        typeof(FabricAverageCount64PerformanceCounterWriter),
                        () =>
                            new
                                FabricAverageCount64PerformanceCounterWriter(
                                this.serviceCounterSetInstance,
                                ServiceRemotingPerformanceCounters.ServiceRequestProcessingTimeMillisecCounterName,
                                ServiceRemotingPerformanceCounters.ServiceRequestProcessingTimeMillisecBaseCounterName));

                this.serviceRequestDeserializationTimeCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () =>
                        new
                            FabricAverageCount64PerformanceCounterWriter(
                            this.serviceCounterSetInstance,
                            ServiceRemotingPerformanceCounters.ServiceRequestDeserializationTimeMillisecCounterName,
                            ServiceRemotingPerformanceCounters.ServiceRequestDeserializationTimeMillisecBaseCounterName));

                this.serviceResponseSerializationTimeCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () =>
                        new
                            FabricAverageCount64PerformanceCounterWriter(
                            this.serviceCounterSetInstance,
                            ServiceRemotingPerformanceCounters.ServiceResponseSerializationTimeMillisecCounterName,
                            ServiceRemotingPerformanceCounters.ServiceResponseSerializationTimeMillisecBaseCounterName));


                this.serviceOutstandingRequestsCounterWriter = this.CreateCounterWriter(
                    serviceCounterInstanceName,
                    typeof(FabricNumberOfItems64PerformanceCounterWriter),
                    () =>
                        new
                            FabricNumberOfItems64PerformanceCounterWriter(
                            this.serviceCounterSetInstance,
                            ServiceRemotingPerformanceCounters.ServiceOutstandingRequestsCounterName));
            }
        }

        private T CreateMethodCounterWriter<T>(string instanceName, Type counterWriterType,
            FabricPerformanceCounterSetInstance instance, Func<FabricPerformanceCounterSetInstance, T>
                counterWriterCreationCallback)
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

        private T CreateCounterWriter<T>(string serviceCounterInstanceName, Type writerType,
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

        private static long GetInterfaceMethodKey(int interfaceId, int methodId)
        {
            var key = (ulong) methodId;
            key = key | (((ulong) interfaceId) << 32);
            return (long) key;
        }

        private void LogCounterInstanceCreationResult(Type counterWriterType, string instanceName, Exception e)
        {
            if (null == e)
            {
                // Success
                ServiceTrace.Source.WriteInfo(TraceType,
                    "Performance counter writer {0} enabled for counter instance {1}.",
                    counterWriterType,
                    instanceName);
            }
            else
            {
                // Failure
                ServiceTrace.Source.WriteWarning(TraceType,
                    "Performance counter writer {0} for instance {1} has been disabled because an exception occurred during its initialization. Exception info: {2}"
                    ,
                    counterWriterType,
                    instanceName,
                    e);
            }
        }

        private class CounterInstanceData
        {
            internal MethodSpecificCounterWriters CounterWriters;
            internal string InstanceName;
        }

        private class MethodSpecificCounterWriters
        {
            internal FabricPerformanceCounterSetInstance ServiceMethodCounterSetInstance;
            internal FabricNumberOfItems64PerformanceCounterWriter ServiceMethodFrequencyCounterWriter;
            internal FabricNumberOfItems64PerformanceCounterWriter ServiceMethodExceptionFrequencyCounterWriter;
            internal FabricAverageCount64PerformanceCounterWriter ServiceMethodExecTimeCounterWriter;
        }

        public void Dispose()
        {
            ServiceTrace.Source.WriteInfo(TraceType, "Disposing Service Performance Counters");

            if (null != this.serviceCounterSetInstance)
            {
                //Remove Counter Instance.
                this.serviceCounterSetInstance.Dispose();
            }
            if (null != this.serviceMethodCounterInstanceData)
            {
                foreach (var counterInstanceData in this.serviceMethodCounterInstanceData.Values)
                {
                    if (null != counterInstanceData.CounterWriters)
                    {
                        if (null != counterInstanceData.CounterWriters.ServiceMethodCounterSetInstance)
                        {
                            //Remove Counter Instance.
                            counterInstanceData.CounterWriters.ServiceMethodCounterSetInstance.Dispose();
                        }
                    }
                }
            }
        }
    }
}