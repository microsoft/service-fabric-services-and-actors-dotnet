// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Actors
{
    public sealed class ActorEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ActorEventSource> test = new EventSourceTest<ActorEventSource>();

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        // Method parameters
        readonly string type = fuzzy.String();
        readonly string clusterOsType = fuzzy.String();
        readonly string runtimePlatform = fuzzy.String();
        readonly string partitionId = fuzzy.String();
        readonly string replicaId = fuzzy.String();
        readonly string serviceName = fuzzy.String();
        readonly string serviceTypeName = fuzzy.String();
        readonly string applicationName = fuzzy.String();
        readonly string applicationTypeName = fuzzy.String();
        readonly string stateProviderName = fuzzy.String();
        readonly string actorType = fuzzy.String();
        readonly string actorServiceType = fuzzy.String();
        readonly string ownerActorId = fuzzy.String();
        readonly string reminderPeriod = fuzzy.String();
        readonly string reminderName = fuzzy.String();

        public void Dispose() =>
            test.Dispose();

        [Fact]
        public void GuidRemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new Guid("e2f2656b-985e-5c5b-5ba3-bbe8a851e1d7"), test.Instance.Guid);

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
        public void ActorStateProviderUsageEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorStateProviderUsageEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                stateProviderName);

            Assert.NotNull(test.Event);
            Assert.Equal(5, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorEventSource.Keywords.Default);
            Assert.Equal("ActorStateProviderUsageEvent", test.Event.EventName);
            Assert.Equal(ActorEventSource.ActorStateProviderUsageEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "partitionId", partitionId);
            test.EventPayload(4, "replicaId", replicaId);
            test.EventPayload(5, "serviceName", serviceName);
            test.EventPayload(6, "serviceTypeName", serviceTypeName);
            test.EventPayload(7, "applicationName", applicationName);
            test.EventPayload(8, "applicationTypeName", applicationTypeName);
            test.EventPayload(9, "stateProviderName", stateProviderName);
        }

        [Fact]
        public void CustomActorServiceUsageEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.CustomActorServiceUsageEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                actorType,
                actorServiceType);

            Assert.NotNull(test.Event);
            Assert.Equal(6, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorEventSource.Keywords.Default);
            Assert.Equal("CustomActorServiceUsageEvent", test.Event.EventName);
            Assert.Equal(ActorEventSource.CustomActorServiceUsageEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "actorType", actorType);
            test.EventPayload(4, "actorServiceType", actorServiceType);
        }

        [Fact]
        public void ActorReminderRegistrationEventWrapperPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorReminderRegistrationEventWrapper(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                ownerActorId,
                reminderPeriod,
                reminderName);

            Assert.NotNull(test.Event);
            Assert.Equal(7, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorEventSource.Keywords.Default);
            Assert.Equal("ActorReminderRegistrationEvent", test.Event.EventName);
            Assert.Equal(ActorEventSource.ActorReminderRegistrationEventTraceFormat, test.Event.Message);
            test.EventPayload(0, "type", type);
            test.EventPayload(1, "clusterOsType", clusterOsType);
            test.EventPayload(2, "runtimePlatform", runtimePlatform);
            test.EventPayload(3, "partitionId", partitionId);
            test.EventPayload(4, "replicaId", replicaId);
            test.EventPayload(5, "serviceName", serviceName);
            test.EventPayload(6, "serviceTypeName", serviceTypeName);
            test.EventPayload(7, "applicationName", applicationName);
            test.EventPayload(8, "applicationTypeName", applicationTypeName);
            test.EventPayload(9, "ownerActorId", ownerActorId);
            test.EventPayload(10, "reminderPeriod", reminderPeriod);
            test.EventPayload(11, "reminderName", reminderName);
        }
    }
}
