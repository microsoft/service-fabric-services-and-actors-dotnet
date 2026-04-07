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
    using Microsoft.ServiceFabric.TestFramework;
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

            Assert.Equal(mockActorId, mockActor.Id);
            Assert.Equal(mockActorService.GetHashCode(), mockActor.ActorService.GetHashCode());
            Assert.Equal(mockActorService.Context.CodePackageActivationContext.ApplicationName, mockActor.ApplicationName);
            Assert.Equal(mockActorService.Context.ServiceName, mockActor.ServiceUri);

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
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State1"));

                await this.StateManager.GetOrAddStateAsync("State2", 10);
                Assert.Equal(10, await this.StateManager.GetOrAddStateAsync("State2", 20));

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State3"));

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                Assert.Equal(20, await this.StateManager.GetStateAsync<int>("State3"));

                await this.StateManager.SetStateAsync("State3", 30);
                Assert.Equal(30, await this.StateManager.GetStateAsync<int>("State3"));

                await this.StateManager.SetStateAsync("State4", 10);
                Assert.Equal(10, await this.StateManager.GetStateAsync<int>("State4"));

                Assert.Equal(4, (await this.StateManager.GetStateNamesAsync()).Count());

                await this.StateManager.RemoveStateAsync("State1");
                Action action = () => this.StateManager.RemoveStateAsync("State1").GetAwaiter().GetResult();
                Assert.Throws<KeyNotFoundException>(action);

                action = () => this.StateManager.GetStateAsync<int>("State1").GetAwaiter().GetResult();
                Assert.Throws<KeyNotFoundException>(action);

                Assert.False(await this.StateManager.ContainsStateAsync("State1"));
                Assert.True(await this.StateManager.ContainsStateAsync("State2"));

                Assert.True(await this.StateManager.TryAddStateAsync("State5", 10));
                Assert.False(await this.StateManager.TryAddStateAsync("State4", 10));

                Assert.True((await this.StateManager.TryGetStateAsync<int>("State2")).HasValue);
                Assert.False((await this.StateManager.TryGetStateAsync<int>("State1")).HasValue);

                Assert.True(await this.StateManager.TryRemoveStateAsync("State2"));
                Assert.False(await this.StateManager.TryRemoveStateAsync("State1"));

                await this.StateManager.SaveStateAsync();
                await this.SaveStateAsync();
                await this.StateManager.ClearCacheAsync();
            }

            public async Task VerifyRemiderMockabilityAsync()
            {
                Action action = () => this.GetReminder("NonExistingReminder");
                Assert.Throws<ReminderNotFoundException>(action);

                await this.RegisterReminderAsync("MockReminder", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                var reminder = this.GetReminder("MockReminder");
                Assert.Equal("MockReminder", reminder.Name);
                Assert.Null(reminder.State);
                Assert.Equal(TimeSpan.FromSeconds(2), reminder.DueTime);
                Assert.Equal(TimeSpan.FromSeconds(2), reminder.Period);

                await this.UnregisterReminderAsync(reminder);

                action = () => this.GetReminder("MockReminder");
                Assert.Throws<ReminderNotFoundException>(action);
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
