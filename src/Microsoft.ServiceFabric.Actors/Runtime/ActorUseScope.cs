// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    internal sealed class ActorUseScope : IDisposable
    {
        public static ActorUseScope TryCreate(ActorBase actor, bool timerUse)
        {
            if (actor.GcHandler.TryUse(timerUse))
            {
                try
                {
                    return new ActorUseScope(actor, timerUse);
                }
                catch (Exception)
                {
                    actor.GcHandler.Unuse(timerUse);
                    throw;
                }
            }

            return null;
        }

        private ActorUseScope(ActorBase actor, bool timerUse)
        {
            this.Actor = actor;
            this.timerUse = timerUse;
        }

        ~ActorUseScope()
        {
            this.Dispose(false);
        }

        public ActorBase Actor { get; private set; }

        /// <summary>
        /// Indicates a value whether the use is for a timer call.
        /// </summary>
        private readonly bool timerUse;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Actor != null)
                {
                    this.Actor.GcHandler.Unuse(this.timerUse);
                    this.Actor = null;
                }
            }
        }
    }
}
