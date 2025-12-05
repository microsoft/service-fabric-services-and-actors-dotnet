// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public class DiagnosticsFactoryTest : MockedMetricsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly DiagnosticsFactory sut;

        internal readonly ServiceContext serviceContext = fuzzy.ServiceContext();
        internal readonly ActorTypeInformation typeInformation = ActorTypeInformation.Get(typeof(TestActor));
        internal readonly ActorMethodFriendlyNameBuilder friendlyNameBuilder;

        public DiagnosticsFactoryTest()
        {
            friendlyNameBuilder = new ActorMethodFriendlyNameBuilder(typeInformation);

            sut = new DiagnosticsFactory(serviceContext, typeInformation, friendlyNameBuilder);
        }

        public class Constructor : DiagnosticsFactoryTest
        {
            [Fact]
            public void ThrowsExceptionWhenServiceContextNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new DiagnosticsFactory(null, typeInformation, friendlyNameBuilder));
                Assert.Equal("serviceContext", exception.ParamName);
            }

            [Fact]
            public void ThrowsExceptionWhenTypeInformationNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new DiagnosticsFactory(serviceContext, null, friendlyNameBuilder));
                Assert.Equal("typeInformation", exception.ParamName);
            }

            [Fact]
            public void ThrowsExceptionWhenNameBuilderNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new DiagnosticsFactory(serviceContext, typeInformation, null));
                Assert.Equal("friendlyNameBuilder", exception.ParamName);
            }

            [Fact]
            public void AssignsNeededFields()
            {
                Assert.Equal(serviceContext, sut.Field<ServiceContext>().Value);
                Assert.Equal(typeInformation, sut.Field<ActorTypeInformation>().Value);
                Assert.Equal(friendlyNameBuilder, sut.Field<ActorMethodFriendlyNameBuilder>().Value);
            }

            [Fact]
            public void InstantiatesPerfCounterProvider()
            {
                PerformanceCounterProviderV2 perfCounterProvider = sut.Field<PerformanceCounterProviderV2>().Value;

                Assert.Equal(serviceContext.PartitionId, perfCounterProvider.Field<Guid>().Value);
                Assert.Equal(typeInformation, perfCounterProvider.Field<ActorTypeInformation>().Value);
            }

            [Fact]
            public void InstantiatesMeterProviders()
            {
                Assert.NotNull(sut.Field<IMeterProvider<TimeSpan>>().Value);
                Assert.NotNull(sut.Field<IMeterProvider<long>>().Value);
            }
        }

        public class DisposeTest : DiagnosticsFactoryTest
        {
            readonly Guid guid = Guid.NewGuid();
            readonly Mock<PerformanceCounterProviderV2> mockPerformanceCounterProviderV2;

            public DisposeTest()
            {
                mockPerformanceCounterProviderV2 = new Mock<PerformanceCounterProviderV2>(guid, typeInformation);
                sut.Field<PerformanceCounterProviderV2>().Set(mockPerformanceCounterProviderV2.Object);
            }

            [Fact]
            public void DisposesPerformanceCounterProvider()
            {
                sut.Dispose();

                mockPerformanceCounterProviderV2.Verify(p => p.Dispose(), Times.Once);
            }
        }

        public class CreateDiagnostics : DiagnosticsFactoryTest
        {
            readonly IClock clock = Mock.Of<IClock>();
            readonly IDiagnostics diagnostics;

            public CreateDiagnostics() => diagnostics = sut.CreateDiagnostics(clock);

            [Fact]
            public void CreatesAggregatedDiagnostics()
            {
                Assert.IsAssignableFrom<AggregatedDiagnostics>(diagnostics);
            }

            [Fact]
            public void CreatesTwoCompositeEvents()
            {
                var compositeDiagnotics = diagnostics.Field<IEnumerable<IDiagnostics>>().Value;

                Assert.Equal(3, compositeDiagnotics.Count());
            }

            [Fact]
            public void CreatesPerformanceCounterEventsFirst()
            {
                var perfCounterDiagnotics = diagnostics.Field<IEnumerable<IDiagnostics>>().Value.ToList()[0];

                Assert.IsAssignableFrom<PerformanceCounterDiagnostics>(perfCounterDiagnotics);
                Assert.Equal(sut.Field<PerformanceCounterProviderV2>().Value, perfCounterDiagnotics.Field<PerformanceCounterProviderV2>().Value);
                Assert.Equal(clock, perfCounterDiagnotics.Field<IClock>().Value);
            }

            [Fact]
            public void CreatesEventSourceEventsSecond()
            {
                var eventSourceDiagnostics = diagnostics.Field<IEnumerable<IDiagnostics>>().Value.ToList()[1];

                Assert.IsAssignableFrom<EventSourceDiagnostics>(eventSourceDiagnostics);
                Assert.Equal(ActorFrameworkEventSource.Writer, eventSourceDiagnostics.Field<ActorFrameworkEventSource>().Value);
                Assert.Equal(serviceContext, eventSourceDiagnostics.Field<ServiceContext>().Value);
                Assert.Equal(clock, eventSourceDiagnostics.Field<IClock>().Value);
                Assert.Equal(typeInformation.ImplementationType.ToString(), eventSourceDiagnostics.Field<string>().Value);
            }

            [Fact]
            public void CreatesMetricsEventsThird()
            {
                var metricsDiagnostics = diagnostics.Field<IEnumerable<IDiagnostics>>().Value.ToList()[2];

                Assert.IsAssignableFrom<MetricDiagnostics>(metricsDiagnostics);
                Assert.Equal(clock, metricsDiagnostics.Field<IClock>().Value);
            }
        }
    }
}
