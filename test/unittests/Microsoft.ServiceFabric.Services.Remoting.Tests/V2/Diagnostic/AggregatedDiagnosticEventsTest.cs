// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;


namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{    
    public class AggregatedDiagnosticEventsTest
    {
        internal interface ITestDiagnosticsEvents : IDiagnosticEvents { }

        readonly IDiagnosticEvents diagnosticEvent = Mock.Of<IDiagnosticEvents>();
        readonly IDiagnosticEvents anotherDiagnosticEvents = Mock.Of<ITestDiagnosticsEvents>();

        readonly IEnumerable<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>
        {
            Mock.Of<IDiagnosticEvents>()
        };

        private AggregatedDiagnosticEvents sut;

        protected AggregatedDiagnosticEventsTest() => sut = new AggregatedDiagnosticEvents(diagnosticEvents);

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
                Assert.Throws<ArgumentException>(() => new AggregatedDiagnosticEvents(new List<IDiagnosticEvents> { diagnosticEvent, null }));
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>());

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Empty(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                Assert.NotNull(this.sut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Single(this.sut.Field<IEnumerable<IDiagnosticEvents>>().Value);
                Assert.IsAssignableFrom<IDiagnosticEvents>(this.sut.Field<IEnumerable<IDiagnosticEvents>>().Value.First());
            }

            [Fact]
            public void AssignsMultipleEvent()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                Assert.NotNull(newSut.Field<IEnumerable<IDiagnosticEvents>>());
                Assert.Equal(2, newSut.Field<IEnumerable<IDiagnosticEvents>>().Value.Count());
                Assert.IsAssignableFrom<IDiagnosticEvents>(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value.First());
                Assert.IsAssignableFrom<ITestDiagnosticsEvents>(newSut.Field<IEnumerable<IDiagnosticEvents>>().Value.Last());
            }
        }

        public class OnEvents : AggregatedDiagnosticEventsTest
        {

            [Fact]
            public void RemotingRequestBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                newSut.OnRemotingRequestBegin();

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
            }

            [Fact]
            public void RemotingRequestEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnRemotingRequestEnd(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnRemotingRequestEnd(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnRemotingRequestEnd(startTime), Times.Once);
            }

            [Fact]
            public void RequestResponseBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                newSut.OnRequestResponseBegin();

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnRequestResponseBegin(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnRequestResponseBegin(), Times.Once);
            }

            [Fact]
            public void RequestResponseEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnRequestResponseEnd(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnRequestResponseEnd(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnRequestResponseEnd(startTime), Times.Once);
            }

            [Fact]
            public void CreateTransportMessageBeginInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                newSut.OnCreateTransportMessageBegin();

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnCreateTransportMessageBegin(), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageBegin(), Times.Once);
            }

            [Fact]
            public void CreateTransportMessageEndInvokesAllDiagnostics()
            {
                var newSut = new AggregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    diagnosticEvent,
                    anotherDiagnosticEvents
                });

                var startTime = DateTime.UtcNow;
                newSut.OnCreateTransportMessageEnd(startTime);

                Mock.Get(diagnosticEvent).Verify(ds => ds.OnCreateTransportMessageEnd(startTime), Times.Once);
                Mock.Get(anotherDiagnosticEvents).Verify(ds => ds.OnCreateTransportMessageEnd(startTime), Times.Once);
            }
        }

    }
}
