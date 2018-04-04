// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

    /// <summary>
    /// Tests for Dependency Injection.
    /// </summary>
    public class DependencyInjectionTests
    {
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
        /// Mock actor Event.
        /// </summary>
        public interface IMockActorEvent : IActorEvents
        {
            /// <summary>
            /// Mock event A.
            /// </summary>
            void MockActorEventA();

            /// <summary>
            /// Mock event B.
            /// </summary>
            /// <param name="id">Actor Id</param>
            void MockActorEventB(ActorId id);
        }

        /// <summary>
        /// Verify mockability for ACtors.
        /// </summary>
        [Fact]
        public void VerifyActorMockability()
        {
            var mockActorId = ActorId.CreateRandom();

            ConsoleLogHelper.LogInfo("Creating Mock Actor Service...");
            var mockActorService = TestMocksRepository.GetActorService<MockActor>();

            ConsoleLogHelper.LogInfo("Creating Mock Actor...");
            var mockActor = new MockActor(mockActorService, mockActorId);

            ConsoleLogHelper.LogInfo("Verifying Public Actor Members...");

            mockActor.Id.Should().Be(mockActorId, "Id from Actor should be what was passed while creating the actor");
            mockActor.ActorService.GetHashCode().Should().Be(mockActorService.GetHashCode(), "ActorService from actor should be what was passed while creating Actor.");
            mockActor.ApplicationName.Should().Be(mockActorService.Context.CodePackageActivationContext.ApplicationName, "Application Name from Actor should be same as what is coming form service's CodePackageActiviationContext");
            mockActor.ServiceUri.Should().Be(mockActorService.Context.ServiceName, "ServiceUri from Actor should be same as what is coming form ServiceContext");

            ConsoleLogHelper.LogInfo("Verifying Actor State Mockability...");
            mockActor.VerifyActorStateMockabilityAsync().GetAwaiter().GetResult();

            ConsoleLogHelper.LogInfo("Verifying Remider Mockability...");
            mockActor.VerifyRemiderMockabilityAsync().GetAwaiter().GetResult();

            ConsoleLogHelper.LogInfo("Verifying Timer Mockability...");
            mockActor.VerifyTimerMockability();

            ConsoleLogHelper.LogInfo("Verifying Actor Event Mockability...");
            mockActor.VerifyActorEventMockability();
        }

        /// <summary>
        /// Test Mock Actor.
        /// </summary>
        internal class MockActor : Actor, IMockActor, IActorEventPublisher<IMockActorEvent>, IRemindable
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

            /// <summary>
            /// Verify mocking of ActorState.
            /// </summary>
            /// <returns>A task.</returns>
            public async Task VerifyActorStateMockabilityAsync()
            {
                // Try to cover all code path for ActorStateManager to ensure they are mockable.
                await this.StateManager.AddStateAsync("State1", 10);
                (await this.StateManager.GetStateAsync<int>("State1")).Should().Be(10, "10 was added for State1 using AddStateAsync");

                await this.StateManager.GetOrAddStateAsync("State2", 10);
                (await this.StateManager.GetOrAddStateAsync("State2", 20)).Should().Be(10, "New value of State2 should not be added by GetOrAddStateAsync as it exists already");

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                (await this.StateManager.GetStateAsync<int>("State3")).Should().Be(10, "10 was added for State3 using AddOrUpdateStateAsync(add).");

                await this.StateManager.AddOrUpdateStateAsync("State3", 10, (s, i) => 20);
                (await this.StateManager.GetStateAsync<int>("State3")).Should().Be(20, "10 was added for State3 with AddOrUpdateStateAsync(update).");

                await this.StateManager.SetStateAsync("State3", 30);
                (await this.StateManager.GetStateAsync<int>("State3")).Should().Be(30, "30 was added for State3 using SetStateAsync(update).");

                await this.StateManager.SetStateAsync("State4", 10);
                (await this.StateManager.GetStateAsync<int>("State4")).Should().Be(10, "10 was added for State4 using SetStateAsync(add).");

                (await this.StateManager.GetStateNamesAsync()).Count().Should().Be(4, "4 states have been added (GetStateNamesAsync verification).");

                await this.StateManager.RemoveStateAsync("State1");
                Action action = () => this.StateManager.RemoveStateAsync("State1").GetAwaiter().GetResult();
                action.ShouldThrow<KeyNotFoundException>("State1 was removed using RemoveStateAsync (RemoveStateAsync verification)");

                action = () => this.StateManager.GetStateAsync<int>("State1").GetAwaiter().GetResult();
                action.ShouldThrow<KeyNotFoundException>("State1 was removed using RemoveStateAsync (GetStateAsync verification)");

                (await this.StateManager.ContainsStateAsync("State1")).Should().BeFalse("State1 has been removed (ContainsStateAsync(State2) verification)");
                (await this.StateManager.ContainsStateAsync("State2")).Should().BeTrue("State2 hasn't been removed (ContainsStateAsync(State2) verification)");

                (await this.StateManager.TryAddStateAsync("State5", 10)).Should().BeTrue("State5 is added for first time (TryAddStateAsync(1) verification)");
                (await this.StateManager.TryAddStateAsync("State4", 10)).Should().BeFalse("State4 is being added again (TryAddStateAsync(2) verification)");

                (await this.StateManager.TryGetStateAsync<int>("State2")).HasValue.Should().BeTrue("STate2 hasn't been removed (TryGetStateAsync(1) verification)");
                (await this.StateManager.TryGetStateAsync<int>("State1")).HasValue.Should().BeFalse("State1 ahs been removed (TryGetStateAsync(2) verification)");

                (await this.StateManager.TryRemoveStateAsync("State2")).Should().BeTrue("State2 hasn't been removed yet (TryRemoveStateAsync(1) verification)");
                (await this.StateManager.TryRemoveStateAsync("State1")).Should().BeFalse("State1 has been removed already (TryRemoveStateAsync(2) verification).");

                await this.StateManager.SaveStateAsync();
                await this.SaveStateAsync();
                await this.StateManager.ClearCacheAsync();
            }

            /// <summary>
            /// Verify mocks for Reminders.
            /// </summary>
            /// <returns>A task.</returns>
            public async Task VerifyRemiderMockabilityAsync()
            {
                Action action = () => this.GetReminder("NonExistingReminder");
                action.ShouldThrow<ReminderNotFoundException>("reminder doesn't exist.");

                await this.RegisterReminderAsync("MockReminder", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                var reminder = this.GetReminder("MockReminder");
                reminder.Name.Should().Be("MockReminder", " Reminder was registered with this name");
                reminder.State.Should().BeNull("Reminder was registered will null state");
                reminder.DueTime.Should().Be(TimeSpan.FromSeconds(2), "Reminder was registered with this due time");
                reminder.Period.Should().Be(TimeSpan.FromSeconds(2), "Reminder was registered with this period");

                await this.UnregisterReminderAsync(reminder);

                action = () => this.GetReminder("MockReminder");
                action.ShouldThrow<ReminderNotFoundException>("reminder was removed and doesn't exist.");
            }

            /// <summary>
            /// Verify Timer mocks.
            /// </summary>
            public void VerifyTimerMockability()
            {
                var actorTimer = TestMocksRepository.GetMockActorTimer();
                Action action = () => this.UnregisterTimer(actorTimer);
                action.ShouldNotThrow("unregistering a timer that doesn't exist shouldn't throw.");

                this.RegisterTimer((obj) => Task.FromResult(true), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                action = () => this.UnregisterTimer(actorTimer);
                action.ShouldNotThrow("unregistering an existing timer shouldn't throw");
            }

            /// <summary>
            /// Verify Mocks for ActorEvents.
            /// </summary>
            public void VerifyActorEventMockability()
            {
                IMockActorEvent actorEvent = null;
                Action action = () => actorEvent = this.GetEvent<IMockActorEvent>();
                action.ShouldNotThrow("Getting an event should not throw.");

                action = () => actorEvent.MockActorEventA();
                action.ShouldNotThrow("actorEvent.MockActorEventA() verification");

                action = () => actorEvent.MockActorEventB(this.Id);
                action.ShouldNotThrow("actorEvent.MockActorEventB() verification");
            }

            /// <inheritdoc/>
            public Task ActorMethodA()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
            {
                throw new NotImplementedException();
            }
        }
    }
}
