// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Indicates whether the actor service participates in state migration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class StateMigrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMigrationAttribute"/> class.
        /// </summary>
        /// <param name="stateMigration">Indicates an actor service is source or target for migration.</param>
        public StateMigrationAttribute(StateMigration stateMigration)
        {
            this.StateMigration = stateMigration;
        }

        /// <summary>
        /// Gets the enum representing type of actor service migration.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.Migration.StateMigration"/> representing type of state store to use for the actor.</value>
        public StateMigration StateMigration { get; private set; }

        internal static StateMigrationAttribute Get(Type actorType)
        {
            var attribute = new StateMigrationAttribute(StateMigration.None);

            var attributes = actorType.GetTypeInfo().GetCustomAttributes(typeof(StateMigrationAttribute), false);
            var enumerator = attributes.GetEnumerator();
            if (enumerator.MoveNext())
            {
                attribute.StateMigration = ((StateMigrationAttribute)enumerator.Current).StateMigration;
            }

            return attribute;
        }
    }
}
