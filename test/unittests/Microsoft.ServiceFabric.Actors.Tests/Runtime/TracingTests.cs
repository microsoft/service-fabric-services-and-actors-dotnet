// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime
{
    using System.Diagnostics.Tracing;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Diagnostics.Tracing.Session;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Xunit;

    /// <summary>
    /// Tests for ActorFrameworkEventSource
    /// </summary>
    public class TracingTests
    {
        private const string TestActorType = "actortype";
        private const long TestCountOfWaitingMethodCalls = 5;
        private const string TestMethodName = "FooMethod";
        private const string TestMethodSignature = "(int foo, string bar) : string";
        private const long TestMethodExecutionTimeTicks = 123;
        private const string TestException = "Something failed";
        private const long TestSaveStateExecutionTimeTicks = 321;
        private const string TestCustomActorServiceType = "CustomActor";
        private const string TestNodeName = "NodeA";
        private const string SpecialExceptionText = "EXCEPTION_DURING_VALUE_LOOKUP";
        private static readonly ActorId TestActorId = ActorId.CreateRandom();
        private static readonly ServiceContext TestServiceContext = TestMocksRepository.GetMockStatefulServiceContext();

        /// <summary>
        /// Tests that all actor events can be read from a trace session. Check event source event definitions on failure.
        /// </summary>
        [Fact]
        public void EventsCanBeReadFromSession()
        {
            const int TotalEventCount = 14;
            using (var session = new TraceEventSession("TracingTests"))
            {
                var eventsFired = 0;
                session.Source.Dynamic.All += data =>
                {
                    if ((long)data.ID == 65534)
                    {
                        return;
                    }

                    data.GetFormattedMessage(CultureInfo.InvariantCulture).Should().NotContain(
                        SpecialExceptionText,
                        "all Message fields should be formatted");

                    eventsFired++;
                    if (eventsFired == TotalEventCount)
                    {
                        // Wait until all events are processed to exit.
                        session.Source.Dispose();
                    }
                };

                session.EnableProvider(
                    EventSource.GetName(typeof(ActorFrameworkEventSource)),
                    matchAnyKeywords: 0);

                Thread.Sleep(60000);

                var writer = ActorFrameworkEventSource.Writer;
                writer.ActorActivated(TestActorType, TestActorId, TestServiceContext);
                writer.ActorDeactivated(TestActorType, TestActorId, TestServiceContext);
                writer.ActorMethodCallsWaitingForLock(TestCountOfWaitingMethodCalls, TestActorType, TestActorId, TestServiceContext);
                writer.ActorMethodStart(TestMethodName, TestMethodSignature, TestActorType, TestActorId, TestServiceContext);
                writer.ActorMethodStop(TestMethodExecutionTimeTicks, TestMethodName, TestMethodSignature, TestActorType, TestActorId, TestServiceContext);
                writer.ActorMethodThrewException(
                    TestException,
                    TestMethodExecutionTimeTicks,
                    TestMethodName,
                    TestMethodSignature,
                    TestActorType,
                    TestActorId,
                    TestServiceContext);
                writer.ActorSaveStateStart(TestActorType, TestActorId, TestServiceContext);
                writer.ActorSaveStateStop(TestSaveStateExecutionTimeTicks, TestActorType, TestActorId, TestServiceContext);
                writer.ActorTypeRegistered(TestActorType, TestCustomActorServiceType, TestNodeName);
                writer.ActorTypeRegistrationFailed(TestException, TestActorType, TestCustomActorServiceType, TestNodeName);
                writer.ReplicaChangeRoleFromPrimary(TestServiceContext);
                writer.ReplicaChangeRoleToPrimary(TestServiceContext);
                writer.ServiceInstanceClose(TestServiceContext);
                writer.ServiceInstanceOpen(TestServiceContext);
                session.Source.Process();
            }
        }
    }
}
