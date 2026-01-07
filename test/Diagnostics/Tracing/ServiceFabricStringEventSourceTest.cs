// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    public sealed class ServiceFabricStringEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ServiceFabricStringEventSource> test = new EventSourceTest<ServiceFabricStringEventSource>();

        public void Dispose() =>
            test.Dispose();

        [Fact]
        public void GuidRemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new Guid("74CF0846-E6A3-4a3e-A10F-80FD527DA5FD"), test.Instance.Guid);

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
    }
}
