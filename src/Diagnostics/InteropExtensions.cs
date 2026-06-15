// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics;

static unsafe class Interop
{
    internal static void Free(GCHandle* handles, int count)
    {
        for (int i = 0; i < count; i++)
            if (handles[i].IsAllocated)
                handles[i].Free();
    }
}
