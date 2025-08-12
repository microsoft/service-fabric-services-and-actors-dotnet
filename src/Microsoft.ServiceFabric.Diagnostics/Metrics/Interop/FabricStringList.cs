using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FabricStringList
    {
        public uint Count;

        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
        public string[] Items;
    }
}
