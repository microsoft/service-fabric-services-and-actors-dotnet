// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    extern alias Microsoft_ServiceFabric_Internal;

    using System;
    using System.Collections.Generic;
    using FabricPerformanceCounterCategoryType = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterCategoryType;
    using FabricPerformanceCounterDefinition = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterDefinition;
    using FabricPerformanceCounterSetDefinition = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterSetDefinition;
    using FabricPerformanceCounterType = Microsoft_ServiceFabric_Internal::System.Fabric.Common.FabricPerformanceCounterType;
    using IFabricPerformanceCountersDefinition = Microsoft_ServiceFabric_Internal::System.Fabric.Common.IFabricPerformanceCountersDefinition;

    internal class ServiceRemotingPerformanceCounters : IFabricPerformanceCountersDefinition
    {
        internal const string ServiceCategoryName = "Service Fabric Service Remoting";
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
                            ServiceCategoryName,
                            "Counters for Service Fabric Service Remoting",
                            FabricPerformanceCounterCategoryType.MultiInstance,
                            new Guid("821B0C50-C020-472E-8380-A63084209727"),
                            "ServiceFabricRemotingService"),
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
                                string.Empty,
                                GetType(ServiceCategoryName, ServiceRequestProcessingTimeMillisecBaseCounterName),
                                "MethodProcessingTimeBase",
                                new[] { "noDisplay" }),
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
                                string.Empty,
                                GetType(ServiceCategoryName, ServiceRequestDeserializationTimeMillisecBaseCounterName),
                                "RequestDeserializationTimeBase",
                                new[] { "noDisplay" }),
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
                                string.Empty,
                                GetType(ServiceCategoryName, ServiceResponseSerializationTimeMillisecBaseCounterName),
                                "ResponseSerializationTimeBase",
                                new[] { "noDisplay" }),
                            new FabricPerformanceCounterDefinition(
                                7,
                                ServiceOutstandingRequestsCounterName,
                                "Number of requests being processed",
                                GetType(ServiceCategoryName, ServiceOutstandingRequestsCounterName),
                                "NumberOfOutstandingRequests"),
                        }
                    },
                };

        public Dictionary<FabricPerformanceCounterSetDefinition, IEnumerable<FabricPerformanceCounterDefinition>>
            GetCounterSets()
        {
            return CounterSets;
        }

        internal static FabricPerformanceCounterType GetType(string categoryName, string counterName)
        {
            return CounterTypes[Tuple.Create(categoryName, counterName)];
        }
    }
}
