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

        readonly string testNodeName = fuzzy.String();
        readonly string testServiceTypeName = fuzzy.String();
        readonly Uri testServiceNameUri = fuzzy.Uri();
        readonly string testApplicationName = fuzzy.String();
        readonly string testApplicationTypeName = fuzzy.String();
        readonly long replicaId = fuzzy.Int64();
        readonly Guid testPartitionId = Guid.NewGuid();

        readonly ServiceContext serviceContext;

        public ServiceMeterProviderTest()
        {
            var codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationName).Returns(testApplicationName);
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationTypeName).Returns(testApplicationTypeName);
            this.serviceContext = new Mock<ServiceContext>(fuzzy.NodeContext(), codePackageActivationContext, testServiceTypeName, testServiceNameUri, fuzzy.Array(fuzzy.Byte), testPartitionId, replicaId).Object;
        }

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
            public void ShouldRecordRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(serviceContext);

                var actualValues = sut.Protected().Field<IList<string>>().Value;

                Assert.Equal(replicaId.ToString(), actualValues[0]);
                Assert.Equal(testPartitionId.ToString(), actualValues[1]);
                Assert.Equal(testServiceTypeName, actualValues[2]);
                Assert.Equal(testServiceNameUri.ToString(), actualValues[3]);
                Assert.Equal(testApplicationName, actualValues[4]);
                Assert.Equal(testApplicationTypeName, actualValues[5]);
            }

            [Fact]
            public void ShouldNotRecordRequiredDimensionsFromNullServiceContext()
            {
                Assert.Throws<ArgumentNullException>(() => new TestMeterProvider<int>(null));
            }
        }

        public class Class : ServiceMeterProviderTest
        {
            [Fact]
            public void ShouldHaveIFabricMeterNativeInterop()
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
