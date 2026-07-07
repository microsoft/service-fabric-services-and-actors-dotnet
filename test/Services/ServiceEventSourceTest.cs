// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services
{
    public sealed class ServiceEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ServiceEventSource> test = new EventSourceTest<ServiceEventSource>();

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        // Method parameters
        readonly string type = fuzzy.String();
        readonly string clusterOsType = fuzzy.String();
        readonly string runtimePlatform = fuzzy.String();
        readonly string partitionId = fuzzy.String();
        readonly string replicaOrInstanceId = fuzzy.String();
        readonly string replicaId = fuzzy.String();
        readonly string serviceName = fuzzy.String();
        readonly string serviceTypeName = fuzzy.String();
        readonly string applicationName = fuzzy.String();
        readonly string applicationTypeName = fuzzy.String();
        readonly string lifecycleEvent = fuzzy.String();
        readonly string serviceKind = fuzzy.String();
        readonly string communicationListenerType = fuzzy.String();
        readonly bool isSecure = fuzzy.Boolean();
        readonly string remotingVersion = fuzzy.String();

        public void Dispose() =>
            test.Dispose();

        [Fact]
        public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new Guid("27b7a543-7280-5c2a-b053-f2f798e2cbb7"), test.Instance.Guid);

        [Fact]
        public void ManifestCanBeSavedForRegistrationWithExternalTools() =>
            test.Manifest();

        [Fact]
        public void ErrorTextPublishesExpectedEvent() =>
            test.ITextEventSource.ErrorText();

        [Fact]
        public void InfoTextPublishesExpectedEvent() =>
            test.ITextEventSource.InfoText();

        [Fact]
        public void NoiseTextPublishesExpectedEvent() =>
            test.ITextEventSource.NoiseText();

        [Fact]
        public void WarningTextPublishesExpectedEvent() =>
            test.ITextEventSource.WarningText();

        [Fact]
        public void ServiceLifecycleEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ServiceLifecycleEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaOrInstanceId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                lifecycleEvent,
                serviceKind);

            Assert.NotNull(test.Event);
            Assert.Equal(5, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ServiceEventSource.Keywords.Default);
            Assert.Equal("ServiceLifecycleEvent", test.Event.EventName);
            Assert.Equal(ServiceEventSource.ServiceLifecycleEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "partitionId", partitionId);
            test.EventPayload(4, "replicaOrInstanceId", replicaOrInstanceId);
            test.EventPayload(5, "serviceName", serviceName);
            test.EventPayload(6, "serviceTypeName", serviceTypeName);
            test.EventPayload(7, "applicationName", applicationName);
            test.EventPayload(8, "applicationTypeName", applicationTypeName);
            test.EventPayload(9, "lifecycleEvent", lifecycleEvent);
            test.EventPayload(10, "serviceKind", serviceKind);
        }

        [Fact]
        public void CommunicationListenerUsageEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.CommunicationListenerUsageEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                communicationListenerType);

            Assert.NotNull(test.Event);
            Assert.Equal(6, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ServiceEventSource.Keywords.Default);
            Assert.Equal("CommunicationListenerUsageEvent", test.Event.EventName);
            Assert.Equal(ServiceEventSource.CommunicationListenerUsageEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "partitionId", partitionId);
            test.EventPayload(4, "replicaId", replicaId);
            test.EventPayload(5, "serviceName", serviceName);
            test.EventPayload(6, "serviceTypeName", serviceTypeName);
            test.EventPayload(7, "applicationName", applicationName);
            test.EventPayload(8, "applicationTypeName", applicationTypeName);
            test.EventPayload(9, "communicationListenerType", communicationListenerType);
        }

        [Fact]
        public void ServiceRemotingUsageEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ServiceRemotingUsageEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                isSecure,
                remotingVersion,
                communicationListenerType);

            Assert.NotNull(test.Event);
            Assert.Equal(7, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ServiceEventSource.Keywords.Default);
            Assert.Equal("ServiceRemotingUsageEvent", test.Event.EventName);
            Assert.Equal(ServiceEventSource.ServiceRemotingUsageEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "partitionId", partitionId);
            test.EventPayload(4, "replicaId", replicaId);
            test.EventPayload(5, "serviceName", serviceName);
            test.EventPayload(6, "serviceTypeName", serviceTypeName);
            test.EventPayload(7, "applicationName", applicationName);
            test.EventPayload(8, "applicationTypeName", applicationTypeName);
            test.EventPayload(9, "isSecure", isSecure);
            test.EventPayload(10, "remotingVersion", remotingVersion);
            test.EventPayload(11, "communicationListenerType", communicationListenerType);
        }
    }
}
