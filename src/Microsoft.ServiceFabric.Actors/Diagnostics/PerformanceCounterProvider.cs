// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;
    using System.Reflection;
    using System.Text;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using Microsoft.ServiceFabric.Services.Remoting.Diagnostic;

    internal class PerformanceCounterProvider : IDisposable
    {
        private const string TraceType = "PerformanceCounterProvider";

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static FabricPerformanceCounterSet ActorCounterSet;        
        private readonly FabricPerformanceCounterSetInstance actorCounterSetInstance;
        private ActorLockContentionCounterWriter actorLockContentionCounterWriter;
        private ActorSaveStateTimeCounterWriter actorSaveStateTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorRequestProcessingTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorLockAcquireWaitTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorLockHoldTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorRequestDeserializationTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorResponseSerializationTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorOnActivateAsyncTimeCounterWriter;
        private FabricAverageCount64PerformanceCounterWriter actorLoadStateTimeCounterWriter;
        private FabricNumberOfItems64PerformanceCounterWriter actorOutstandingRequestsCounterWriter;

        private readonly Guid partitionId;                
        private static FabricPerformanceCounterSet ActorMethodCounterSet;
        private Dictionary<long, CounterInstanceData> actorMethodCounterInstanceData;
        private readonly string counterInstanceDifferentiator;
        private readonly ActorTypeInformation actorTypeInformation;

        private static Dictionary<string, FabricPerformanceCounterSet> AvaiableFabricCounterSet;

        internal PerformanceCounterProvider(Guid partitionId, ActorTypeInformation actorTypeInformation)
        {
            // The counter instance names end with "_<TickCount>", where <TickCount> is the tick count when
            // the current object is created. 
            //
            // If we didn't include the <TickCount> portion, the following problem could arise. Just after
            // a reconfiguration, a new primary creates a performance counter instance with the same name
            // as the one created by the old primary. If the old primary has not yet finished cleaning up
            // the old counter instance before the new primary creates its counter instance, then both
            // the old and the new primary will be referencing the same counter instance. Eventually, the
            // old primary will clean up the counter instance, while the new primary could still be using
            // it. By appending the <TickCount> portion, we ensure that the old and new primaries do not 
            // reference the same counter instance. Therefore, when the old primary cleans up its counter
            // instance, the new primary is not affected by it.
            this.counterInstanceDifferentiator = DateTime.UtcNow.Ticks.ToString("D");

            this.partitionId = partitionId;
            this.actorTypeInformation = actorTypeInformation;

            // Creates counter writers for partition-wide counters.
            var actorCounterInstanceName = String.Concat(this.partitionId.ToString("D"), "_", this.counterInstanceDifferentiator);

            if (AvaiableFabricCounterSet.TryGetValue(ActorPerformanceCounters.ActorCategoryName, out ActorCounterSet))
            {
                try
                {
                    //Creates ActorCounterSetInstance
                    this.actorCounterSetInstance = ActorCounterSet.CreateCounterSetInstance(actorCounterInstanceName);
                }
                catch (Exception ex)
                {
                    //Displays a warning when the creation of this instance failed.
                    ActorTrace.Source.WriteWarning(
                        TraceType,
                        "Data for performance counter instance {0} of category {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                        actorCounterInstanceName, ActorPerformanceCounters.ActorCategoryName, ex);
                    return;
                }
            }

            this.CreateActorCounterWriters(actorCounterInstanceName);
        }

        private void CreateActorCounterWriters(string actorCounterInstanceName)
        {
            if (this.actorCounterSetInstance != null)
            {
                this.actorLockContentionCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(ActorLockContentionCounterWriter),
                    () => new ActorLockContentionCounterWriter(this.actorCounterSetInstance));

                this.actorRequestProcessingTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorRequestProcessingTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorRequestProcessingTimeMillisecBaseCounterName));

                this.actorLockAcquireWaitTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorLockAcquireWaitTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorLockAcquireWaitTimeMillisecBaseCounterName));

                this.actorLockHoldTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorLockHoldTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorLockHoldTimeMillisecBaseCounterName));

                this.actorRequestDeserializationTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorRequestDeserializationTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorRequestDeserializationTimeMillisecBaseCounterName));

                this.actorResponseSerializationTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorResponseSerializationTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorResponseSerializationTimeMillisecBaseCounterName));

                this.actorOnActivateAsyncTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorOnActivateAsyncTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorOnActivateAsyncTimeMillisecBaseCounterName));

                this.actorSaveStateTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(ActorSaveStateTimeCounterWriter),
                    () => new ActorSaveStateTimeCounterWriter(this.actorCounterSetInstance));

                this.actorLoadStateTimeCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricAverageCount64PerformanceCounterWriter),
                    () => new FabricAverageCount64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorLoadStateTimeMillisecCounterName,
                                ActorPerformanceCounters.ActorLoadStateTimeMillisecBaseCounterName));

                this.actorOutstandingRequestsCounterWriter = this.CreateCounterWriter(
                    actorCounterInstanceName,
                    typeof(FabricNumberOfItems64PerformanceCounterWriter),
                    () => new FabricNumberOfItems64PerformanceCounterWriter(
                                this.actorCounterSetInstance,
                                ActorPerformanceCounters.ActorOutstandingRequestsCounterName));
            }
        }

        private T CreateCounterWriter<T>(string actorCounterInstanceName, Type writerType, Func<T> writerCreationCallback)
        {
            T result = default(T);
            Exception ex = null;
            try
            {
                result = writerCreationCallback();
            }
            catch (Exception e)
            {
                ex = e;
            }

            this.LogCounterInstanceCreationResult(writerType, actorCounterInstanceName, ex);
            return result;
        }

        private static void DumpCounterSetInfo(FabricPerformanceCounterSet counterSet, IEnumerable<FabricPerformanceCounterDefinition> activeCounters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("Created performance counter category {0} with following counters.", counterSet.CounterSetDefinition.Name));
            sb.AppendLine();
            foreach(var counter in activeCounters)
            {
                sb.Append(String.Format("CounterName : {0}", counter.Name));
                sb.AppendLine();
            }
            ActorTrace.Source.WriteInfo(TraceType, sb.ToString());
        }

        internal static void InitializeAvailableCounterTypes()
        {
            ActorPerformanceCounters actorPerformanceCounters = new ActorPerformanceCounters();
            var requestedCounterSets = actorPerformanceCounters.GetCounterSets();

            AvaiableFabricCounterSet = new Dictionary<string, FabricPerformanceCounterSet>();

            foreach(FabricPerformanceCounterSetDefinition category in requestedCounterSets.Keys)
            {
                FabricPerformanceCounterSet counterSet;
                try
                {
                    //Creates CounterSet for this category.
                    counterSet = new FabricPerformanceCounterSet(category, requestedCounterSets[category]);
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteWarning(TraceType,
                        "Data for performance counter category {0} will not be provided because an exception occurred during its initialization. Exception info: {1}",
                        category.Name, ex);
                    continue;
                }
                DumpCounterSetInfo(counterSet, requestedCounterSets[category]);
                AvaiableFabricCounterSet.Add(category.Name, counterSet);
            }
        }

        internal void RegisterWithDiagnosticsEventManager(DiagnosticsEventManager diagnosticsEventManager)
        {
            this.InitializeActorMethodInfo(diagnosticsEventManager);

            diagnosticsEventManager.OnActorMethodFinish += this.OnActorMethodFinish;
            diagnosticsEventManager.OnPendingActorMethodCallsUpdated += this.OnPendingActorMethodCallsUpdated;
            diagnosticsEventManager.OnSaveActorStateFinish += this.OnSaveActorStateFinish;
            diagnosticsEventManager.OnActorRequestProcessingStart += this.OnActorRequestProcessingStart;
            diagnosticsEventManager.OnActorRequestProcessingFinish += this.OnActorRequestProcessingFinish;
            diagnosticsEventManager.OnActorLockAcquired += this.OnActorLockAcquired;
            diagnosticsEventManager.OnActorLockReleased += this.OnActorLockReleased;
            diagnosticsEventManager.OnActorRequestDeserializationFinish += this.OnActorRequestDeserializationFinish;
            diagnosticsEventManager.OnActorResponseSerializationFinish += this.OnActorResponseSerializationFinish;
            diagnosticsEventManager.OnActorOnActivateAsyncFinish += this.OnActorOnActivateAsyncFinish;
            diagnosticsEventManager.OnLoadActorStateFinish += this.OnLoadActorStateFinish;
        }

        private void InitializeActorMethodInfo(DiagnosticsEventManager diagnosticsEventManager)
        {
            this.actorMethodCounterInstanceData = new Dictionary<long, CounterInstanceData>();
            var methodInfoList = new List<KeyValuePair<long, MethodInfo>>();
            foreach (var actorInterfaceType in this.actorTypeInformation.InterfaceTypes)
            {
                int interfaceId;
                MethodDescription[] actorInterfaceMethodDescriptions;
                diagnosticsEventManager.ActorMethodFriendlyNameBuilder.GetActorInterfaceMethodDescriptions(actorInterfaceType, out interfaceId, out actorInterfaceMethodDescriptions);
                foreach (var actorInterfaceMethodDescription in actorInterfaceMethodDescriptions)
                {
                    var kvp = new KeyValuePair<long, MethodInfo>(
                        DiagnosticsEventManager.GetInterfaceMethodKey((uint)interfaceId, (uint)actorInterfaceMethodDescription.Id),
                        actorInterfaceMethodDescription.MethodInfo);
                    methodInfoList.Add(kvp);
                }
            }

            // Computes the counter instance names for all the actor methods.
            var percCounterInstanceNameBuilder = new PerformanceCounterInstanceNameBuilder(this.partitionId, this.counterInstanceDifferentiator);
            var counterInstanceNames = percCounterInstanceNameBuilder.GetMethodCounterInstanceNames(methodInfoList);
            foreach (var kvp in counterInstanceNames)
            {
                this.actorMethodCounterInstanceData[kvp.Key] = new CounterInstanceData { InstanceName = kvp.Value };
            }
        }

        private void OnActorMethodFinish(ActorMethodDiagnosticData methodData)
        {
            long interfaceMethodKey = methodData.InterfaceMethodKey;
            MethodSpecificCounterWriters counterWriters = this.actorMethodCounterInstanceData[interfaceMethodKey].CounterWriters;

            if (!AvaiableFabricCounterSet.TryGetValue(ActorPerformanceCounters.ActorMethodCategoryName, out ActorMethodCounterSet))
            {
                return;
            }
            
            if (counterWriters == null )
            {
                bool logCounterWriterCreation = false;
                lock (this.actorMethodCounterInstanceData[interfaceMethodKey])
                {
                    if (this.actorMethodCounterInstanceData[interfaceMethodKey].CounterWriters == null)
                    {
                        // We have not yet created the objects that write the counter values. So build
                        // up the list of counter writers now.
                        string instanceName = this.actorMethodCounterInstanceData[interfaceMethodKey].InstanceName;

                        var tempCounterWriters = new MethodSpecificCounterWriters();

                        try
                        {
                            tempCounterWriters.ActorMethodCounterSetInstance =
                                ActorMethodCounterSet.CreateCounterSetInstance(instanceName);
                        }
                        catch (Exception ex)
                        {
                            //Displays a warning when the creation of this instance failed.
                            ActorTrace.Source.WriteWarning(
                                TraceType,
                                "Data for performance counter instance {0} of category {1} will not be provided because an exception occurred during its initialization. Exception info: {2}",
                                instanceName, ActorPerformanceCounters.ActorMethodCategoryName, ex);
                            return;
                        }

                        tempCounterWriters.ActorMethodFrequencyCounterWriter = this.CreateMethodCounterWriter(
                            instanceName,
                            typeof(ActorMethodFrequencyCounterWriter),
                            tempCounterWriters.ActorMethodCounterSetInstance,
                            inst => new ActorMethodFrequencyCounterWriter(inst));
                        tempCounterWriters.ActorMethodExceptionFrequencyCounterWriter = this.CreateMethodCounterWriter(
                            instanceName,
                            typeof(ActorMethodExceptionFrequencyCounterWriter),
                            tempCounterWriters.ActorMethodCounterSetInstance,
                            inst => new ActorMethodExceptionFrequencyCounterWriter(inst));
                        tempCounterWriters.ActorMethodExecTimeCounterWriter = this.CreateMethodCounterWriter(
                            instanceName,
                            typeof(ActorMethodExecTimeCounterWriter),
                            tempCounterWriters.ActorMethodCounterSetInstance,
                            inst => new ActorMethodExecTimeCounterWriter(inst));
                        logCounterWriterCreation = true;

                        this.actorMethodCounterInstanceData[interfaceMethodKey].CounterWriters = tempCounterWriters;
                    }
                }
                counterWriters = this.actorMethodCounterInstanceData[interfaceMethodKey].CounterWriters;

                if (logCounterWriterCreation)
                {
                    object[] newlyCreatedCounterWriters =
                    {
                        counterWriters.ActorMethodFrequencyCounterWriter,
                        counterWriters.ActorMethodExceptionFrequencyCounterWriter,
                        counterWriters.ActorMethodExecTimeCounterWriter
                    };
                    foreach (var newlyCreatedCounterWriter in newlyCreatedCounterWriters)
                    {
                        if (null != newlyCreatedCounterWriter)
                        {
                            this.LogCounterInstanceCreationResult(
                                newlyCreatedCounterWriter.GetType(),
                                this.actorMethodCounterInstanceData[interfaceMethodKey].InstanceName,
                                null);
                        }
                    }
                }
            }

            // Calls the counter writers to update the counter values
            if (null != counterWriters.ActorMethodFrequencyCounterWriter)
            {
                counterWriters.ActorMethodFrequencyCounterWriter.UpdateCounterValue();
            }
            if (null != counterWriters.ActorMethodExceptionFrequencyCounterWriter)
            {
                counterWriters.ActorMethodExceptionFrequencyCounterWriter.UpdateCounterValue(methodData);
            }
            if (null != counterWriters.ActorMethodExecTimeCounterWriter)
            {
                counterWriters.ActorMethodExecTimeCounterWriter.UpdateCounterValue(methodData);
            }
        }

        private T CreateMethodCounterWriter<T>(string instanceName, Type counterWriterType, FabricPerformanceCounterSetInstance instance, Func<FabricPerformanceCounterSetInstance, T> counterWriterCreationCallback)
        {
            T retVal = default(T);
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

        private void OnPendingActorMethodCallsUpdated(PendingActorMethodDiagnosticData pendingMethodData)
        {
            if (null != this.actorLockContentionCounterWriter)
            {
                this.actorLockContentionCounterWriter.UpdateCounterValue(pendingMethodData);
            }
        }

        private void OnSaveActorStateFinish(ActorStateDiagnosticData stateData)
        {
            if (null != this.actorSaveStateTimeCounterWriter)
            {
                this.actorSaveStateTimeCounterWriter.UpdateCounterValue(stateData);
            }
        }

        private void OnActorRequestProcessingStart()
        {
            if (null != this.actorOutstandingRequestsCounterWriter)
            {
                this.actorOutstandingRequestsCounterWriter.UpdateCounterValue(1);
            }
        }

        private void OnActorRequestProcessingFinish(TimeSpan processingTime)
        {
            if (null != this.actorOutstandingRequestsCounterWriter)
            {
                this.actorOutstandingRequestsCounterWriter.UpdateCounterValue(-1);
            }
            if (null != this.actorRequestProcessingTimeCounterWriter)
            {
                this.actorRequestProcessingTimeCounterWriter.UpdateCounterValue((long) processingTime.TotalMilliseconds);
            }
        }

        private void OnActorLockAcquired(TimeSpan lockAcquireWaitTime)
        {
            if (null != this.actorLockAcquireWaitTimeCounterWriter)
            {
                this.actorLockAcquireWaitTimeCounterWriter.UpdateCounterValue((long) lockAcquireWaitTime.TotalMilliseconds);
            }
        }

        private void OnActorLockReleased(TimeSpan lockHoldTime)
        {
            if (null != this.actorLockHoldTimeCounterWriter)
            {
                this.actorLockHoldTimeCounterWriter.UpdateCounterValue((long) lockHoldTime.TotalMilliseconds);
            }
        }

        private void OnActorRequestDeserializationFinish(TimeSpan deserializationTime)
        {
            if (null != this.actorRequestDeserializationTimeCounterWriter)
            {
                this.actorRequestDeserializationTimeCounterWriter.UpdateCounterValue((long)deserializationTime.TotalMilliseconds);
            }
        }

        private void OnActorResponseSerializationFinish(TimeSpan serializationTime)
        {
            if (null != this.actorResponseSerializationTimeCounterWriter)
            {
                this.actorResponseSerializationTimeCounterWriter.UpdateCounterValue((long)serializationTime.TotalMilliseconds);
            }
        }

        private void OnActorOnActivateAsyncFinish(TimeSpan onActivateAsyncTime)
        {
            if (null != this.actorOnActivateAsyncTimeCounterWriter)
            {
                this.actorOnActivateAsyncTimeCounterWriter.UpdateCounterValue((long)onActivateAsyncTime.TotalMilliseconds);
            }
        }

        private void OnLoadActorStateFinish(TimeSpan loadStateTime)
        {
            if (null != this.actorLoadStateTimeCounterWriter)
            {
                this.actorLoadStateTimeCounterWriter.UpdateCounterValue((long)loadStateTime.TotalMilliseconds);
            }
        }

        private class CounterInstanceData
        {
            internal MethodSpecificCounterWriters CounterWriters;
            internal string InstanceName;
        }

        private class MethodSpecificCounterWriters
        {
            internal FabricPerformanceCounterSetInstance ActorMethodCounterSetInstance;
            internal ActorMethodFrequencyCounterWriter ActorMethodFrequencyCounterWriter;
            internal ActorMethodExceptionFrequencyCounterWriter ActorMethodExceptionFrequencyCounterWriter;
            internal ActorMethodExecTimeCounterWriter ActorMethodExecTimeCounterWriter;
        }

        private void LogCounterInstanceCreationResult(Type counterWriterType, string instanceName, Exception e)
        {
            if (null == e)
            {
                // Success
                ActorTrace.Source.WriteInfo(
                    TraceType,
                    "Performance counter writer {0} enabled for counter instance {1}.",
                    counterWriterType,
                    instanceName);
            }
            else
            {
                // Failure
                ActorTrace.Source.WriteWarning(
                    TraceType,
                    "Performance counter writer {0} for instance {1} has been disabled because an exception occurred during its initialization. Exception info: {2}",
                    counterWriterType,
                    instanceName,
                    e);
            }

        }

        public void Dispose()
        {           
            if (null != this.actorCounterSetInstance)
            {
                //Removes Counter Instance.
                this.actorCounterSetInstance.Dispose();
            }
            if (null != this.actorMethodCounterInstanceData)
            {
                foreach (var counterInstanceData in this.actorMethodCounterInstanceData.Values)
                {
                    if (null != counterInstanceData.CounterWriters)
                    {
                        if (null != counterInstanceData.CounterWriters.ActorMethodCounterSetInstance)
                        {
                            //Removes Counter Instance.
                            counterInstanceData.CounterWriters.ActorMethodCounterSetInstance.Dispose();
                        }
                    }
                }
            }
        }
    }
}
