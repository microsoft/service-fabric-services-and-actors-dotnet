// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

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
            this.actorId = ActorId.CreateRandom();
            this.actorService = TestMocksRepository.GetActorService<MockActor>();

            var friendlyNameBuilder = new ActorMethodFriendlyNameBuilder(this.actorService.ActorTypeInformation);
            this.actorService.InitializeInternal(friendlyNameBuilder);
        }

        /// <summary>
        /// Mock Actor Interface.
        /// </summary>
        public interface IMockActor : IActor
        {
            /// <summary>
            /// Mock Actor method.
            /// </summary>
            /// <returns>A task.</returns>
            Task ActorMethodA();
        }

        /// <summary>
        /// Verifies ActorManager close.
        /// </summary>
        [Fact]
        public void VerifyClose()
        {
            this.ResetActorManager();
            this.RegisterReminders();
            this.VerifyReminderPresence();
            this.actorManager.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();
            this.VerifyNoReminders();
        }

        /// <summary>
        /// Verifieis aCtormanager abort.
        /// </summary>
        [Fact]
        public void VerifyAbort()
        {
            this.ResetActorManager();
            this.RegisterReminders();
            this.VerifyReminderPresence();
            this.actorManager.Abort();
            this.VerifyNoRemindersWithRetry();
        }

        /// <summary>
        /// Verify FireReminder after close.
        /// </summary>
        [Fact]
        public void VerifyFireReminderNoThrow()
        {
            this.ResetActorManager();
            this.actorManager.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();

            var reminder = new ActorReminder(
                ActorId.CreateRandom(),
                this.actorManager,
                "reminderName",
                null,
                TimeSpan.FromMinutes(30),
                TimeSpan.FromMinutes(30));

            this.actorManager.FireReminderAsync(reminder).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifies that Actor entry frpom Reminders dictionary is removed, when last reminder for the actor is removed.
        /// </summary>
        [Fact]
        public void VerifyNoReminderEntry()
        {
            this.ResetActorManager();
            this.RegisterReminders();
            this.VerifyReminderPresence();
            this.UnregisterReminders();
            this.VerifyNoReminderEntryForActor();
            this.actorManager.Abort();
        }

        private void ResetActorManager()
        {
            ConsoleLogHelper.LogInfo("Resetting ActorManager...");
            this.actorManager = new ActorManager(this.actorService);

            this.actorManager.OpenAsync(null, CancellationToken.None).GetAwaiter().GetResult();
            this.actorManager.StartLoadingRemindersAsync(CancellationToken.None).GetAwaiter().GetResult();

            while (!this.actorManager.HasRemindersLoaded)
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
                this.actorManager.RegisterOrUpdateReminderAsync(
                    this.actorId,
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
                this.actorManager.UnregisterReminderAsync(
                    "Reminder_" + i,
                    this.actorId,
                    false).GetAwaiter().GetResult();
            }
        }

        private void VerifyReminderPresence()
        {
            for (var i = 1; i <= ReminderCount; i++)
            {
                this.actorManager.GetReminder("Reminder_" + i, this.actorId);
            }
        }

        private void VerifyNoReminders()
        {
            if (this.actorManager.Test_HasAnyReminders())
            {
                throw new InvalidOperationException($"Reminders still exist.");
            }
        }

        private void VerifyNoReminderEntryForActor()
        {
            if (this.actorManager.Test_ReminderDictionaryHasEntry(this.actorId))
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
                    this.VerifyNoReminders();
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

        /// <summary>
        /// Test Mock Actor.
        /// </summary>
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
