// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class Int64MeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly string testNodeName = fuzzy.String();
        readonly string testServiceTypeName = fuzzy.String();
        readonly Uri testServiceNameUri = fuzzy.Uri();
        readonly string testApplicationName = fuzzy.String();
        readonly string testApplicationTypeName = fuzzy.String();
        readonly long replicaId = fuzzy.Int64();
        readonly Guid testPartitionId = Guid.NewGuid();

        readonly ServiceContext serviceContext;

        readonly IFabricMeterProvider fabricMeterProvider = Mock.Of<IFabricMeterProvider>();

        public Int64MeterProviderTest()
        {
            var codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationName).Returns(testApplicationName);
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationTypeName).Returns(testApplicationTypeName);
            this.serviceContext = new Mock<ServiceContext>(fuzzy.NodeContext(), codePackageActivationContext, testServiceTypeName, testServiceNameUri, fuzzy.Array(fuzzy.Byte), testPartitionId, replicaId).Object;

            Mock.Get(fabricMeterProvider)
                .Setup(x => x.CreateMeter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<uint>()))
                .Returns(Mock.Of<IFabricMeter>());
            typeof(ServiceMeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => fabricMeterProvider);
        }

        public class CreateMeter : Int64MeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();
            readonly Int64MeterProvider sut;
            readonly IList<string> systemDimensionsValues;

            public CreateMeter()
            {
                sut = new Int64MeterProvider(serviceContext);
                systemDimensionsValues = sut.Protected().Field<IList<string>>().Value;
            }

            [Fact]
            public void ShouldCreateMeterWithCorrectSystemDimensions()
            {
                var meter = sut.CreateMeter(testNamespace, testMetric);

                Assert.NotNull(meter);
                Assert.IsType<Int64Meter>(meter);

                Assert.Equal(systemDimensionsValues, ((Int64Meter)meter).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter1DWithCorrectSystemDimensions()
            {
                var meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Assert.NotNull(meter1D);
                Assert.IsType<Int64Meter1D>(meter1D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter1D)meter1D).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter2DWithCorrectSystemDimensions()
            {
                var meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Assert.NotNull(meter2D);
                Assert.IsType<Int64Meter2D>(meter2D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter2D)meter2D).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter3DWithCorrectSystemDimensions()
            {
                var meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);
                Assert.NotNull(meter3D);
                Assert.IsType<Int64Meter3D>(meter3D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter3D)meter3D).Field<IEnumerable<string>>().Value);
            }
        }
    }
}
