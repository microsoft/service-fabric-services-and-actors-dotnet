// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Interop;
using System.Linq;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public class MeterProviderTest : IDisposable
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IMeterProvider<int> sut;

        readonly IFabricMeterProvider2 fabricMeterProvider = Mock.Of<IFabricMeterProvider2>();
        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public MeterProviderTest()
        {
            typeof(MeterProvider<int>).Field<Func<IFabricMeterProvider2>>().Set(() => fabricMeterProvider);
            sut = new Mock<MeterProvider<int>>(serviceContext).Object;
        }

        public virtual void Dispose() => typeof(MeterProvider<int>).Field<Func<IFabricMeterProvider2>>().Set(NativeTelemetry.FabricCreateMeterProvider);

        public class Constructor : MeterProviderTest
        {

            [Fact]
            public void SetsRequiredDimensionsFromServiceContext()
            {
                var actualSystemDimensionNames = sut.Private().Field<IReadOnlyCollection<string>>().Value;
                var actualSystemDimensionValues = sut.Protected().Field<IReadOnlyCollection<string>>().Value;

                Assert.Equal(serviceContext.PartitionId.ToString(), actualSystemDimensionValues.ElementAt(0));
                Assert.Equal(serviceContext.ServiceTypeName, actualSystemDimensionValues.ElementAt(1));
                Assert.Equal(serviceContext.ServiceName.ToString(), actualSystemDimensionValues.ElementAt(2));
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationName, actualSystemDimensionValues.ElementAt(3));
                Assert.Equal(serviceContext.CodePackageActivationContext.ApplicationTypeName, actualSystemDimensionValues.ElementAt(4));

                Assert.Equal(nameof(ServiceContext.PartitionId), actualSystemDimensionNames.ElementAt(0));
                Assert.Equal(nameof(ServiceContext.ServiceTypeName), actualSystemDimensionNames.ElementAt(1));
                Assert.Equal(nameof(ServiceContext.ServiceName), actualSystemDimensionNames.ElementAt(2));
                Assert.Equal(nameof(ServiceContext.CodePackageActivationContext.ApplicationName), actualSystemDimensionNames.ElementAt(3));
                Assert.Equal(nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName), actualSystemDimensionNames.ElementAt(4));
            }

            [Fact]
            public void SetsEmptyDimensionsIfNullServiceContext()
            {
                var sut = new Mock<MeterProvider<int>>(null).Object;

                var actualSystemDimensionNames = sut.Private().Field<IReadOnlyCollection<string>>().Value;
                var actualSystemDimensionValues = sut.Protected().Field<IReadOnlyCollection<string>>().Value;

                Assert.Empty(actualSystemDimensionValues);
                Assert.Empty(actualSystemDimensionNames);
            }
        }

        public class DisposeTest : MeterProviderTest
        {
            readonly Func<object, int> finalReleaseComObject = Mock.Of<Func<object, int>>();

            // Set the finalReleaseComObject delegate to a mock function, so we can verify that it was called in Dispose.
            public DisposeTest() =>
                typeof(MeterProvider<int>).Field<Func<object, int>>().Set(finalReleaseComObject);

            public override void Dispose()
            {
                base.Dispose();

                // Restore the original finalReleaseComObject delegate after the test, to avoid affecting other tests.
                typeof(MeterProvider<int>).Field<Func<object, int>>().Set(Utility.FinalReleaseComObject);
            }

            [Fact]
            public void ReleasesNativeFabricMeterProvider()
            {
                sut.Dispose();

                Mock.Get(finalReleaseComObject).Verify(f => f(fabricMeterProvider), Times.Once);
                Assert.Null(sut.Private().Field<IFabricMeterProvider2>().Value);
            }

            [Fact]
            public void SubsequentDisposesDoNotReleaseNativeFabricMeterProvider()
            {
                sut.Dispose();
                sut.Dispose();
                Mock.Get(finalReleaseComObject).Verify(f => f(fabricMeterProvider), Times.Once);
            }
        }

        public class CreateMeterProvider : DisposeTest
        {
            readonly Func<string, string, IEnumerable<string>, IFabricMeter> createMeterProvider;

            readonly string metricNamespace = fuzzy.String();
            readonly string metricName = fuzzy.String();
            readonly IEnumerable<string> dimensions = fuzzy.List(() => fuzzy.String());

            public CreateMeterProvider() =>
                createMeterProvider = sut.Protected().Method<Func<string, string, IEnumerable<string>, IFabricMeter>>();

            [Fact]
            public void ThrowsIfDisposed()
            {
                sut.Dispose();

                _ = Assert.Throws<ObjectDisposedException>(() => createMeterProvider.Invoke(metricNamespace, metricName, dimensions));
            }
        }
    }
}
