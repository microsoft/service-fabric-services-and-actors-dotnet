// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Common
{
    using System;
    using System.Reflection;

    internal static class IdUtil
    {
        internal static int ComputeId(MethodInfo methodInfo)
        {
            var hash = methodInfo.Name.GetHashCode();

            if (methodInfo.DeclaringType != null)
            {
                if (methodInfo.DeclaringType.Namespace != null)
                {
                    hash = HashCombine(methodInfo.DeclaringType.Namespace.GetHashCode(), hash);
                }

                hash = HashCombine(methodInfo.DeclaringType.Name.GetHashCode(), hash);
            }

            return hash;
        }

        internal static int ComputeId(Type type)
        {
            var hash = type.Name.GetHashCode();
            if (type.Namespace != null)
            {
                hash = HashCombine(type.Namespace.GetHashCode(), hash);
            }

            return hash;
        }

        internal static int ComputeId(string typeName, string typeNamespace)
        {
            var hash = typeName.GetHashCode();
            if (typeNamespace != null)
            {
                hash = HashCombine(typeNamespace.GetHashCode(), hash);
            }

            return hash;
        }

        /// <summary>
        /// This is how VB Anonymous Types combine hash values for fields.
        /// </summary>
        internal static int HashCombine(int newKey, int currentKey)
        {
            return unchecked((currentKey*(int) 0xA5555529) + newKey);
        }
    }
}