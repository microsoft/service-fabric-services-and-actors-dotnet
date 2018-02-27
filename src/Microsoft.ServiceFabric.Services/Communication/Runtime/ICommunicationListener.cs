// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Defines the base interface and the state machine contract for the communication listener
    ///     for a Service Fabric Service.
    /// </summary>
    public interface ICommunicationListener
    {
        /// <summary>
        ///     This method causes the communication listener to be opened. Once the Open
        ///     completes, the communication listener becomes usable - accepts and sends messages.
        /// </summary>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the endpoint string.
        /// </returns>
        Task<string> OpenAsync(CancellationToken cancellationToken);

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method allows the communication listener to transition to this state in a
        /// graceful manner.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        Task CloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// This method causes the communication listener to close. Close is a terminal state and
        /// this method causes the transition to close ungracefully. Any outstanding operations
        /// (including close) should be canceled when this method is called.
        /// </summary>
        void Abort();
    }
}
