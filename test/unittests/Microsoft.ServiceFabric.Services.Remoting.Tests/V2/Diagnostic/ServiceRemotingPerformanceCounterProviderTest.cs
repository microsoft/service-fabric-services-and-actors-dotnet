// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using FluentAssertions;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class ServiceRemotingPerformanceCounterProviderTest
    {
        private readonly Guid testPartitionId = Guid.NewGuid();
        private readonly long testReplicaOrInstanceId = 12345L;

        public class RegisterWithDiagnosticsEventManager : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionWhenDiagnosticsEventManagerIsNull()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);

                Action act = () => provider.RegisterWithDiagnosticsEventManager(null);

                act.Should().Throw<ArgumentNullException>()
                    .WithParameterName("diagnosticsEventManager");
            }

            [Fact]
            public void ShouldRegisterCorrectEventHandlerMethods()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var diagnosticsEventManager = new DiagnosticsEventManager();

                provider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager);

                // Verify that the registered handlers are the correct methods
                diagnosticsEventManager.OnRequestStart.Method.Name.Should().Be("OnRequestStart");
                diagnosticsEventManager.OnRequestEnd.Method.Name.Should().Be("OnRequestEnd");
                diagnosticsEventManager.OnCreateTransportMessage.Method.Name.Should().Be("OnCreateTransportMessage");
                diagnosticsEventManager.OnCreateRemotingMessage.Method.Name.Should().Be("OnCreateRemotingMessage");
            }
        }

        public class OnRequestStart : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldNotThrowWhenCounterWriterIsNull()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                
                // Ensure counter writer is null by setting it
                SetPrivateProperty(provider, "ServiceOutstandingRequestsCounterWriter", null);

                Action act = () => provider.OnRequestStart();

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldExecuteWithoutExceptionWhenCalled()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);

                Action act = () => provider.OnRequestStart();

                act.Should().NotThrow();
            }
        }

        public class OnRequestEnd : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldNotThrowWhenCounterWritersAreNull()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();
                
                SetPrivateProperty(provider, "ServiceOutstandingRequestsCounterWriter", null);
                SetPrivateProperty(provider, "ServiceRequestProcessingTimeCounterWriter", null);

                Action act = () => provider.OnRequestEnd(stopwatch);

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldExecuteWithoutExceptionWhenCalledWithValidStopwatch()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                Action act = () => provider.OnRequestEnd(stopwatch);

                act.Should().NotThrow();
            }
        }

        public class OnCreateTransportMessage : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldNotThrowWhenCounterWriterIsNull()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();
                
                SetPrivateProperty(provider, "ServiceResponseSerializationTimeCounterWriter", null);

                Action act = () => provider.OnCreateTransportMessage(stopwatch);

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldExecuteWithoutExceptionWhenCalledWithValidStopwatch()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                Action act = () => provider.OnCreateTransportMessage(stopwatch);

                act.Should().NotThrow();
            }
        }

        public class OnCreateRemotingMessage : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldNotThrowWhenCounterWriterIsNull()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();
                
                SetPrivateProperty(provider, "ServiceRequestDeserializationTimeCounterWriter", null);

                Action act = () => provider.OnCreateRemotingMessage(stopwatch);

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldExecuteWithoutExceptionWhenCalledWithValidStopwatch()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                Action act = () => provider.OnCreateRemotingMessage(stopwatch);

                act.Should().NotThrow();
            }
        }

        public class IntegrationTests : ServiceRemotingPerformanceCounterProviderTest
        {
            [Fact]
            public void ShouldWorkEndToEndWithDiagnosticsEventManager()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var diagnosticsEventManager = new DiagnosticsEventManager();

                provider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager);

                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                // Simulate the workflow - these should all execute without throwing exceptions
                Action act = () =>
                {
                    diagnosticsEventManager.OnRequestStart?.Invoke();
                    diagnosticsEventManager.OnCreateRemotingMessage?.Invoke(stopwatch);
                    diagnosticsEventManager.OnCreateTransportMessage?.Invoke(stopwatch);
                    diagnosticsEventManager.OnRequestEnd?.Invoke(stopwatch);
                };

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldHandleMultipleRegistrationsGracefully()
            {
                var provider = new ServiceRemotingPerformanceCounterProvider(testPartitionId, testReplicaOrInstanceId);
                var diagnosticsEventManager1 = new DiagnosticsEventManager();
                var diagnosticsEventManager2 = new DiagnosticsEventManager();

                Action act = () =>
                {
                    provider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager1);
                    provider.RegisterWithDiagnosticsEventManager(diagnosticsEventManager2);
                };

                act.Should().NotThrow();
            }
        }

        private static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }
    }
}
