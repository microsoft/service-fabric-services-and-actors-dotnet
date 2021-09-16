// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors.Remoting;

    internal class Helper
    {
        public static string GetCallContext()
        {
            if (ActorLogicalCallContext.TryGet(out var callContextValue))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    callContextValue,
                    Guid.NewGuid().ToString());
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }

        public static bool IsMigrationSource(List<Type> types)
        {
            return ActorStateMigrationAttribute.Get(types).ActorStateMigration.HasFlag(ActorStateMigration.Source);
        }

        public static bool IsMigrationTarget(List<Type> types)
        {
            return ActorStateMigrationAttribute.Get(types).ActorStateMigration.HasFlag(ActorStateMigration.Target);
        }

        // Assembly.CreateQualifiedName is not coreCLRCompliant. Implementation of the method from .NET
        // This method creates the name of a type qualified by the display name of its assembly.
        public static string CreateQualifiedNameForAssembly(string assemblyName, string typeName)
        {
            return typeName + ", " + assemblyName;
        }
    }
}
