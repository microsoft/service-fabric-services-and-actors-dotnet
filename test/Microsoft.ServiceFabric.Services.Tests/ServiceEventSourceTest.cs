// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services
{
    public sealed class ServiceEventSourceTest: IDisposable
    {
        readonly EventSourceTest<ServiceEventSource> test;

        public ServiceEventSourceTest(ITestOutputHelper output) =>
            test = new EventSourceTest<ServiceEventSource>(output);

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
    }
}
