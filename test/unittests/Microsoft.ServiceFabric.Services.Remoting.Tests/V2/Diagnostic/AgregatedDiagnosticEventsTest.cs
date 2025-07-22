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
    public class AgregatedDiagnosticEventsTest
    {
        internal interface ITestDiagnosticsEvents : IDiagnosticEvents { }

        private IDiagnosticEvents mockedDiagnosticEvents = Mock.Of<IDiagnosticEvents>();
        private IDiagnosticEvents mockedAnotherDiagnosticEvents = Mock.Of<ITestDiagnosticsEvents>();

        private AgregatedDiagnosticEvents sut;
        IClock mockClock = Mock.Of<IClock>();
        IEnumerable<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>
        {
            Mock.Of<IDiagnosticEvents>()
        };

        public AgregatedDiagnosticEventsTest()
        {
            this.sut = new AgregatedDiagnosticEvents(diagnosticEvents);
        }

        public class Class : AgregatedDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticEvents()
            {
                var sutType = typeof(AgregatedDiagnosticEvents);
                var expectedType = typeof(IDiagnosticEvents);

                Assert.True(expectedType.IsAssignableFrom(sutType));
            }
        }

        public class Constructor : AgregatedDiagnosticEventsTest
        {
            [Fact]
            public void WithParametersPresent()
            {
                var sutType = typeof(AgregatedDiagnosticEvents);
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
                Assert.Throws<ArgumentException>(() => new AgregatedDiagnosticEvents(null));
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                Assert.Throws<ArgumentException>(() => new AgregatedDiagnosticEvents(new List<IDiagnosticEvents> { mockedDiagnosticEvents, null }));
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>());

                Assert.NotNull(newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet"));
                Assert.Empty(newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value);
            }

            [Fact]
            public void AssignsSingleEvent()
            {
                Assert.NotNull(this.sut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet"));
                Assert.Single(this.sut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value);
                Assert.IsAssignableFrom<IDiagnosticEvents>(this.sut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value.First());
            }

            [Fact]
            public void AssignsMultipleEvent()
            {
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedAnotherDiagnosticEvents
                });

                Assert.NotNull(newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet"));
                Assert.Equal(2, newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value.Count);
                Assert.IsAssignableFrom<IDiagnosticEvents>(newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value.First());
                Assert.IsAssignableFrom<ITestDiagnosticsEvents>(newSut.Field<HashSet<IDiagnosticEvents>>("diagnosticsEventSet").Value.Last());
            }

            [Fact]
            public void AssignFailsWithMultipleSameDiagnosticEvents()
            {
                Assert.Throws<ArgumentException>(() => new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedDiagnosticEvents
                }));
            }
        }

        public class OnEvents : AgregatedDiagnosticEventsTest
        {

            [Fact]
            public void RemotingRequestBeginInvokesAllDiagnostics()
            {
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
                var newSut = new AgregatedDiagnosticEvents(new List<IDiagnosticEvents>
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
