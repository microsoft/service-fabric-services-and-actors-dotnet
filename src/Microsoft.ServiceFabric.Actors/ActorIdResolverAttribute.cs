// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents the attributes that allows configuring the properties of the actor service.
    /// The attribute is applied on the actor type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ActorIdResolverAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorIdResolverAttribute"/> class.
        /// </summary>
        public ActorIdResolverAttribute()
        {
        }

        /// <summary>
        /// Gets the ActorIdResolver for actor state migration.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Actors.ActorIdResolverAttribute"/> representing ActorIdResolver to use for the actor state migration.</value>
        internal static ActorIdResolverAttribute Get(Type actorImplementationType)
        {
            return actorImplementationType.GetTypeInfo().GetCustomAttribute<ActorIdResolverAttribute>(false);
        }
    }
}
