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
    public class FabricTelemetryDllFixture : IDisposable
    {
        public FabricTelemetryDllFixture()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(() => new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object);
        }

        public virtual void Dispose()
        {
            typeof(MeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
        }
    }
}
