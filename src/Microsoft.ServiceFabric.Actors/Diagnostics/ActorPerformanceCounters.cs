// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;

    internal class ActorPerformanceCounters : IFabricPerformanceCountersDefinition
    {
        internal const string ActorMethodCategoryName = "Service Fabric Actor Method";
        internal const string ActorMethodInvocationsPerSecCounterName = "Invocations/Sec";
        internal const string ActorMethodExecTimeMillisecCounterName = "Average milliseconds per invocation";
        internal const string ActorMethodExecTimeMillisecBaseCounterName = "Average milliseconds per invocation base";
        internal const string ActorMethodExceptionsPerSecCounterName = "Exceptions thrown/Sec";

        internal const string ActorCategoryName = "Service Fabric Actor";
        internal const string ActorCallsWaitingForLockCounterName = "# of actor calls waiting for actor lock";
        internal const string ActorSaveStateTimeMillisecCounterName = "Average milliseconds per save state operation";
        internal const string ActorSaveStateTimeMillisecBaseCounterName = "Average milliseconds per save state operation base";
        internal const string ActorOnActivateAsyncTimeMillisecCounterName = "Average OnActivateAsync milliseconds";
        internal const string ActorOnActivateAsyncTimeMillisecBaseCounterName = "Average OnActivateAsync milliseconds base";
        internal const string ActorRequestProcessingTimeMillisecCounterName = "Average milliseconds per request";
        internal const string ActorRequestProcessingTimeMillisecBaseCounterName = "Average milliseconds per request base";
        internal const string ActorLockAcquireWaitTimeMillisecCounterName = "Average milliseconds per lock wait";
        internal const string ActorLockAcquireWaitTimeMillisecBaseCounterName = "Average milliseconds per lock wait base";
        internal const string ActorLockHoldTimeMillisecCounterName = "Average milliseconds actor lock held";
        internal const string ActorLockHoldTimeMillisecBaseCounterName = "Average milliseconds actor lock held base";
        internal const string ActorRequestDeserializationTimeMillisecCounterName = "Average milliseconds for request deserialization";
        internal const string ActorRequestDeserializationTimeMillisecBaseCounterName = "Average milliseconds for request deserialization base";
        internal const string ActorResponseSerializationTimeMillisecCounterName = "Average milliseconds for response serialization";
        internal const string ActorResponseSerializationTimeMillisecBaseCounterName = "Average milliseconds for response serialization base";
        internal const string ActorLoadStateTimeMillisecCounterName = "Average milliseconds per load state operation";
        internal const string ActorLoadStateTimeMillisecBaseCounterName = "Average milliseconds per load state operation base";
        internal const string ActorOutstandingRequestsCounterName = "# of outstanding requests";

        private static readonly Dictionary<Tuple<string, string>, FabricPerformanceCounterType> CounterTypes = new Dictionary
            <Tuple<string, string>, FabricPerformanceCounterType>()
        {
            {
                Tuple.Create(ActorMethodCategoryName, ActorMethodInvocationsPerSecCounterName),
                FabricPerformanceCounterType.RateOfCountsPerSecond64
            },
            {
                Tuple.Create(ActorMethodCategoryName, ActorMethodExecTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorMethodCategoryName, ActorMethodExecTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorMethodCategoryName, ActorMethodExceptionsPerSecCounterName),
                FabricPerformanceCounterType.RateOfCountsPerSecond64
            },
            {
                Tuple.Create(ActorCategoryName, ActorCallsWaitingForLockCounterName),
                FabricPerformanceCounterType.NumberOfItems64
            },
            {
                Tuple.Create(ActorCategoryName, ActorSaveStateTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorSaveStateTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorRequestProcessingTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorRequestProcessingTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorLockAcquireWaitTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorLockAcquireWaitTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorLockHoldTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorLockHoldTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorRequestDeserializationTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorRequestDeserializationTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorResponseSerializationTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorResponseSerializationTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorOnActivateAsyncTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorOnActivateAsyncTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorLoadStateTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ActorCategoryName, ActorLoadStateTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ActorCategoryName, ActorOutstandingRequestsCounterName),
                FabricPerformanceCounterType.NumberOfItems64
            },
        };

        private static readonly Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>> CounterSets = new Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>>()
        {
            {
                new FabricPerformanceCounterSetDefinition(
                    ActorMethodCategoryName,
                    "Counters for methods implemented by Service Fabric actor services",
                    FabricPerformanceCounterCategoryType.MultiInstance,
                    new Guid("7D3CC77C-4631-4FD3-8FA2-F7FBFC261272"),
                    "ServiceFabricActorMethod"), 
                new[]
                {
                    new FabricPerformanceCounterDefinition(
                            1,
                            ActorMethodInvocationsPerSecCounterName,
                            "Number of times the actor service method is invoked per second", 
                            GetType(ActorMethodCategoryName, ActorMethodInvocationsPerSecCounterName),
                            "InvocationsPerSecond"),

                    new FabricPerformanceCounterDefinition(
                            2,
                            3,
                            ActorMethodExecTimeMillisecCounterName,
                            "Time taken to execute the actor service method in milliseconds", 
                            GetType(ActorMethodCategoryName, ActorMethodExecTimeMillisecCounterName),
                            "ExecutionTime"),

                    new FabricPerformanceCounterDefinition(
                            3,
                            ActorMethodExecTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorMethodCategoryName, ActorMethodExecTimeMillisecBaseCounterName),
                            "ExecutionTimeBase",
                            new[] {"noDisplay"}),

                    new FabricPerformanceCounterDefinition(
                            4,
                            ActorMethodExceptionsPerSecCounterName,
                            "Number of times the actor service method threw an exception per second", 
                            GetType(ActorMethodCategoryName, ActorMethodExceptionsPerSecCounterName), 
                            "ExceptionsPerSecond"),
                }
            },
            {
                new FabricPerformanceCounterSetDefinition(
                    ActorCategoryName,
                    "Counters for Service Fabric actor services",
                    FabricPerformanceCounterCategoryType.MultiInstance,
                    new Guid("FA62271D-5CE2-4BA9-93B5-6CE5FFE10486"),
                    "ServiceFabricActor"),
                new[]
                {
                    new FabricPerformanceCounterDefinition(
                            1,
                            ActorCallsWaitingForLockCounterName,
                            "Number of pending actor calls waiting for per-actor lock to be acquired",
                            GetType(ActorCategoryName, ActorCallsWaitingForLockCounterName),
                            "NumberOfActorCallWaitingForActorLock"),
                    new FabricPerformanceCounterDefinition(
                            2,
                            3,
                            ActorSaveStateTimeMillisecCounterName,
                            "Time taken to save actor state in milliseconds", 
                            GetType(ActorCategoryName, ActorSaveStateTimeMillisecCounterName),
                            "SaveStateTime"),
                    new FabricPerformanceCounterDefinition(
                            3,
                            ActorSaveStateTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorSaveStateTimeMillisecBaseCounterName),
                            "SaveStateTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            4,
                            5,
                            ActorRequestProcessingTimeMillisecCounterName,
                            "Time taken to process actor request in milliseconds", 
                            GetType(ActorCategoryName, ActorRequestProcessingTimeMillisecCounterName),
                            "MethodProcessingTime"),
                    new FabricPerformanceCounterDefinition(
                            5,
                            ActorRequestProcessingTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorRequestProcessingTimeMillisecBaseCounterName),
                            "MethodProcessingTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            6,
                            7,
                            ActorLockAcquireWaitTimeMillisecCounterName,
                            "Time taken to acquire actor lock in milliseconds", 
                            GetType(ActorCategoryName, ActorLockAcquireWaitTimeMillisecCounterName),
                            "LockAcquireTime"),
                    new FabricPerformanceCounterDefinition(
                            7,
                            ActorLockAcquireWaitTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorLockAcquireWaitTimeMillisecBaseCounterName),
                            "LockAcquireTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            8,
                            9,
                            ActorLockHoldTimeMillisecCounterName,
                            "Time for which actor lock is held in milliseconds", 
                            GetType(ActorCategoryName, ActorLockHoldTimeMillisecCounterName),
                            "LockHoldTime"),
                    new FabricPerformanceCounterDefinition(
                            9,
                            ActorLockHoldTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorLockHoldTimeMillisecBaseCounterName),
                            "LockHoldTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            10,
                            11,
                            ActorRequestDeserializationTimeMillisecCounterName,
                            "Actor request deserialization time in milliseconds", 
                            GetType(ActorCategoryName, ActorRequestDeserializationTimeMillisecCounterName),
                            "RequestDeserializationTime"),
                    new FabricPerformanceCounterDefinition(
                            11,
                            ActorRequestDeserializationTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorRequestDeserializationTimeMillisecBaseCounterName),
                            "RequestDeserializationTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            12,
                            13,
                            ActorResponseSerializationTimeMillisecCounterName,
                            "Actor response serialization time in milliseconds", 
                            GetType(ActorCategoryName, ActorResponseSerializationTimeMillisecCounterName),
                            "ResponseSerializationTime"),
                    new FabricPerformanceCounterDefinition(
                            13,
                            ActorResponseSerializationTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorResponseSerializationTimeMillisecBaseCounterName),
                            "ResponseSerializationTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            14,
                            15,
                            ActorOnActivateAsyncTimeMillisecCounterName,
                            "Time taken to execute OnActivateAsync method in milliseconds", 
                            GetType(ActorCategoryName, ActorOnActivateAsyncTimeMillisecCounterName),
                            "OnActivateAsyncTime"),
                    new FabricPerformanceCounterDefinition(
                            15,
                            ActorOnActivateAsyncTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorOnActivateAsyncTimeMillisecBaseCounterName),
                            "OnActivateAsyncTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            16,
                            17,
                            ActorLoadStateTimeMillisecCounterName,
                            "Time taken to load actor state in milliseconds", 
                            GetType(ActorCategoryName, ActorLoadStateTimeMillisecCounterName),
                            "LoadStateTime"),
                    new FabricPerformanceCounterDefinition(
                            17,
                            ActorLoadStateTimeMillisecBaseCounterName,
                            "", 
                            GetType(ActorCategoryName, ActorLoadStateTimeMillisecBaseCounterName),
                            "LoadStateTimeBase",
                            new[] { "noDisplay" }
                            ),
                    new FabricPerformanceCounterDefinition(
                            18,
                            ActorOutstandingRequestsCounterName,
                            "Number of requests being processed",
                            GetType(ActorCategoryName, ActorOutstandingRequestsCounterName),
                            "NumberOfOutstandingRequests"),
                }
            }
        };

        internal static FabricPerformanceCounterType GetType(string categoryName, string counterName)
        {
            return CounterTypes[Tuple.Create(categoryName, counterName)];
        }

        public Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>> GetCounterSets()
        {
            return CounterSets;
        }
    }
}
