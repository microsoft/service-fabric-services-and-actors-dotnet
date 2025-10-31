// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class ActorMethodInfo
    {
        readonly string methodName;
        readonly string methodSignature;

        internal ActorMethodInfo(string methodName, string methodSignature)
        {
            this.methodName = methodName;
            this.methodSignature = methodSignature;
        }

        internal string MethodName => methodName;

        internal string MethodSignature => methodSignature;
    }
}
