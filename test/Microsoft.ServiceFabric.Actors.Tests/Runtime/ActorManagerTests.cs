// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Tests;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime
{

    /// <summary>
    /// Unit tests for ActorManager.
    /// </summary>
    public class ActorManagerTests
    {
        private const int ReminderCount = 10;
        private readonly ActorId actorId;
        private readonly ActorService actorService;
        private ActorManager actorManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorManagerTests"/> class.
        /// </summary>
        public ActorManagerTests()
        {
            actorId = ActorId.CreateRandom();
            actorService = TestMocksRepository.GetActorService<MockActor>();

            var friendlyNameBuilder = new ActorMethodFriendlyNameBuilder(actorService.ActorTypeInformation);
            actorService.InitializeInternal(friendlyNameBuilder);
        }

        public class Remainder : ActorManagerTests
        {

            /// <summary>
            /// Verifies ActorManager close.
            /// </summary>
            [Fact]
            public async Task VerifyClose()
            {
                ResetActorManager();
                RegisterReminders();
                VerifyReminderPresence();
                await actorManager.CloseAsync(CancellationToken.None);
                VerifyNoReminders();
            }

            /// <summary>
            /// Verifieis aCtormanager abort.
            /// </summary>
            [Fact]
            public void VerifyAbort()
            {
                ResetActorManager();
                RegisterReminders();
                VerifyReminderPresence();
                actorManager.Abort();
                VerifyNoRemindersWithRetry();
            }

            /// <summary>
            /// Verify FireReminder after close.
            /// </summary>
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

            /// <summary>
            /// Verifies that Actor entry frpom Reminders dictionary is removed, when last reminder for the actor is removed.
            /// </summary>
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

            private void ResetActorManager()
            {
                ConsoleLogHelper.LogInfo("Resetting ActorManager...");
                actorManager = new ActorManager(actorService);

                actorManager.OpenAsync(null, CancellationToken.None).GetAwaiter().GetResult();
                actorManager.StartLoadingRemindersAsync(CancellationToken.None).GetAwaiter().GetResult();

                while (!actorManager.HasRemindersLoaded)
                {
                    ConsoleLogHelper.LogInfo("Waiting for reminders to load...");
                    Task.Delay(TimeSpan.FromMilliseconds(100)).GetAwaiter().GetResult();
                }
            }

            private void RegisterReminders()
            {
                ConsoleLogHelper.LogInfo("Registering reminders...");

                for (var i = 1; i <= ReminderCount; i++)
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
                ConsoleLogHelper.LogInfo("Unregistering reminders...");

                for (var i = 1; i <= ReminderCount; i++)
                {
                    actorManager.UnregisterReminderAsync(
                        "Reminder_" + i,
                        actorId,
                        false).GetAwaiter().GetResult();
                }
            }

            private void VerifyReminderPresence()
            {
                for (var i = 1; i <= ReminderCount; i++)
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
        }


        public interface IMockActor : IActor
        {
            Task ActorMethodA();
        }

        internal class MockActor : Actor, IMockActor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockActor"/> class.
            /// </summary>
            /// <param name="actorService">Actor Service.</param>
            /// <param name="actorId">Actor Id.</param>
            public MockActor(ActorService actorService, ActorId actorId)
                : base(actorService, actorId)
            {
            }

            /// <inheritdoc/>
            public Task ActorMethodA()
            {
                throw new NotImplementedException();
            }
        }
    }
}
