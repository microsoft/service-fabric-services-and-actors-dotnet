// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services;

public abstract class ServiceEventSourceTest : IDisposable
{
    readonly EventSourceTest<ServiceEventSource> test = new();
    readonly ServiceEventSource sut;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceEventSourceTest() =>
        sut = test.Instance;

    void IDisposable.Dispose() =>
        test.Dispose();

    public sealed class CommunicationListenerUsageEventWrapper : ServiceEventSourceTest
    {
        readonly string type = fuzzy.String();
        readonly string clusterOsType = fuzzy.String();
        readonly string runtimePlatform = fuzzy.String();
        readonly string partitionId = fuzzy.String();
        readonly string replicaId = fuzzy.String();
        readonly string serviceName = fuzzy.String();
        readonly string serviceTypeName = fuzzy.String();
        readonly string applicationName = fuzzy.String();
        readonly string applicationTypeName = fuzzy.String();
        readonly string communicationListenerType = fuzzy.String();

        [Fact]
        public void PublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            sut.CommunicationListenerUsageEventWrapper(
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
            Assert.Equal(10, test.Event.Payload.Count);
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
    }

    public sealed class ErrorText : ServiceEventSourceTest
    {
        [Fact]
        public void PublishesExpectedEvent() =>
            test.ITextEventSource.ErrorText();
    }

    public sealed class Guid : ServiceEventSourceTest
    {
        [Fact]
        public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new System.Guid("27b7a543-7280-5c2a-b053-f2f798e2cbb7"), sut.Guid);
    }

    public sealed class InfoText : ServiceEventSourceTest
    {
        [Fact]
        public void PublishesExpectedEvent() =>
            test.ITextEventSource.InfoText();
    }

    public sealed class Manifest : ServiceEventSourceTest
    {
        [Fact]
        public void CanBeSavedForRegistrationWithExternalTools() =>
            test.Manifest();
    }

    public sealed class NoiseText : ServiceEventSourceTest
    {
        [Fact]
        public void PublishesExpectedEvent() =>
            test.ITextEventSource.NoiseText();
    }

    public sealed class ServiceLifecycleEventWrapper : ServiceEventSourceTest
    {
        readonly string type = fuzzy.String();
        readonly string clusterOsType = fuzzy.String();
        readonly string runtimePlatform = fuzzy.String();
        readonly string partitionId = fuzzy.String();
        readonly string replicaOrInstanceId = fuzzy.String();
        readonly string serviceName = fuzzy.String();
        readonly string serviceTypeName = fuzzy.String();
        readonly string applicationName = fuzzy.String();
        readonly string applicationTypeName = fuzzy.String();
        readonly string lifecycleEvent = fuzzy.String();
        readonly string serviceKind = fuzzy.String();

        [Fact]
        public void PublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            sut.ServiceLifecycleEventWrapper(
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
            Assert.Equal(11, test.Event.Payload.Count);
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
    }

    public sealed class ServiceRemotingUsageEventWrapper : ServiceEventSourceTest
    {
        readonly string type = fuzzy.String();
        readonly string clusterOsType = fuzzy.String();
        readonly string runtimePlatform = fuzzy.String();
        readonly string partitionId = fuzzy.String();
        readonly string replicaId = fuzzy.String();
        readonly string serviceName = fuzzy.String();
        readonly string serviceTypeName = fuzzy.String();
        readonly string applicationName = fuzzy.String();
        readonly string applicationTypeName = fuzzy.String();
        readonly string remotingVersion = fuzzy.String();
        readonly string communicationListenerType = fuzzy.String();

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PublishesExpectedEvent(bool isSecure)
        {
            test.EnableEvents(EventLevel.LogAlways);

            sut.ServiceRemotingUsageEventWrapper(
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
            Assert.Equal(12, test.Event.Payload.Count);
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

    public sealed class WarningText : ServiceEventSourceTest
    {
        [Fact]
        public void PublishesExpectedEvent() =>
            test.ITextEventSource.WarningText();
    }
}
