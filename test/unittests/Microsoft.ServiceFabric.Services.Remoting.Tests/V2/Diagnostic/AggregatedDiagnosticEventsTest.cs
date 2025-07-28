// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Util;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;


namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{    
    public class AggregatedDiagnosticEventsTest
    {
        internal interface ITestDiagnosticsEvents : IDiagnosticEvents { }

        readonly static IDiagnosticEvents mockedDiagnosticEvents = Mock.Of<IDiagnosticEvents>();
        readonly static IDiagnosticEvents mockedAnotherDiagnosticEvents = Mock.Of<ITestDiagnosticsEvents>();

        readonly static IClock mockClock = Mock.Of<IClock>();
        readonly static IEnumerable<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>
        {
            Mock.Of<IDiagnosticEvents>()
        };

        private AggregatedDiagnosticEvents sut = new AggregatedDiagnosticEvents(diagnosticEvents);

        public class Class : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticEvents()
            {
                var sutType = typeof(AggregatedDiagnosticEvents);
                var expectedType = typeof(IDiagnosticEvents);

                Assert.True(expectedType.IsAssignableFrom(sutType));
            }
        }

        public class Constructor : AggregatedDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersPresent()
            {
                var sutType = typeof(AggregatedDiagnosticEvents);
                var expectedParameterTypes = new[] { typeof(IEnumerable<IDiagnosticEvents>) };

                var constructor = sutType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                Assert.Single(constructor.GetParameters());
                Assert.Equal(typeof(IEnumerable<IDiagnosticEvents>), constructor.GetParameters()[0].ParameterType);
            }

            [Fact]
            public void ThrowsOnNullEventsList()
            {
                Assert.Throws<ArgumentException>(() => new AggregatedDiagnosticEvents(null));
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                Assert.Throws<ArgumentException>(() => new AggregatedDiagnosticEvents(new List<IDiagnosticEvents> { mockedDiagnosticEvents, null }));
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>());

                Assert.NotNull(newSut.Field<HashSet<IDiagnosticEvents>>());
                Assert.Empty(newSut.Field<HashSet<IDiagnosticEvents>>().Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                Assert.NotNull(this.sut.Field<HashSet<IDiagnosticEvents>>());
                Assert.Single(this.sut.Field<HashSet<IDiagnosticEvents>>().Value);
                Assert.IsAssignableFrom<IDiagnosticEvents>(this.sut.Field<HashSet<IDiagnosticEvents>>().Value.First());
            }

            [Fact]
            public void AssignsMultipleEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                Assert.NotNull(newSut.Field<HashSet<IDiagnosticEvents>>());
                Assert.Equal(2, newSut.Field<HashSet<IDiagnosticEvents>>().Value.Count);
                Assert.IsAssignableFrom<IDiagnosticEvents>(newSut.Field<HashSet<IDiagnosticEvents>>().Value.First());
                Assert.IsAssignableFrom<ITestDiagnosticsEvents>(newSut.Field<HashSet<IDiagnosticEvents>>().Value.Last());
            }

            [Fact]
            public void AssignFailsWithMultipleSameDiagnosticEvents()
            {
                Assert.Throws<ArgumentException>(() => new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedDiagnosticEvents
                }));
            }
        }

        public class OnEvents : AggregatedDiagnosticEventsTest
        {

            [Fact]
            public void RemotingRequestBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                newSut.OnRemotingRequestBegin();

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
            }

            [Fact]
            public void RemotingRequestEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnRemotingRequestEnd(startTime);

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnRemotingRequestEnd(startTime), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnRemotingRequestEnd(startTime), Times.Once);
            }

            [Fact]
            public void RequestResponseBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                newSut.OnRequestResponseBegin();

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnRequestResponseBegin(), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnRequestResponseBegin(), Times.Once);
            }

            [Fact]
            public void RequestResponseEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnRequestResponseEnd(startTime);

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnRequestResponseEnd(startTime), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnRequestResponseEnd(startTime), Times.Once);
            }

            [Fact]
            public void CreateTransportMessageBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                newSut.OnCreateTransportMessageBegin();

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageBegin(), Times.Once);
            }

            [Fact]
            public void CreateTransportMessageEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnCreateTransportMessageEnd(startTime);

                Mock.Get(mockedDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageEnd(startTime), Times.Once);
                Mock.Get(mockedAnotherDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageEnd(startTime), Times.Once);
            }
        }

    }
}
