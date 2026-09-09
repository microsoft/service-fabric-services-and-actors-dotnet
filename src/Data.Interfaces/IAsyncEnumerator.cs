// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Supports asynchronous iteration over a sequence of values of type <typeparamref name="T"/>.
    /// </summary>
    public interface IAsyncEnumerator<out T> : IDisposable
    {
        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <remarks>
        /// The value is undefined when the enumerator is positioned before the first element — after creation
        /// or <see cref="Reset"/> — and after <see cref="MoveNextAsync(CancellationToken)"/> completes with
        /// <see langword="false"/>.
        /// </remarks>
        T Current { get; }

        /// <summary>
        /// Asynchronously returns a value that indicates whether the enumerator was successfully advanced to the next element
        /// of the sequence.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator was advanced to the next element; otherwise,
        /// <see langword="false"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
        Task<bool> MoveNextAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the enumerator to its initial position, before the first element of the sequence.
        /// </summary>
        /// <exception cref="NotSupportedException">Resetting the enumerator is not supported.</exception>
        void Reset();
    }
}
