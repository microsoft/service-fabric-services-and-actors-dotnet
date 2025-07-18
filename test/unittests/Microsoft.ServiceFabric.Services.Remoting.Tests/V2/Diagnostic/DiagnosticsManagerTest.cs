// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{    
    public class DiagnosticsManagerTest
    {
        internal interface ITestDiagnosticsSource : IDiagnosticsSource { }

        private IDiagnosticsSource mockedFirstSource = Mock.Of<IDiagnosticsSource>();
        private IDiagnosticsSource mockedSecondSource = Mock.Of<ITestDiagnosticsSource>();

        private DiagnosticsManager sut;
        ITimeProvider mockTimeProvider = Mock.Of<ITimeProvider>();
        Guid partitionId = Guid.NewGuid();
        long replicaOrInstanceId = 123L;

        public DiagnosticsManagerTest()
        {
            this.sut = new DiagnosticsManager(mockTimeProvider, partitionId, replicaOrInstanceId);
        }

        public class Class : DiagnosticsManagerTest
        {
            [Fact]
            public void HasDiagnosticsSource()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var iDiagnosticsSourceType = typeof(IDiagnosticsSource);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(diagnosticsManagerType), "DiagnosticsManager should implement IDiagnosticsSource interface");
            }
        }

        public class Constructor : DiagnosticsManagerTest
        {
            [Fact]
            public void WithParametersPresent()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var expectedParameterTypes = new[] { typeof(ITimeProvider), typeof(Guid), typeof(long) };

                var constructor = diagnosticsManagerType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                Assert.Equal(3, constructor.GetParameters().Length);
                Assert.Equal(typeof(ITimeProvider), constructor.GetParameters()[0].ParameterType);
                Assert.Equal(typeof(Guid), constructor.GetParameters()[1].ParameterType);
                Assert.Equal(typeof(long), constructor.GetParameters()[2].ParameterType);
            }

            [Fact]
            public void AssignsParametersToFields()
            {
                var partitionField = sut.Field<Guid>("partitionId");
                Assert.NotNull(partitionField);
                Assert.Equal(partitionId, partitionField.Value);

                var timeProviderFiled = sut.Field<ITimeProvider>("timeProvider");
                Assert.NotNull(timeProviderFiled);
                Assert.Equal(mockTimeProvider, timeProviderFiled.Value);

                var replaicaIdField = sut.Field<long>("replicaOrInstanceId");
                Assert.NotNull(replaicaIdField);
                Assert.Equal(replicaOrInstanceId, replaicaIdField.Value);
            }
        }

        public class OnRemotingRequestBegin : DiagnosticsManagerTest
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
