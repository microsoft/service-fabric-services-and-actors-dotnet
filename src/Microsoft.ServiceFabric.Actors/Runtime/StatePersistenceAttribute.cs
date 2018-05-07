// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Indicates whether the actor state should be volatile (in-memory only), persisted, or not stored at all.
    /// The store type given to this attribute must match the type of state provider used in the actor service.
    /// </summary>
    /// <remarks>
    /// The StatePersistence attribute is not inherited by derived class, each Actor type must provide its StatePersistence level,
    /// If an Actor type doesn't specify a StatePersistence attribute, actor state is not replicated or written to disk.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class StatePersistenceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatePersistenceAttribute"/> class.
        /// </summary>
        /// <param name="statePersistence">Indicates how actor state is stored for an actor service.</param>
        public StatePersistenceAttribute(StatePersistence statePersistence)
        {
            this.StatePersistence = statePersistence;
        }

        /// <summary>
        /// Gets the enum representing type of state store to use for the actor.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.Runtime.StatePersistence"/> representing type of state store to use for the actor.</value>
        public StatePersistence StatePersistence { get; private set; }

        internal static StatePersistenceAttribute Get(Type actorType)
        {
            var attribute = new StatePersistenceAttribute(StatePersistence.None);

            var attributes = actorType.GetTypeInfo().GetCustomAttributes(typeof(StatePersistenceAttribute), false);
            var enumerator = attributes.GetEnumerator();
            if (enumerator.MoveNext())
            {
                attribute.StatePersistence = ((StatePersistenceAttribute)enumerator.Current).StatePersistence;
            }

            return attribute;
        }
    }
}
