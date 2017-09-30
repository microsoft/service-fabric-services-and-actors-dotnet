// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.Diagnostics;
    using System.Fabric.Common;

    internal class PooledBuffer : IPooledBuffer
    {
        private readonly IBufferPoolManager manager;
        private bool isRelease;

        public PooledBuffer(IBufferPoolManager manager, ArraySegment<byte> segment, int lengthUsed)
        {
            this.manager = manager;
            this.Value = segment;
            this.ContentLength = lengthUsed;
            this.isRelease = false;
        }

        public void ResetBuffer()
        {
            this.isRelease = false;
            this.ContentLength = 0;
        }
        public ArraySegment<byte> Value { get; set; }

        public int ContentLength { get; set; }

        public bool Release()
        {
            if (!this.isRelease)
            {
                this.isRelease = true;
                this.ContentLength = 0;
                if (this.manager != null)
                {
                 return   this.manager.ReturnBuffer(this);
                }
            }
            return false;
        }

        ~PooledBuffer()
        {
            Debug.Assert(this.isRelease, "Pooled Buffer has not been released ");
        }
    }
}