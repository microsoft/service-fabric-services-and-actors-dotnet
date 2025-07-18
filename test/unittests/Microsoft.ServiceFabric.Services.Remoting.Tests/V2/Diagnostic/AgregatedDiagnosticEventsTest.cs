// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
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
        ITimeProvider mockTimeProvider = Mock.Of<ITimeProvider>();
        IEnumerable<IDiagnosticEvents> diagnosticEvents = new List<IDiagnosticEvents>
        {
            Mock.Of<IDiagnosticEvents>()
        };

        public AgregatedDiagnosticEventsTest()
        {
            this.sut = new AgregatedDiagnosticEvents(mockTimeProvider);
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
                var expectedParameterTypes = new[] { typeof(ITimeProvider) };

                var constructor = sutType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                //Assert.Equal(1, constructor.GetParameters().Length);
                Assert.Equal(typeof(ITimeProvider), constructor.GetParameters()[0].ParameterType);
            }

            [Fact]
            public void AssignsClockField()
            {
                var timeProviderFiled = sut.Field<ITimeProvider>("timeProvider");
                Assert.NotNull(timeProviderFiled);
                Assert.Equal(mockTimeProvider, timeProviderFiled.Value);
            }
        }

        public class OnRemotingRequestBegin : AgregatedDiagnosticEventsTest
        {

            //private DateTime currentTime = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);

            //public OnRemotingRequestBegin() 
            //{ 
                //Mock.Get(mockTimeProvider).Setup(tp => tp.UtcNow).Returns(currentTime);
            //}

            //[Fact]
            //public void InvokesAllRegisteredSources()
            //{
                //sut.RegisterDiagnosticsSource(mockedFirstSource);
                //sut.RegisterDiagnosticsSource(mockedSecondSource);

                //sut.OnRemotingRequestBegin();

                //Mock.Get(mockedFirstSource).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
                //Mock.Get(mockedSecondSource).Verify(ds => ds.OnRemotingRequestBegin(), Times.Once);
            //}

            //[Fact]
            //public void ReturnsCurrentTime()
            //{
                //var result = sut.OnRemotingRequestBegin();

                //Mock.Get(mockTimeProvider).Verify(tp => tp.UtcNow, Times.Once);
                //Assert.Equal(currentTime, result);
            //}
        }

    }
}
