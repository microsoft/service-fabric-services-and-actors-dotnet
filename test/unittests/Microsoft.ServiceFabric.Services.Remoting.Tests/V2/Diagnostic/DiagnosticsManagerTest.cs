// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class DiagnosticsManagerTest
    {
        private readonly Guid testPartitionId = Guid.NewGuid();
        private readonly long testReplicaOrInstanceId = 12345L;

        public class Constructor : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldCreateInstanceWithValidParameters()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                diagnosticsManager.Should().NotBeNull();
            }

            [Fact]
            public void ShouldInitializeDiagnosticsEventManager()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                
                diagnosticsEventManager.Should().NotBeNull();
            }

            [Fact]
            public void ShouldInitializePerformanceCounterProvider()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                var perfCounterProviderField = typeof(DiagnosticsManager)
                    .GetField("perfCounterProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var perfCounterProvider = perfCounterProviderField?.GetValue(diagnosticsManager);
                
                perfCounterProvider.Should().NotBeNull();
                perfCounterProvider.Should().BeOfType<ServiceRemotingPerformanceCounterProvider>();
            }
        }

        public class FabricTransportRequestBegin : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldCallDiagnosticsEventManagerOnRequestStart()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnRequestStart = () => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportRequestBegin();

                callbackInvoked.Should().BeTrue();
            }

            [Fact]
            public void ShouldHandleNullCallback()
            {
                // Arrange
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                // Access the internal diagnosticsEventManager
                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);

                diagnosticsEventManager.OnRequestStart = null;

                Action act = () => diagnosticsManager.FabricTransportRequestBegin();
                act.Should().NotThrow();
            }
        }

        public class FabricTransportRequestEnd : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldCallDiagnosticsEventManagerOnRequestEnd()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnRequestEnd = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportRequestEnd(Stopwatch.StartNew());

                callbackInvoked.Should().BeTrue();
            }

            [Fact]
            public void ShouldHandleNullCallback()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnRequestEnd = null;

                Action act = () => diagnosticsManager.FabricTransportRequestEnd(Stopwatch.StartNew());

                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldHandleNullStopwatch()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnRequestEnd = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportRequestEnd(null);

                callbackInvoked.Should().BeFalse();
            }
        }

        public class FabricTransportCreateTransportMessage : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldCallDiagnosticsEventManagerOnCreateTransportMessage()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateTransportMessage = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportCreateTransportMessage(Stopwatch.StartNew());

                callbackInvoked.Should().BeTrue();
            }

            [Fact]
            public void ShouldHandleNullCallback()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateTransportMessage = null;

                Action act = () => diagnosticsManager.FabricTransportCreateTransportMessage(Stopwatch.StartNew());
                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldHandleNullStopwatch()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateTransportMessage = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportCreateTransportMessage(null);

                callbackInvoked.Should().BeFalse();
            }
        }

        public class FabricTransportCreateRemotingMessage : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldCallDiagnosticsEventManagerOnCreateRemotingMessage()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateRemotingMessage = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportCreateRemotingMessage(Stopwatch.StartNew());

                callbackInvoked.Should().BeTrue();
            }

            [Fact]
            public void ShouldHandleNullCallback()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateRemotingMessage = null;

                Action act = () => diagnosticsManager.FabricTransportCreateRemotingMessage(Stopwatch.StartNew());
                act.Should().NotThrow();
            }

            [Fact]
            public void ShouldHandleNullStopwatch()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);
                var callbackInvoked = false;

                var diagnosticsEventManagerField = typeof(DiagnosticsManager)
                    .GetField("diagnosticsEventManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var diagnosticsEventManager = (DiagnosticsEventManager)diagnosticsEventManagerField?.GetValue(diagnosticsManager);
                diagnosticsEventManager.OnCreateRemotingMessage = (sw) => { callbackInvoked = true; };

                diagnosticsManager.FabricTransportCreateRemotingMessage(null);

                callbackInvoked.Should().BeFalse();
            }
        }

        public class Dispose : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldDisposeWithNoExceptions()
            {
                var diagnosticsManager = new DiagnosticsManager(testPartitionId, testReplicaOrInstanceId);

                Action act = () => ((IDisposable)diagnosticsManager).Dispose();
                act.Should().NotThrow();
            }
        }
    }
}
