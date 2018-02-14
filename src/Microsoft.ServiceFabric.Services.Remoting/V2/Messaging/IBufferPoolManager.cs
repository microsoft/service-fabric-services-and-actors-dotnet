// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    /// <summary>
    /// Defines the interface that represents the IBufferPoolManager class.
    /// </summary>
    public interface IBufferPoolManager
    {
        /// <summary>
        /// Gets the Buffer from the Pool.
        /// </summary>
        /// <returns></returns>
        IPooledBuffer TakeBuffer();

        /// <summary>
        /// Return the Buffer to the Pool.
        /// </summary>
        /// <param name="buffer"></param>
        bool ReturnBuffer(IPooledBuffer buffer);
    }
}
