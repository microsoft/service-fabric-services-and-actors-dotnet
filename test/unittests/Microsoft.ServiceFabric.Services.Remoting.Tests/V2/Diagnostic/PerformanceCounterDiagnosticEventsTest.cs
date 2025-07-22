// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        private static readonly IServiceRemotingPerformanceCounterWriterProvider mockPerformanceCounterProvider = Mock.Of<IServiceRemotingPerformanceCounterWriterProvider>();
        internal PerformanceCounterDiagnosticEvents sut = new PerformanceCounterDiagnosticEvents(mockPerformanceCounterProvider);

        public class Class : PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticsEvents()
            {
                var performanceCounterDiagnosticsSourceType = typeof(PerformanceCounterDiagnosticEvents);
                var iDiagnosticsSourceType = typeof(IDiagnosticEvents);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(performanceCounterDiagnosticsSourceType));
            }
        }

        public class Constructor: PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersPresent()
            {
                var sutType = typeof(PerformanceCounterDiagnosticEvents);
                var expectedParameterTypes = new[] { typeof(IServiceRemotingPerformanceCounterWriterProvider) };
                var constructor = sutType.GetConstructor(expectedParameterTypes);
                Assert.NotNull(constructor);
            }

            [Fact]
            public void WithParametersSetsValue()
            {
                var serviceRequestProcessingTimeCounterWriter = sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestProcessingTimeCounterWriter").Value;
                Assert.NotNull(serviceRequestProcessingTimeCounterWriter);
                Assert.IsAssignableFrom<AbstractFabricCounterWriterWrapper>(serviceRequestProcessingTimeCounterWriter);

                var serviceRequestDeserializationTimeCounterWriter = sut.Field<AbstractFabricCounterWriterWrapper>("serviceRequestDeserializationTimeCounterWriter").Value;
                Assert.NotNull(serviceRequestDeserializationTimeCounterWriter);
                Assert.IsAssignableFrom<AbstractFabricCounterWriterWrapper>(serviceRequestDeserializationTimeCounterWriter);

                var serviceResponseSerializationTimeCounterWriter = sut.Field<AbstractFabricCounterWriterWrapper>("serviceResponseSerializationTimeCounterWriter").Value;
                Assert.NotNull(serviceResponseSerializationTimeCounterWriter);
                Assert.IsAssignableFrom<AbstractFabricCounterWriterWrapper>(serviceResponseSerializationTimeCounterWriter);

                var serviceOutstandingRequestsCounterWriter = sut.Field<AbstractFabricCounterWriterWrapper>("serviceOutstandingRequestsCounterWriter").Value;
                Assert.NotNull(serviceOutstandingRequestsCounterWriter);
                Assert.IsAssignableFrom<AbstractFabricCounterWriterWrapper>(serviceOutstandingRequestsCounterWriter);
            }

            [Fact]
            public void ThrowsOnNullProvider()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new PerformanceCounterDiagnosticEvents(null);
                });
            }
        }

        public class OnEvent: PerformanceCounterDiagnosticEventsTest
        {

        }
    }
}
