// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;

    internal class Allocator
    {
        private readonly int segmentSize;

        public Allocator(int segmentSize)
        {
            this.segmentSize = segmentSize;
        }

        public ArraySegment<byte> GetSegment()
        {
            var arraySegment = new ArraySegment<byte>(new byte[this.segmentSize]);

            return arraySegment;
        }
    }
}