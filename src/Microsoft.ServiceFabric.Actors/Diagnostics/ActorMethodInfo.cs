// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Reflection;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class ActorMethodInfo
    {
        internal ActorMethodInfo() { }

        internal ActorMethodInfo(MethodInfo methodInfo)
        {
            MethodName = string.Concat(methodInfo.DeclaringType.Name, ".", methodInfo.Name);
            MethodSignature = methodInfo.ToString();
        }

        internal string MethodName { get; set; }

        internal string MethodSignature { get; set; }
    }
}
