// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    internal class Util
    {
        unsafe internal static string[] CaptureStringPointers(IntPtr arrayPtr, uint arrayLength)
        {
            IntPtr* stringsPtr = (IntPtr*)arrayPtr;
            string[] capturedStrings = new string[arrayLength];

            for (int i = 0; i < arrayLength; i++)
            {
                capturedStrings[i] = Marshal.PtrToStringUni(stringsPtr[i]);
            }

            return capturedStrings;
        }
    }
}
