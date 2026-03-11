// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public class MeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public class Constructor : MeterProviderTest, IDisposable
        {
            public Constructor()
            {
                typeof(MeterProvider<int>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
            }

            public void Dispose()
            {
                typeof(MeterProvider<int>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            }

            [Fact]
            public void SetsRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(serviceContext);

                var actualSystemDimensionNames = (string[])sut.Private().Field<IReadOnlyCollection<string>>().Value;
                var actualSystemDimensionValues = (string[])sut.Protected().Field<IReadOnlyCollection<string>>().Value;

                Assert.Equal(serviceContext.PartitionId.ToString(), actualSystemDimensionValues[0]);
                Assert.Equal(serviceContext.ServiceTypeName, actualSystemDimensionValues[1]);
                Assert.Equal(serviceContext.ServiceName.ToString(), actualSystemDimensionValues[2]);
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationName, actualSystemDimensionValues[3]);
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationTypeName, actualSystemDimensionValues[4]);

                Assert.Equal(nameof(ServiceContext.PartitionId), actualSystemDimensionNames[0]);
                Assert.Equal(nameof(ServiceContext.ServiceTypeName), actualSystemDimensionNames[1]);
                Assert.Equal(nameof(ServiceContext.ServiceName), actualSystemDimensionNames[2]);
                Assert.Equal(nameof(ServiceContext.CodePackageActivationContext.ApplicationName), actualSystemDimensionNames[3]);
                Assert.Equal(nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName), actualSystemDimensionNames[4]);
            }

            [Fact]
            public void ThrowAnArgumentExceptionWhenServiceContextNull()
            {
                var sut = new TestMeterProvider<int>(null);

                var actualSystemDimensionNames = sut.Private().Field<IReadOnlyCollection<string>>().Value;
                var actualSystemDimensionValues = sut.Protected().Field<IReadOnlyCollection<string>>().Value;

                Assert.Empty(actualSystemDimensionValues);
                Assert.Empty(actualSystemDimensionNames);
            }
        }

        public class Class : MeterProviderTest
        {
            [Fact]
            public void HasFabricCreateMeterProviderFunc()
            {
                Func<IFabricMeterProvider> expected = typeof(NativeTelemetry).Method<Func<IFabricMeterProvider>>(nameof(NativeTelemetry.FabricCreateMeterProvider));
                Func<IFabricMeterProvider> actual = typeof(MeterProvider<int>).Field<Func<IFabricMeterProvider>>();
                Assert.Equal(expected, actual);
            }
        }

        class TestMeterProvider<TValueType> : MeterProvider<TValueType>
        {
            public TestMeterProvider(ServiceContext serviceContext)
                : base(serviceContext)
            {
            }

            public override IMeter<TValueType> CreateMeter(string metricNamespace, string name)
            {
                throw new NotImplementedException();
            }

            public override IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name)
            {
                throw new NotImplementedException();
            }

            public override IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name)
            {
                throw new NotImplementedException();
            }

            public override IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name)
            {
                throw new NotImplementedException();
            }
        }
    }
}
