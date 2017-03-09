// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class ServiceTypeExtensions
    {
        public static Type[] GetServiceInterfaces(this Type serviceType)
        {
            var list = new List<Type>(serviceType.GetInterfaces().Where(t => typeof(IService).IsAssignableFrom(t)));
            list.RemoveAll(t => (t.GetNonServiceParentType() != null));

            return list.ToArray();
        }

        internal static Type GetNonServiceParentType(this Type type)
        {
            var list = new List<Type>(type.GetInterfaces());

            // must have IService as the parent, so removal of it should result in reduction in the count.
            if (list.RemoveAll(t => (t == typeof(IService))) == 0)
            {
                return type;
            }

            foreach (var t in list)
            {
                var nonServiceParent = GetNonServiceParentType(t);
                if (nonServiceParent != null)
                {
                    return nonServiceParent;
                }
            }

            return null;
        }
    }
}