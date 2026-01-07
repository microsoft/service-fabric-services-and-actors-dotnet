// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Actors
{
    [Serializable]
    internal sealed class ActorDeletedException : FabricTransientException
    {
        public ActorDeletedException()
            : base()
        {
        }

        public ActorDeletedException(string message)
            : base(message)
        {
        }

        public ActorDeletedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
