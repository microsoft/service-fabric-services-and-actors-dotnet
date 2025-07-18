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
            this.sut = new AgregatedDiagnosticEvents(mockClock, diagnosticEvents);
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
                var expectedParameterTypes = new[] { typeof(IClock), typeof(IEnumerable<IDiagnosticEvents>) };

                var constructor = sutType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                Assert.Equal(2, constructor.GetParameters().Length);
                Assert.Equal(typeof(IClock), constructor.GetParameters()[0].ParameterType);
                Assert.Equal(typeof(IEnumerable<IDiagnosticEvents>), constructor.GetParameters()[1].ParameterType);
            }

            [Fact]
            public void ThrowsOnNullClock()
            {
                Assert.Throws<ArgumentNullException>(() => new AgregatedDiagnosticEvents(null, new List<IDiagnosticEvents>()));
            }

            [Fact]
            public void AssignsClockField()
            {
                var timeProviderFiled = this.sut.Field<IClock>("timeProvider");
                Assert.NotNull(timeProviderFiled);
                Assert.Equal(mockClock, timeProviderFiled.Value);
            }

            [Fact]
            public void ThrowsOnNullEventsList()
            {
                Assert.Throws<ArgumentNullException>(() => new AgregatedDiagnosticEvents(mockClock, null));
            }

            [Fact]
            public void ThrowsOnAnyNullEvents()
            {
                Assert.Throws<ArgumentException>(() => new AgregatedDiagnosticEvents(mockClock, new List<IDiagnosticEvents> { mockedDiagnosticEvents, null }));
            }

            [Fact]
            public void AssignsEmptyEvent()
            {
                var newSut = new AgregatedDiagnosticEvents(mockClock, new List<IDiagnosticEvents>());

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
                var newSut = new AgregatedDiagnosticEvents(mockClock, new List<IDiagnosticEvents>
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
                Assert.Throws<ArgumentException>(() => new AgregatedDiagnosticEvents(mockClock, new List<IDiagnosticEvents>
                {
                    mockedDiagnosticEvents,
                    mockedDiagnosticEvents
                }));
            }
        }

        public class OnRemotingRequestBegin : AgregatedDiagnosticEventsTest
        {

            //private DateTime currentTime = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);

            //public OnRemotingRequestBegin() 
            //{ 
                //Mock.Get(mockClock).Setup(tp => tp.UtcNow).Returns(currentTime);
            //}

            //[Fact]
            //public void InvokesAllRegisteredSources()
            //{
                //newSut.RegisterDiagnosticsSource(mockedFirstSource);
                //newSut.RegisterDiagnosticsSource(mockedSecondSource);

                //newSut.OnRemotingRequestBegin();

                //Mock.Get(mockedFirstSource).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
                //Mock.Get(mockedSecondSource).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
            //}

            //[Fact]
            //public void ReturnsCurrentTime()
            //{
                //var result = newSut.OnRemotingRequestBegin();

                //Mock.Get(mockClock).Verify(tp => tp.UtcNow, Times.Once);
                //Assert.Equal(currentTime, result);
            //}
        }

    }
}
