// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

    public class DependencyInjectionTests
    {
        public interface IMockActor : IActor
        {
            Task ActorMethodA();
        }

        public interface IMockActorEvent : IActorEvents
        {
            void MockActorEventA();

            void MockActorEventB(ActorId id);
        }

        [Fact]
        public async Task VerifyActorMockability()
        {
            var mockActorId = ActorId.CreateRandom();

            ConsoleLogHelper.LogInfo("Creating Mock Actor Service...");
            var mockActorService = TestMocksRepository.GetActorService<MockActor>();

            ConsoleLogHelper.LogInfo("Creating Mock Actor...");
            var mockActor = new MockActor(mockActorService, mockActorId);

            ConsoleLogHelper.LogInfo("Verifying Public Actor Members...");

            Assert.Equal(mockActorId, mockActor.Id); // Id from Actor should be what was passed while creating the actor
            Assert.Equal(mockActorService.GetHashCode(), mockActor.ActorService.GetHashCode()); // ActorService from actor should be what was passed while creating Actor.
            Assert.Equal(mockActorService.Context.CodePackageActivationContext.ApplicationName, mockActor.ApplicationName); // Application Name from Actor should be same as what is coming form service's CodePackageActiviationContext
            Assert.Equal(mockActorService.Context.ServiceName, mockActor.ServiceUri); // ServiceUri from Actor should be same as what is coming form ServiceContext

            ConsoleLogHelper.LogInfo("Verifying Actor State Mockability...");
            await mockActor.VerifyActorStateMockabilityAsync();

            ConsoleLogHelper.LogInfo("Verifying Remider Mockability...");
            await mockActor.VerifyRemiderMockabilityAsync();

            ConsoleLogHelper.LogInfo("Verifying Timer Mockability...");
            mockActor.VerifyTimerMockability();

            ConsoleLogHelper.LogInfo("Verifying Actor Event Mockability...");
            mockActor.VerifyActorEventMockability();
        }

        internal class MockActor : Actor, IMockActor, IActorEventPublisher<IMockActorEvent>, IRemindable
        {
            public MockActor(ActorService actorService, ActorId actorId)
                : base(actorService, actorId)
            {
            }

            public async Task VerifyActorStateMockabilityAsync()
            {
                // Try to cover all code path for ActorStateManager to ensure they are mockable.
                await this.StateManager.AddStateAsync("State1", 10);
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State1")); // 10 was added for State1 using AddStateAsync

                await this.StateManager.GetOrAddStateAsync("State2", 10);
                Assert.Equal(10, await this.StateManager.GetOrAddStateAsync("State2", 20)); // New value of State2 should not be added by GetOrAddStateAsync as it exists already

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State3")); // 10 was added for State3 using AddOrUpdateStateAsync(add).

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                Assert.Equal(20, await this.StateManager.GetStateAsync<int>("State3")); // 10 was added for State3 with AddOrUpdateStateAsync(update).

                await this.StateManager.SetStateAsync("State3", 30);
                Assert.Equal(30, await this.StateManager.GetStateAsync<int>("State3")); // 30 was added for State3 using SetStateAsync(update).

                await this.StateManager.SetStateAsync("State4", 10);
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State4")); // 10 was added for State4 using SetStateAsync(add).

                Assert.Equal(4, (await this.StateManager.GetStateNamesAsync()).Count()); // 4 states have been added (GetStateNamesAsync verification).

                await this.StateManager.RemoveStateAsync("State1");
                Action action = () => this.StateManager.RemoveStateAsync("State1").GetAwaiter().GetResult();
                Assert.Throws<KeyNotFoundException>(action); // State1 was removed using RemoveStateAsync (RemoveStateAsync verification)

                action = () => this.StateManager.GetStateAsync<int>("State1").GetAwaiter().GetResult();
                Assert.Throws<KeyNotFoundException>(action); // State1 was removed using RemoveStateAsync (GetStateAsync verification)

                Assert.False(await this.StateManager.ContainsStateAsync("State1")); // State1 has been removed (ContainsStateAsync(State2) verification)
                Assert.True(await this.StateManager.ContainsStateAsync("State2")); // State2 hasn't been removed (ContainsStateAsync(State2) verification)

                Assert.True(await this.StateManager.TryAddStateAsync("State5", 10)); // State5 is added for first time (TryAddStateAsync(1) verification)
                Assert.False(await this.StateManager.TryAddStateAsync("State4", 10)); // State4 is being added again (TryAddStateAsync(2) verification)

                Assert.True((await this.StateManager.TryGetStateAsync<int>("State2")).HasValue); // STate2 hasn't been removed (TryGetStateAsync(1) verification)
                Assert.False((await this.StateManager.TryGetStateAsync<int>("State1")).HasValue); // State1 ahs been removed (TryGetStateAsync(2) verification)

                Assert.True(await this.StateManager.TryRemoveStateAsync("State2")); // State2 hasn't been removed yet (TryRemoveStateAsync(1) verification)
                Assert.False(await this.StateManager.TryRemoveStateAsync("State1")); // State1 has been removed already (TryRemoveStateAsync(2) verification).

                await this.StateManager.SaveStateAsync();
                await this.SaveStateAsync();
                await this.StateManager.ClearCacheAsync();
            }

            public async Task VerifyRemiderMockabilityAsync()
            {
                Action action = () => this.GetReminder("NonExistingReminder");
                Assert.Throws<ReminderNotFoundException>(action); // reminder doesn't exist.

                await this.RegisterReminderAsync("MockReminder", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                var reminder = this.GetReminder("MockReminder");
                Assert.Equal("MockReminder", reminder.Name); // Reminder was registered with this name
                Assert.Null(reminder.State); // Reminder was registered will null state
                Assert.Equal(TimeSpan.FromSeconds(2), reminder.DueTime); // Reminder was registered with this due time
                Assert.Equal(TimeSpan.FromSeconds(2), reminder.Period); // Reminder was registered with this period

                await this.UnregisterReminderAsync(reminder);

                action = () => this.GetReminder("MockReminder");
                Assert.Throws<ReminderNotFoundException>(action); // reminder was removed and doesn't exist.
            }

            public void VerifyTimerMockability()
            {
                var actorTimer = TestMocksRepository.GetMockActorTimer();
                Action action = () => this.UnregisterTimer(actorTimer);
                action(); // Should not throw

                this.RegisterTimer((obj) => Task.FromResult(true), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                action(); // Should not throw
            }

            public void VerifyActorEventMockability()
            {
                IMockActorEvent actorEvent = null;
                actorEvent = this.GetEvent<IMockActorEvent>(); // Should not throw
                actorEvent.MockActorEventA(); // Should not throw
                actorEvent.MockActorEventB(this.Id); // Should not throw
            }

            public Task ActorMethodA()
            {
                throw new NotImplementedException();
            }
            public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
            {
                throw new NotImplementedException();
            }
        }
    }
}
