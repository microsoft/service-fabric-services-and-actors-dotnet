// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Common;

    /// <summary>
    /// DummyActor object is created in case of Deletion of inactive actor.
    /// DummyActor object's ReentrantGuard is used for blocking other calls.
    /// </summary>
    internal sealed class DummyActor : ActorBase
    {
        internal DummyActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            this.IsDummy = true;
        }

        internal override Task OnResetStateAsyncInternal()
        {
            return TaskDone.Done;
        }

        internal override Task OnSaveStateAsyncInternal()
        {
            return TaskDone.Done;
        }

        internal override Task OnPostActivateAsync()
        {
            return TaskDone.Done;
        }
    }
}
