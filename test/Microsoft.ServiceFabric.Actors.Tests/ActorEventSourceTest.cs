// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Actors
{
    public sealed class ActorEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ActorEventSource> test;

        public ActorEventSourceTest(ITestOutputHelper output) =>
            test = new EventSourceTest<ActorEventSource>(output);

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
        public void WarningTextPublishesExpectedEvent() =>
            test.ITextEventSource.WarningText();
    }
}
