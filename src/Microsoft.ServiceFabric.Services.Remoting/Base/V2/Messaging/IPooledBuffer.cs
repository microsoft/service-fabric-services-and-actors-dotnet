// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging
{
    using System;

    /// <summary>
    /// Defines the interface that represents the IPooledBuffer class.
    /// </summary>
    public interface IPooledBuffer
    {
        /// <summary>
        /// Gets the Value of the buffer.
        /// </summary>
        ArraySegment<byte> Value { get; }

        /// <summary>
        /// Gets or sets the length of the buffer used.
        /// </summary>
        int ContentLength { get; set; }

        /// <summary>
        /// Release the buffer to the Pool.
        /// </summary>
        /// <returns>true if it has successfully released buffer to the pool , false otherwise.</returns>
        bool Release();
    }
}
