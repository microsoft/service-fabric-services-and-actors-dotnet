// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorManagerTest : MockedMetricsTest
    {
        internal const int RemainderCount = 10;
        internal readonly ActorId actorId;
        internal readonly ActorService actorService;

        readonly IClock clock = Mock.Of<IClock>();
        readonly IDiagnostics diagnostics = Mock.Of<IDiagnostics>();
        readonly DateTime startTime = DateTime.Now;

        public ActorManagerTest()
        {
            actorId = ActorId.CreateRandom();
            actorService = TestMocksRepository.GetActorService<TestActor>();
            actorService.Field<IClock>().Set(clock);
            Mock.Get(clock).Setup(clock => clock.UtcNow).Returns(startTime);

            var friendlyNameBuilder = new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation);
            actorService.InitializeInternal(friendlyNameBuilder);
        }

        public class Remainder : ActorManagerTest
        {
            ActorManager actorManager;

            [Fact]
            public async Task VerifyClose()
            {
                ResetActorManager();
                RegisterReminders();
                VerifyReminderPresence();
                await actorManager.CloseAsync(CancellationToken.None);
                VerifyNoReminders();
            }

            [Fact]
            public void VerifyAbort()
            {
                ResetActorManager();
                RegisterReminders();
                VerifyReminderPresence();
                actorManager.Abort();
                VerifyNoRemindersWithRetry();
            }

            [Fact]
            public async Task VerifyFireReminderNoThrow()
            {
                ResetActorManager();
                await actorManager.CloseAsync(CancellationToken.None);

                var reminder = new ActorReminder(
                    ActorId.CreateRandom(),
                    actorManager,
                    "reminderName",
                    null,
                    TimeSpan.FromMinutes(30),
                    TimeSpan.FromMinutes(30));

                await actorManager.FireReminderAsync(reminder);
            }

            [Fact]
            public void VerifyNoReminderEntry()
            {
                ResetActorManager();
                RegisterReminders();
                VerifyReminderPresence();
                UnregisterReminders();
                VerifyNoReminderEntryForActor();
                actorManager.Abort();
            }

            private void RegisterReminders()
            {
                ConsoleLogHelper.LogInfo("Registering reminders...");

                for (var i = 1; i <= RemainderCount; i++)
                {
                    actorManager.RegisterOrUpdateReminderAsync(
                        actorId,
                        "Reminder_" + i,
                        null,
                        TimeSpan.FromSeconds(60),
                        TimeSpan.FromSeconds(60),
                        false).GetAwaiter().GetResult();
                }
            }

            private void UnregisterReminders()
            {
                ConsoleLogHelper.LogInfo("Unregistered reminders...");

                for (var i = 1; i <= RemainderCount; i++)
                {
                    actorManager.UnregisterReminderAsync(
                        "Reminder_" + i,
                        actorId,
                        false).GetAwaiter().GetResult();
                }
            }

            private void VerifyReminderPresence()
            {
                for (var i = 1; i <= RemainderCount; i++)
                {
                    actorManager.GetReminder("Reminder_" + i, actorId);
                }
            }

            private void VerifyNoReminders()
            {
                if (actorManager.Test_HasAnyReminders())
                {
                    throw new InvalidOperationException($"Reminders still exist.");
                }
            }

            private void VerifyNoReminderEntryForActor()
            {
                if (actorManager.Test_ReminderDictionaryHasEntry(actorId))
                {
                    throw new InvalidOperationException($"Reminder entry for actor still exist.");
                }
            }

            private void VerifyNoRemindersWithRetry()
            {
                var retryCount = 3;

                for (var retry = 1; retry <= retryCount; retry++)
                {
                    ConsoleLogHelper.LogInfo($"VerifyNoRemindersWithRetry: Retry = {retry}.");

                    try
                    {
                        VerifyNoReminders();
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        if (retry == retryCount)
                        {
                            throw;
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            private void ResetActorManager()
            {
                ConsoleLogHelper.LogInfo("Resetting ActorManager...");
                actorManager = new ActorManager(actorService, clock, diagnostics);

                actorManager.OpenAsync(null, CancellationToken.None).GetAwaiter().GetResult();
                actorManager.StartLoadingRemindersAsync(CancellationToken.None).GetAwaiter().GetResult();

                while (!actorManager.HasRemindersLoaded)
                {
                    ConsoleLogHelper.LogInfo("Waiting for reminders to load...");
                    Task.Delay(TimeSpan.FromMilliseconds(100)).GetAwaiter().GetResult();
                }
            }
        }

        public class DiagnosticEvents : ActorManagerTest
        {
            readonly static IFuzz fuzzy = new RandomFuzz();

            ActorManager sut;

            readonly string callContext = fuzzy.String();

            public DiagnosticEvents()
            {
                sut = new ActorManager(actorService, clock, diagnostics);
            }

            public class DispatchToActorAsync : DiagnosticEvents
            {
                readonly DiagnosticsContext mockDiagnoticContext = Mock.Of<DiagnosticsContext>();
                readonly PendingActorMethodDiagnosticData actorMethodDiagnosticData;

                readonly long pendingCalls = fuzzy.UInt32();
                readonly long deltaCalls = fuzzy.UInt16();

                public DispatchToActorAsync()
                {
                    Mock.Get(mockDiagnoticContext).Setup(diagnotic => diagnotic.UpdateLastReportedActorMethodCalls()).Returns(deltaCalls);
                    Mock.Get(mockDiagnoticContext).Setup(diagnotic => diagnotic.PendingActorMethodCalls).Returns(pendingCalls);
                    sut.GetActor(actorId, true, false).Actor.Field<DiagnosticsContext>().Set(mockDiagnoticContext);

                    actorMethodDiagnosticData = new PendingActorMethodDiagnosticData() { ActorId = actorId, PendingActorMethodCalls = pendingCalls, PendingActorMethodCallsDelta = deltaCalls };
                }

                [Fact]
                public async Task EmitsDiagnosticsNoException()
                {
                    await sut.DispatchToActorAsync(
                        actorId: actorId,
                        actorMethodContext: new ActorMethodContext(),
                        createIfRequired: true,
                        (actorBase, cancellationToken) => Task.FromResult((ActorReminder)null),
                        callContext: callContext,
                        timerCall: false,
                        cancellationToken: TestContext.Current.CancellationToken);

                    Mock.Get(mockDiagnoticContext).Verify(d => d.IncremenetPendingActorMethodCalls(), Times.Once);
                    Mock.Get(mockDiagnoticContext).Verify(d => d.UpdateLastReportedActorMethodCalls(), Times.Once);
                    Mock.Get(mockDiagnoticContext).Verify(d => d.DecremenetPendingActorMethodCalls(), Times.Never);

                    Mock.Get(diagnostics).Verify(d => d.AcquireActorLockFinish(It.Is<PendingActorMethodDiagnosticData>(data => data.Equals(actorMethodDiagnosticData)), startTime), Times.Once);
                    Mock.Get(diagnostics).Verify(d => d.ReleaseActorLock(startTime), Times.Once);
                }

                [Fact]
                public async Task EmitsDiagnosticsWhenException()
                {
                    await Assert.ThrowsAsync<NullReferenceException>(async () => await sut.DispatchToActorAsync(
                            actorId: actorId,
                            actorMethodContext: new ActorMethodContext(),
                            createIfRequired: true,
                            (actorBase, cancellationToken) => Task.FromResult((ActorReminder)null),
                            callContext: null,
                            timerCall: false,
                            cancellationToken: TestContext.Current.CancellationToken));

                    Mock.Get(mockDiagnoticContext).Verify(d => d.IncremenetPendingActorMethodCalls(), Times.Once);
                    Mock.Get(mockDiagnoticContext).Verify(d => d.DecremenetPendingActorMethodCalls(), Times.Once);
                    Mock.Get(mockDiagnoticContext).Verify(d => d.UpdateLastReportedActorMethodCalls(), Times.Never);
                }
            }

            public class ActorActivate : DiagnosticEvents
            {
                [Fact]
                public async Task EmitDiagnosticsWhenActorActivatedAsync()
                {
                    await sut.DispatchToActorAsync(
                        actorId: actorId,
                        actorMethodContext: new ActorMethodContext(),
                        createIfRequired: true,
                        (actorBase, cancellationToken) => Task.FromResult((ActorReminder)null),
                        callContext: callContext,
                        timerCall: false,
                        cancellationToken: TestContext.Current.CancellationToken);

                    Mock.Get(diagnostics).Verify(d => d.ActorActivated(actorId), Times.Once);
                }

                [Fact]
                public async Task EmitDiagnosticsWhenActorDeactivatedAsync()
                {
                    await sut.DispatchToActorAsync(
                        actorId: actorId,
                        actorMethodContext: new ActorMethodContext(),
                        createIfRequired: true,
                        (actorBase, cancellationToken) => Task.FromResult((ActorReminder)null),
                        callContext: callContext,
                        timerCall: false,
                        cancellationToken: TestContext.Current.CancellationToken);
                    await sut.StartLoadingRemindersAsync(CancellationToken.None);
                    sut.GetActor(actorId, true, false).Actor.IsDummy = false;

                    await sut.DeleteActorAsync(
                        actorId: actorId,
                        callContext: callContext,
                        cancellationToken: TestContext.Current.CancellationToken);

                    Mock.Get(diagnostics).Verify(d => d.ActorDeactivated(actorId), Times.Once);
                }
            }

            public class OnActivateInternAsync : DiagnosticEvents
            {
                readonly ActorBase actor;
                public OnActivateInternAsync()
                {
                    actor = sut.GetActor(actorId, true, false).Actor;
                    actor.Field<IDiagnostics>().Set(diagnostics);
                }

                [Fact]
                public async Task OnActivateInternAsyncEmitsDiagnosticsAsync()
                {
                    await actor.OnActivateInternalAsync();

                    Mock.Get(diagnostics).Verify(d => d.ActorOnActivateAsyncStart(), Times.Once);
                    Mock.Get(diagnostics).Verify(d => d.ActorOnActivateAsyncFinish(startTime), Times.Once);
                }
            }
        }
    }
}
