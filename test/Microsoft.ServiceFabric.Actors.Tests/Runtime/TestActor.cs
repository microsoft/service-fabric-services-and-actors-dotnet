// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    internal interface ITestActor : IActor
    {
        Task TestMethod();
    }

    sealed class TestActor : Actor, ITestActor
    {
        public TestActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task TestMethod()
        {
            throw new NotImplementedException();
        }
    }
}
