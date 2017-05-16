// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Diagnostic
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;

    internal class ServiceRemotingPerformanceCounters : IFabricPerformanceCountersDefinition
    {
        internal const string ServiceMethodCategoryName = "Service Fabric Service Method";
        internal const string ServiceMethodInvocationsPerSecCounterName = "Invocations/Sec";
        internal const string ServiceMethodExecTimeMillisecCounterName = "Average milliseconds per invocation";
        internal const string ServiceMethodExecTimeMillisecBaseCounterName = "Average milliseconds per invocation base";
        internal const string ServiceMethodExceptionsPerSecCounterName = "Exceptions thrown/Sec";

        internal const string ServiceCategoryName = "Service Fabric Service";
        internal const string ServiceRequestProcessingTimeMillisecCounterName = "Average milliseconds per request";

        internal const string ServiceRequestProcessingTimeMillisecBaseCounterName =
            "Average milliseconds per request base";

        internal const string ServiceRequestDeserializationTimeMillisecCounterName =
            "Average milliseconds for request deserialization";

        internal const string ServiceRequestDeserializationTimeMillisecBaseCounterName =
            "Average milliseconds for request deserialization base";

        internal const string ServiceResponseSerializationTimeMillisecCounterName =
            "Average milliseconds for response serialization";

        internal const string ServiceResponseSerializationTimeMillisecBaseCounterName =
            "Average milliseconds for response serialization base";

        internal const string ServiceOutstandingRequestsCounterName = "# of outstanding requests";


        private static readonly Dictionary<Tuple<string, string>, FabricPerformanceCounterType> CounterTypes = new Dictionary
            <Tuple<string, string>, FabricPerformanceCounterType>()
        {
            {
                Tuple.Create(ServiceMethodCategoryName, ServiceMethodInvocationsPerSecCounterName),
                FabricPerformanceCounterType.RateOfCountsPerSecond64
            },
            {
                Tuple.Create(ServiceMethodCategoryName, ServiceMethodExecTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ServiceMethodCategoryName, ServiceMethodExecTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ServiceMethodCategoryName, ServiceMethodExceptionsPerSecCounterName),
                FabricPerformanceCounterType.RateOfCountsPerSecond64
            },

            {
                Tuple.Create(ServiceCategoryName, ServiceRequestProcessingTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ServiceCategoryName, ServiceRequestProcessingTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ServiceCategoryName, ServiceRequestDeserializationTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ServiceCategoryName, ServiceRequestDeserializationTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },
            {
                Tuple.Create(ServiceCategoryName, ServiceResponseSerializationTimeMillisecCounterName),
                FabricPerformanceCounterType.AverageCount64
            },
            {
                Tuple.Create(ServiceCategoryName, ServiceResponseSerializationTimeMillisecBaseCounterName),
                FabricPerformanceCounterType.AverageBase
            },

            {
                Tuple.Create(ServiceCategoryName, ServiceOutstandingRequestsCounterName),
                FabricPerformanceCounterType.NumberOfItems64
            },
        };

        private static readonly
            Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>>
            CounterSets = new Dictionary
                <FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>>()
            {
                {
                    new FabricPerformanceCounterSetDefinition(
                        ServiceMethodCategoryName,
                        "Counters for methods implemented by Service Fabric Service services",
                        FabricPerformanceCounterCategoryType.MultiInstance,
                        new Guid("02A93BC2-A3A0-4D4D-85F6-8B16E25FC418"),
                        "ServiceFabricServiceMethod"),
                    new[]
                    {
                        new FabricPerformanceCounterDefinition(
                            1,
                            ServiceMethodInvocationsPerSecCounterName,
                            "Number of times the Service service method is invoked per second",
                            GetType(ServiceMethodCategoryName, ServiceMethodInvocationsPerSecCounterName),
                            "InvocationsPerSecond"),
                        new FabricPerformanceCounterDefinition(
                            2,
                            3,
                            ServiceMethodExecTimeMillisecCounterName,
                            "Time taken to execute the Service service method in milliseconds",
                            GetType(ServiceMethodCategoryName, ServiceMethodExecTimeMillisecCounterName),
                            "ExecutionTime"),
                        new FabricPerformanceCounterDefinition(
                            3,
                            ServiceMethodExecTimeMillisecBaseCounterName,
                            "",
                            GetType(ServiceMethodCategoryName, ServiceMethodExecTimeMillisecBaseCounterName),
                            "ExecutionTimeBase",
                            new[] {"noDisplay"}),
                        new FabricPerformanceCounterDefinition(
                            4,
                            ServiceMethodExceptionsPerSecCounterName,
                            "Number of times the Service service method threw an exception per second",
                            GetType(ServiceMethodCategoryName, ServiceMethodExceptionsPerSecCounterName),
                            "ExceptionsPerSecond"),
                    }
                },
                {
                    new FabricPerformanceCounterSetDefinition(
                        ServiceCategoryName,
                        "Counters for Service Fabric Service services",
                        FabricPerformanceCounterCategoryType.MultiInstance,
                        new Guid("7E01810A-A821-4319-B4AA-C6C515707114"),
                        "ServiceFabricService"),
                    new[]
                    {
                        new FabricPerformanceCounterDefinition(
                            1,
                            2,
                            ServiceRequestProcessingTimeMillisecCounterName,
                            "Time taken to process Service request in milliseconds",
                            GetType(ServiceCategoryName, ServiceRequestProcessingTimeMillisecCounterName),
                            "MethodProcessingTime"),
                        new FabricPerformanceCounterDefinition(
                            2,
                            ServiceRequestProcessingTimeMillisecBaseCounterName,
                            "",
                            GetType(ServiceCategoryName, ServiceRequestProcessingTimeMillisecBaseCounterName),
                            "MethodProcessingTimeBase",
                            new[] {"noDisplay"}
                            ),
                        new FabricPerformanceCounterDefinition(
                            3,
                            4,
                            ServiceRequestDeserializationTimeMillisecCounterName,
                            "Service request deserialization time in milliseconds",
                            GetType(ServiceCategoryName, ServiceRequestDeserializationTimeMillisecCounterName),
                            "RequestDeserializationTime"),
                        new FabricPerformanceCounterDefinition(
                            4,
                            ServiceRequestDeserializationTimeMillisecBaseCounterName,
                            "",
                            GetType(ServiceCategoryName, ServiceRequestDeserializationTimeMillisecBaseCounterName),
                            "RequestDeserializationTimeBase",
                            new[] {"noDisplay"}
                            ),
                        new FabricPerformanceCounterDefinition(
                            5,
                            6,
                            ServiceResponseSerializationTimeMillisecCounterName,
                            "Service response serialization time in milliseconds",
                            GetType(ServiceCategoryName, ServiceResponseSerializationTimeMillisecCounterName),
                            "ResponseSerializationTime"),
                        new FabricPerformanceCounterDefinition(
                            6,
                            ServiceResponseSerializationTimeMillisecBaseCounterName,
                            "",
                            GetType(ServiceCategoryName, ServiceResponseSerializationTimeMillisecBaseCounterName),
                            "ResponseSerializationTimeBase",
                            new[] {"noDisplay"}
                            ),
                        new FabricPerformanceCounterDefinition(
                            7,
                            ServiceOutstandingRequestsCounterName,
                            "Number of requests being processed",
                            GetType(ServiceCategoryName, ServiceOutstandingRequestsCounterName),
                            "NumberOfOutstandingRequests"),
                    }
                }
            };


        internal static FabricPerformanceCounterType GetType(string categoryName, string counterName)
        {
            return CounterTypes[Tuple.Create(categoryName, counterName)];
        }

        public Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>>
            GetCounterSets()
        {
            return CounterSets;
        }
    }
}
