// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents the attributes that allows configuring the properties of the actor service.
    /// The attribute is applied on the actor type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ActorStateMigrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorStateMigrationAttribute"/> class.
        /// </summary>
        /// /// <param name="actorStateMigration">Indicates an actor service is source or target for migration.</param>
        public ActorStateMigrationAttribute(ActorStateMigration actorStateMigration)
        {
            this.ActorStateMigration = actorStateMigration;
        }

        /// <summary>
        /// Gets the enum representing type of actor service migration.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorStateMigration"/> representing type of state store to use for the actor.</value>
        public ActorStateMigration ActorStateMigration { get; private set; }

        internal static string DefaultKvsMigrationListenerName
        {
            get { return "Kvs_Grcp_Listener"; }
        }

        /// <summary>
        /// Gets the enum representing type of actor service migration.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorStateMigration"/> representing type of state store to use for the actor.</value>
        internal static ActorStateMigrationAttribute Get(IEnumerable<Type> types = null)
        {
            if (types != null)
            {
                foreach (var t in types)
                {
                    var attribute = t.GetTypeInfo().Assembly.GetCustomAttribute<ActorStateMigrationAttribute>();
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }

            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var attribute = assembly.GetCustomAttribute<ActorStateMigrationAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return new ActorStateMigrationAttribute(ActorStateMigration.None);
        }
    }
}
