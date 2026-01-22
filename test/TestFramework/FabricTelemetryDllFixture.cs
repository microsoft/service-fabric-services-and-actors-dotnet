// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.TestFramework
{
    public class FabricTelemetryDllFixture : IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            await Task.CompletedTask;
        }
    }
}
