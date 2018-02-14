// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Common;
    using SR = Microsoft.ServiceFabric.Actors.SR;
    using System.Globalization;

    internal sealed class ActorStateManager : IActorStateManager
    {
        private readonly IActorStateProvider stateProvider;
        private readonly Dictionary<string, StateMetadata> stateChangeTracker;
        private readonly ActorBase actor;

        internal ActorStateManager(ActorBase actor, IActorStateProvider actorStateProvider)
        {
            this.actor = actor;
            this.stateProvider = actorStateProvider;
            this.stateChangeTracker = new Dictionary<string, StateMetadata>();
        }

        #region IActorStateManager Members

        public async Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            if (!(await this.TryAddStateAsync(stateName, value, cancellationToken)))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Actors.SR.ActorStateAlreadyExists, stateName));
            }
        }

        public async Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                // Check if the property was marked as remove in the cache
                if (stateMetadata.ChangeKind == StateChangeKind.Remove)
                {
                    this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeKind.Update);
                    return true;
                }

                return false;
            }

            if (await this.stateProvider.ContainsStateAsync(this.actor.Id, stateName, cancellationToken))
            {
                return false;
            }

            this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeKind.Add);
            return true;
        }

        public async Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            var condRes = await this.TryGetStateAsync<T>(stateName, cancellationToken);

            if (condRes.HasValue)
            {
                return condRes.Value;
            }

            throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, SR.ErrorNamedActorStateNotFound, stateName));
        }

        public async Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                // Check if the property was marked as remove in the cache
                if (stateMetadata.ChangeKind == StateChangeKind.Remove)
                {
                    return new ConditionalValue<T>(false, default(T));
                }

                return new ConditionalValue<T>(true, (T)stateMetadata.Value);
            }

            var conditionalResult = await this.TryGetStateFromStateProviderAsync<T>(stateName, cancellationToken);
            if (conditionalResult.HasValue)
            {
                this.stateChangeTracker.Add(stateName, StateMetadata.Create(conditionalResult.Value, StateChangeKind.None));
            }

            return conditionalResult;
        }

        public async Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];
                stateMetadata.Value = value;

                if (stateMetadata.ChangeKind == StateChangeKind.None ||
                    stateMetadata.ChangeKind == StateChangeKind.Remove)
                {
                    stateMetadata.ChangeKind = StateChangeKind.Update;
                }
            }
            else if (await this.stateProvider.ContainsStateAsync(this.actor.Id, stateName, cancellationToken))
            {
                this.stateChangeTracker.Add(stateName, StateMetadata.Create(value, StateChangeKind.Update));
            }
            else
            {
                this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeKind.Add);
            }
        }

        public async Task RemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            if (!(await this.TryRemoveStateAsync(stateName, cancellationToken)))
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, SR.ErrorNamedActorStateNotFound, stateName));
            }
        }

        public async Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                switch (stateMetadata.ChangeKind)
                {
                    case StateChangeKind.Remove:
                        return false;
                    case StateChangeKind.Add:
                        this.stateChangeTracker.Remove(stateName);
                        return true;
                }

                stateMetadata.ChangeKind = StateChangeKind.Remove;
                return true;
            }

            if (await this.stateProvider.ContainsStateAsync(this.actor.Id, stateName, cancellationToken))
            {
                this.stateChangeTracker.Add(stateName, StateMetadata.CreateForRemove());
                return true;
            }

            return false;
        }

        public async Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken)
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                // Check if the property was marked as remove in the cache
                return stateMetadata.ChangeKind != StateChangeKind.Remove;
            }

            if (await this.stateProvider.ContainsStateAsync(this.actor.Id, stateName, cancellationToken))
            {
                return true;
            }

            return false;
        }

        public async Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            var condRes = await this.TryGetStateAsync<T>(stateName, cancellationToken);

            if (condRes.HasValue)
            {
                return condRes.Value;
            }

            var changeKind = this.IsStateMarkedForRemove(stateName) ? StateChangeKind.Update : StateChangeKind.Add;

            this.stateChangeTracker[stateName] = StateMetadata.Create(value, changeKind);
            return value;
        }

        public async Task<T> AddOrUpdateStateAsync<T>(
            string stateName,
            T addValue,
            Func<string, T, T> updateValueFactory,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfClosed();

            Requires.Argument("stateName", stateName).NotNull();

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                // Check if the property was marked as remove in the cache
                if (stateMetadata.ChangeKind == StateChangeKind.Remove)
                {
                    this.stateChangeTracker[stateName] = StateMetadata.Create(addValue, StateChangeKind.Update);
                    return addValue;
                }

                var newValue = updateValueFactory.Invoke(stateName, (T)stateMetadata.Value);
                stateMetadata.Value = newValue;

                if (stateMetadata.ChangeKind == StateChangeKind.None)
                {
                    stateMetadata.ChangeKind = StateChangeKind.Update;
                }

                return newValue;
            }

            var conditionalResult = await this.TryGetStateFromStateProviderAsync<T>(stateName, cancellationToken);
            if (conditionalResult.HasValue)
            {
                var newValue = updateValueFactory.Invoke(stateName, conditionalResult.Value);
                this.stateChangeTracker.Add(stateName, StateMetadata.Create(newValue, StateChangeKind.Update));

                return newValue;
            }

            this.stateChangeTracker[stateName] = StateMetadata.Create(addValue, StateChangeKind.Add);
            return addValue;
        }

        public async Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfClosed();

            var namesFromStateProvider = await this.stateProvider.EnumerateStateNamesAsync(this.actor.Id, cancellationToken);
            var stateNameList = new List<string>(namesFromStateProvider);

            var kvPairEnumerator = this.stateChangeTracker.GetEnumerator();

            while (kvPairEnumerator.MoveNext())
            {
                switch (kvPairEnumerator.Current.Value.ChangeKind)
                {
                    case StateChangeKind.Add:
                        stateNameList.Add(kvPairEnumerator.Current.Key);
                        break;
                    case StateChangeKind.Remove:
                        stateNameList.Remove(kvPairEnumerator.Current.Key);
                        break;
                }
            }

            return stateNameList;
        }

        public Task ClearCacheAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.stateChangeTracker.Clear();
            return TaskDone.Done;
        }

        public async Task SaveStateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfClosed();

            if (this.stateChangeTracker.Count > 0)
            {
                var stateChangeList = new List<ActorStateChange>();
                var statesToRemove = new List<string>();

                foreach (var stateName in this.stateChangeTracker.Keys)
                {
                    var stateMetadata = this.stateChangeTracker[stateName];

                    if (stateMetadata.ChangeKind != StateChangeKind.None)
                    {
                        stateChangeList.Add(
                            new ActorStateChange(stateName, stateMetadata.Type, stateMetadata.Value, stateMetadata.ChangeKind));

                        if (stateMetadata.ChangeKind == StateChangeKind.Remove)
                        {
                            statesToRemove.Add(stateName);
                        }

                        stateMetadata.ChangeKind = StateChangeKind.None;
                    }
                }

                if (stateChangeList.Count > 0)
                {
                    this.actor.Manager.DiagnosticsEventManager.SaveActorStateStart(this.actor);

                    await this.stateProvider.SaveStateAsync(this.actor.Id, stateChangeList.AsReadOnly(), cancellationToken);

                    this.actor.Manager.DiagnosticsEventManager.SaveActorStateFinish(this.actor);
                }

                foreach (var stateToRemove in statesToRemove)
                {
                    this.stateChangeTracker.Remove(stateToRemove);
                }
            }
        }

        #endregion

        private bool IsStateMarkedForRemove(string stateName)
        {
            if (this.stateChangeTracker.ContainsKey(stateName) &&
                this.stateChangeTracker[stateName].ChangeKind == StateChangeKind.Remove)
            {
                return true;
            }

            return false;
        }

        private async Task<ConditionalValue<T>> TryGetStateFromStateProviderAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            ConditionalValue<T> result;

            this.actor.Manager.DiagnosticsEventManager.LoadActorStateStart(this.actor);

            if (await this.stateProvider.ContainsStateAsync(this.actor.Id, stateName, cancellationToken))
            {
                var value = await this.stateProvider.LoadStateAsync<T>(this.actor.Id, stateName, cancellationToken);
                result = new ConditionalValue<T>(true, value);
            }
            else
            {
                result = new ConditionalValue<T>(false, default(T));
            }

            this.actor.Manager.DiagnosticsEventManager.LoadActorStateFinish(this.actor);
            return result;
        }

        /// <summary>
        /// Once ActorManager is closed, StateManager should not allow any new operation.
        /// </summary>
        private void ThrowIfClosed()
        {
            if (this.actor.Manager.IsClosed)
            {
                throw new FabricNotPrimaryException();
            }
        }

        #region Helper Classes

        private sealed class StateMetadata
        {
            private readonly Type type;

            private StateMetadata(object value, Type type, StateChangeKind changeKind)
            {
                this.Value = value;
                this.type = type;
                this.ChangeKind = changeKind;
            }

            public object Value { get; set; }

            public StateChangeKind ChangeKind { get; set; }

            public Type Type
            {
                get { return this.type; }
            }

            public static StateMetadata Create<T>(T value, StateChangeKind changeKind)
            {
                return new StateMetadata(value, typeof(T), changeKind);
            }

            public static StateMetadata CreateForRemove()
            {
                return new StateMetadata(null, typeof(object), StateChangeKind.Remove);
            }
        }

        #endregion
    }
}
