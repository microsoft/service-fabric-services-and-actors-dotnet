// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;

namespace Microsoft.ServiceFabric.TestFramework
{
    /// <summary>
    /// Classes that have a direct or indirect dependency on Metrics API (<see cref="IMeter"/>, <see cref="IMeterProvider"/>) end up calling native metrics Interop which is unavailable during unit testing
    /// We still want to test such classes, and in order to do so we need to replace calls to the native code with a mock, which is done in this class. Any test that needs this behaviour can inheirt this class.
    /// </summary>
    public class MockedMetricsTest : IDisposable
    {
        protected MockedMetricsTest()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
        }
        /// <summary>
        /// Implementation of <see cref="MeterProvider{TValueType}"/> for long integer metrics.
        /// Creates meters for recording long integer telemetry values with various dimension configurations.
        /// </summary>
        public virtual void Dispose()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
        }
    }
}
