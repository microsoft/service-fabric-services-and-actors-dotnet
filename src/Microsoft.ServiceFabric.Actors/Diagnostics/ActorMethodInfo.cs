// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class ActorMethodInfo
    {
        internal readonly string methodName;
        internal readonly string methodSignature;

        internal ActorMethodInfo(string methodName, string methodSignature)
        {
            this.methodName = methodName;
            this.methodSignature = methodSignature;
        }
    }
}
