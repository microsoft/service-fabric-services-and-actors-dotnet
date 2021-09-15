// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;

    internal class Utility
    {
        internal static bool IsMigrationSource(List<Type> types)
        {
            return ActorStateMigrationAttribute.Get(types).ActorStateMigration.HasFlag(ActorStateMigration.Source);
        }

        internal static bool IsMigrationTarget(List<Type> types)
        {
            return ActorStateMigrationAttribute.Get(types).ActorStateMigration.HasFlag(ActorStateMigration.Target);
        }
    }
}
