// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    internal interface IMockActor : IActor
    {
        Task ActorMethodA();
    }

    internal class MockActor : Actor, IMockActor
    {
        public MockActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task ActorMethodA()
        {
            throw new NotImplementedException();
        }
    }
}
