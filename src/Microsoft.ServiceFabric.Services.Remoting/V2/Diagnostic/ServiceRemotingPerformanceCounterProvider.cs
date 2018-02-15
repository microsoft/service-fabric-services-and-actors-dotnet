// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;
    using System.Linq;
    using System.Text;

    internal class ServiceRemotingPerformanceCounterProvider : IDisposable
    {
        private readonly FabricPerformanceCounterSetInstance serviceCounterSetInstance;
        private readonly Guid partitionId;
        private readonly string counterInstanceDifferentiator;
        private static readonly string TraceType = "ServiceRemotingPerformanceCounterProvider";

        private static FabricPerformanceCounterSet ServiceCounterSet;

        internal FabricAverageCount64PerformanceCounterWriter serviceRequestProcessingTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter serviceRequestDeserializationTimeCounterWriter;
        internal FabricAverageCount64PerformanceCounterWriter serviceResponseSerializationTimeCounterWriter;
        internal FabricNumberOfItems64PerformanceCounterWriter serviceOutstandingRequestsCounterWriter;
        private static readonly int MaxDigits = 10;

        static ServiceRemotingPerformanceCounterProvider()
        {
            InitializeAvailableCounterTypes();
        }

        public ServiceRemotingPerformanceCounterProvider(Guid partitionId, long replicaOrInstanceId)

        {
            this.partitionId = partitionId;
            var ticks = (long)((DateTime.UtcNow.Ticks) % Math.Pow(10, MaxDigits));
            this.counterInstanceDifferentiator = string.Concat(replicaOrInstanceId,
                "_",
                ticks.ToString("D"));
            var serviceCounterInstanceName = string.Concat(this.partitionId.ToString("D"), "_",
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
        }

        private static void InitializeAvailableCounterTypes()
        {
            var servicePerformanceCounters = new ServiceRemotingPerformanceCounters();
            var counterSetDefinitions = servicePerformanceCounters.GetCounterSets();
            ServiceCounterSet = CreateCounterSet(counterSetDefinitions,
                ServiceRemotingPerformanceCounters.ServiceCategoryName);
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

            sb.Append(string.Format("Created performance counter category {0} with following counters.",
                counterSet.CounterSetDefinition.Name));
            sb.AppendLine();
            foreach (var counter in activeCounters)
            {
                sb.Append(string.Format("CounterName : {0}", counter.Name));
                sb.AppendLine();
            }
            ServiceTrace.Source.WriteInfo(TraceType, sb.ToString());
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





        public void Dispose()
        {
            ServiceTrace.Source.WriteInfo(TraceType, "Disposing Service Remoting Performance Counters");

            if (null != this.serviceCounterSetInstance)
            {
                //Remove Counter Instance.
                this.serviceCounterSetInstance.Dispose();
            }
        }
    }
}
