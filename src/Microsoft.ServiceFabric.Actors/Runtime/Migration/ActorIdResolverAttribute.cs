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
    /// Represents the class is an implementation of <see cref="Microsoft.ServiceFabric.Actors.Runtime.Migration.IActorIdResolver"/>. This implementation will be used to resolve ambigious actor storage keys.
    /// There can be more than one class annotated with this attribute. In that case, all the resolvers will be tried in order.
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

        internal static IEnumerable<Type> GetTypesWithAttribute()
        {
            // TODO: Is checking entry assembly sufficient?
            var assembly = Assembly.GetEntryAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(ActorIdResolverAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
