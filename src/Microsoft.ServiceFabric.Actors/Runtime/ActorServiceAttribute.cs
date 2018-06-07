// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors.Generator;

    /// <summary>
    /// Represents the attributes that allows configuring the properties of the actor service.
    /// The attribute is applied on the actor type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActorServiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorServiceAttribute"/> class.
        /// </summary>
        public ActorServiceAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the relative name of the actor service. This name will be combined with the application name to provide the full name of the
        /// actor service.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     By default, the actor service name is derived from the type of the actor interface
        ///     (<see cref="ActorNameFormat.GetFabricServiceName(Type,string)"/>).
        ///     However, in case when an actor interface is implemented by more than one actor, including by a derived type,
        ///     the name cannot be determined from the actor interface in an unambiguous manner.
        ///     In that case, the name of the actor service must be configured using this property of the
        ///     <see cref="ActorServiceAttribute"/>.
        ///     </para>
        /// </remarks>
        /// <value>The name of the actor service relative to the application name.</value>
        ///
        public string Name { get; set; }

        internal static ActorServiceAttribute Get(Type actorImplementationType)
        {
            return actorImplementationType.GetTypeInfo().GetCustomAttribute<ActorServiceAttribute>(false);
        }
    }
}
