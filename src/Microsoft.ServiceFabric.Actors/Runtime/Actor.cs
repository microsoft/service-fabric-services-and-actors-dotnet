// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an actor that can have multiple reliable 'named' states associated with it.
    /// </summary>
    /// <remarks>
    /// The state is preserved across actor garbage collections and fail-overs. The storage and retrieval of the state is
    /// provided by the actor state provider <see cref="IActorStateProvider"/>.
    /// </remarks>
    /// <seealso cref="ActorBase"/>
    public abstract class Actor : ActorBase
    {
        private IActorStateManager stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="actorService">
        /// The <see cref="ActorService"/> that will host this actor instance.
        /// </param>
        /// <param name="actorId">
        /// The <see cref="ActorId"/> for this actor instance.
        /// </param>
        protected Actor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            this.stateManager = this.ActorService.CreateStateManager(this);
        }

        /// <summary>
        /// Gets the state manager for <see cref="Actor"/>
        /// which can be used to get/add/update/remove named states.
        /// </summary>
        /// <value>
        /// An <see cref="IActorStateManager"/> which can be used to manage actor state.
        /// </value>
        public IActorStateManager StateManager
        {
            get { return this.stateManager; }
        }

        /// <summary>
        /// Saves all the state changes (add/update/remove) that were made since last call to
        /// <see cref="Actor.SaveStateAsync"/>,
        /// to the actor state provider associated with the actor.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        protected Task SaveStateAsync()
        {
            return this.DoSaveStateAsync();
        }
        
        internal override Task OnResetStateAsyncInternal()
        {
            return this.stateManager.ClearCacheAsync();
        }

        internal override Task OnSaveStateAsyncInternal()
        {
            return this.DoSaveStateAsync();
        }

        internal override Task OnPostActivateAsync()
        {
            return this.SaveStateAsync();
        }

        internal override async Task OnDeactivateInternalAsync()
        {
            await this.stateManager.ClearCacheAsync();
            await base.OnDeactivateInternalAsync();
        }

        private async Task DoSaveStateAsync()
        {
            if (!this.IsDirty)
            {
                await this.stateManager.SaveStateAsync();
            }
        }
    }
}
