// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class Util
    {
        public static long GetInterfaceMethodKey(uint interfaceId, uint methodId)
        {
            var key = (ulong)methodId;
            key = key | (ulong)interfaceId << 32;
            return (long)key;
        }
    }
}
