// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class dispatches requests from the client to the interface/method of the remoted object.
    /// This class is used by remoting code generator.
    /// </summary>
    public abstract class MethodDispatcherBase
    {
        private int interfaceId;
        private IReadOnlyDictionary<int, string> methodNameMap;

        /// <summary>
        /// Gets the id of the interface supported by this method dispatcher.
        /// </summary>
        public int InterfaceId
        {
            get { return this.interfaceId; }
        }

        /// <summary>
        /// This method is used to dispatch request to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation">The object impplemented the remoted interface.</param>
        /// <param name="methodId">Id of the method to which to dispatch the request to.</param>
        /// <param name="requestBody">The body of the request object that needs to be dispatched to the object.</param>
        /// <param name="cancellationToken">The cancellation token that will be signaled if this operation is cancelled.</param>
        /// <returns>A task that represents the outstanding asynchronous call to the implementation object.
        /// The return value of the task contains the returned value from the invoked method.</returns>
        public abstract Task<object> DispatchAsync(
            object objectImplementation,
            int methodId,
            object requestBody,
            CancellationToken cancellationToken);

        /// <summary>
        /// This method is used to dispatch one way messages to the specified methodId of the interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation">The object impplemented the remoted interface.</param>
        /// <param name="methodId">Id of the method to which to dispatch the request to.</param>
        /// <param name="messageBody">The body of the one-way message that needs to be dispatched to the object.</param>
        public abstract void Dispatch(object objectImplementation, int methodId, object messageBody);

        /// <summary>
        /// Gets the name of the method that has the specified methodId.
        /// </summary>
        /// <param name="methodId">The id of the method.</param>
        /// <returns>The name of the method corresponding to the specified method id.</returns>
        public string GetMethodName(int methodId)
        {
            return this.methodNameMap[methodId];
        }

        internal void SetInterfaceId(int interfaceId)
        {
            this.interfaceId = interfaceId;
        }

        internal void SetMethodNameMap(IReadOnlyDictionary<int, string> methodNameMap)
        {
            this.methodNameMap = methodNameMap;
        }

        /// <summary>
        /// Internal - used by Service remoting
        /// </summary>
        /// <param name="task">continuation task</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected Task<object> ContinueWith(Task task)
        {
            return task.ContinueWith<object>(
                t =>
                {
                    t.GetAwaiter().GetResult();
                    return null;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
