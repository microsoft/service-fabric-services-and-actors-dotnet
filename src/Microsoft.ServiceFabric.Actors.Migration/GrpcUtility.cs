// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Reflection;
    using Google.Protobuf;

    internal static class GrpcUtility
    {
        private const string TraceType = "GrpcUtility";

        // Static method to do zero-copy byte[] to ByteString conversion
        // This is internal to GRPC since can be easily used improperly,
        // but this is exactly what we need for our use case
        private static readonly MethodInfo AttachBytesMethodInfo = null;

        static GrpcUtility()
        {
                var type = typeof(ByteString);
                AttachBytesMethodInfo = type.GetMethod(
                    "AttachBytes",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                    null,
                    new Type[] { typeof(byte[]) },
                    null);
        }

        internal static ByteString ZeroCopyByteString(byte[] data)
        {
            return (ByteString)(AttachBytesMethodInfo.Invoke(null, new object[] { data }));
        }
    }
}
