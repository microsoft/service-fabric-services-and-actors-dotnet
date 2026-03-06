using System;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Tests.Metrics.Implementation
{
    internal class Util
    {
        unsafe internal static string[] CaputreStringPointers(IntPtr arrayPtr, uint arrayLength)
        {
            IntPtr* stringsPtr = (IntPtr*)arrayPtr;
            string[] capuredStrings = new string[arrayLength];

            for (int i = 0; i < arrayLength; i++)
            {
                capuredStrings[i] = Marshal.PtrToStringUni(stringsPtr[i]);
            }

            return capuredStrings;
        }
    }
}
