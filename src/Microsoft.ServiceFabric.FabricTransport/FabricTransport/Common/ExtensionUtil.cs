// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
#if NET
using System.Runtime.InteropServices.Marshalling;
#endif

namespace Microsoft.ServiceFabric.FabricTransport
{
    internal static class ExtensionUtil
    {
        public static int FinalReleaseComObject(this object obj)
        {
#if NET
            if (!ComWrappers.TryGetComInstance(obj, out IntPtr unknown))
                throw new ArgumentException();
            while(Marshal.Release(unknown) > 0);
            return 0;
#else
            return Marshal.FinalReleaseComObject(obj);
#endif
        }
    }
}
