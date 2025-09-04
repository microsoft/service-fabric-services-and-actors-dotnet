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
    public class ServiceMeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public class Constructor : ServiceMeterProviderTest, IDisposable
        {
            public Constructor()
            {
                typeof(ServiceMeterProvider<int>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
            }

            public void Dispose()
            {
                typeof(ServiceMeterProvider<int>).Field<Func<IFabricMeterProvider>>().Set(NativeTelemetry.FabricCreateMeterProvider);
            }

            [Fact]
            public void SetsRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(serviceContext);

                var actualValues = (string[])sut.Protected().Field<IEnumerable<string>>().Value;

                Assert.Equal(serviceContext.ReplicaOrInstanceId.ToString(), actualValues[0]);
                Assert.Equal(serviceContext.PartitionId.ToString(), actualValues[1]);
                Assert.Equal(serviceContext.ServiceTypeName, actualValues[2]);
                Assert.Equal(serviceContext.ServiceName.ToString(), actualValues[3]);
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationName, actualValues[4]);
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationTypeName, actualValues[5]);
            }

            [Fact]
            public void ThrowAnArgumentExceptionWhenServiceContextNull()
            {
                Assert.Throws<ArgumentNullException>(() => new TestMeterProvider<int>(null));
            }
        }

        public class Class : ServiceMeterProviderTest
        {
            [Fact]
            public void HasFabricCreateMeterProviderFunc()
            {
                Func<IFabricMeterProvider> expected = typeof(NativeTelemetry).Method<Func<IFabricMeterProvider>>(nameof(NativeTelemetry.FabricCreateMeterProvider));
                Func<IFabricMeterProvider> actual = typeof(ServiceMeterProvider<int>).Field<Func<IFabricMeterProvider>>();
                Assert.Equal(expected, actual);
            }
        }

        class TestMeterProvider<TValueType> : ServiceMeterProvider<TValueType>
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
