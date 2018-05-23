// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    /// <summary>
    /// This class dispatches requests from the client to the interface/method of the remoted object.
    /// This class is used by remoting code generator.
    /// </summary>
    public abstract class MethodDispatcherBase : Remoting.Builder.MethodDispatcherBase
    {
        /// <summary>
        /// Why we pass IServiceRemotingMessageBodyFactory to this function instead of
        /// setting at class level?. Since we cache MethodDispatcher for each interface,
        /// we can't set IServiceRemotingMessageBodyFactory at class level.
        /// These can be cases where multiple IServiceRemotingMessageBodyFactory implmenetation but single dispatcher class.
        /// This method is used to dispatch request to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation">The object impplemented the remoted interface.</param>
        /// <param name="methodId">Id of the method to which to dispatch the request to.</param>
        /// <param name="requestBody">The body of the request object that needs to be dispatched to the object.</param>
        /// <param name="remotingMessageBodyFactory">IServiceRemotingMessageBodyFactory implementaion</param>
        /// <param name="cancellationToken">The cancellation token that will be signaled if this operation is cancelled.</param>
        /// <returns>A task that represents the outstanding asynchronous call to the implementation object.
        /// The return value of the task contains the returned value from the invoked method.</returns>
        public Task<IServiceRemotingResponseMessageBody> DispatchAsync(
            object objectImplementation,
            int methodId,
            IServiceRemotingRequestMessageBody requestBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dispatchTask = this.OnDispatchAsync(
                methodId,
                objectImplementation,
                requestBody,
                remotingMessageBodyFactory,
                cancellationToken);

            return dispatchTask;
        }

        /// <summary>
        /// This method is used to dispatch one way messages to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation">The object impplemented the remoted interface.</param>
        /// <param name="methodId">Id of the method to which to dispatch the request to.</param>
        /// <param name="requestMessageBody">The body of the request object that needs to be dispatched to the remoting implementation.</param>
        public void Dispatch(object objectImplementation, int methodId, IServiceRemotingRequestMessageBody requestMessageBody)
        {
            this.OnDispatch(methodId, objectImplementation, requestMessageBody);
        }

        // Needed as this class inheriting from MethodDispatcherBase

        /// <inheritdoc />
        public override Task<object> DispatchAsync(
            object objectImplementation,
            int methodId,
            object requestBody,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Needed as this class inheriting from MethodDispatcherBase

        /// <inheritdoc />
        public override void Dispatch(object objectImplementation, int methodId, object messageBody)
        {
            throw new NotImplementedException();
        }

        internal void Initialize(InterfaceDescription description, IReadOnlyDictionary<int, string> methodMap)
        {
            this.SetInterfaceId(description.Id);
            this.SetMethodNameMap(methodMap);
        }

        /// <summary>
        /// This method is used ti create the remoting response from the specified return value
        /// </summary>
        /// <param name="interfaceName">Interface Name of the remoting Interface</param>
        /// <param name="methodName">Method Name of the remoting method</param>
        /// <param name="methodId">MethodId of the remoting method</param>
        /// <param name="remotingMessageBodyFactory">MessageFactory for the remoting Interface.</param>
        /// <param name="response">Response returned by remoting method</param>
        /// <returns>Remoting Response</returns>
        protected IServiceRemotingResponseMessageBody CreateResponseMessageBody(
            string interfaceName,
            string methodName,
            int methodId,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            object response)
        {
            var msg = remotingMessageBodyFactory.CreateResponse(
                interfaceName,
                methodName,
                this.CreateWrappedResponseBody(methodId, response));

            if (!(msg is WrappedMessage))
            {
                msg.Set(response);
            }

            return msg;
        }

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch request to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="methodId">Id of the method.</param>
        /// <param name="remotedObject">The remoted object instance.</param>
        /// <param name="requestBody">Request body</param>
        /// <param name="remotingMessageBodyFactory">Remoting Message Body Factory implementation needed for creating response object.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// The result of the task is the return value from the method.
        /// </returns>
        protected abstract Task<IServiceRemotingResponseMessageBody> OnDispatchAsync(
                    int methodId,
                    object remotedObject,
                    IServiceRemotingRequestMessageBody requestBody,
                    IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
                    CancellationToken cancellationToken);

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch one way messages to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="methodId">Id of the method.</param>
        /// <param name="remotedObject">The remoted object instance.</param>
        /// <param name="requestBody">Request body</param>
        protected abstract void OnDispatch(int methodId, object remotedObject, IServiceRemotingRequestMessageBody requestBody);

        /// <summary>
        /// Internal - used by Service remoting
        /// </summary>
        /// <param name="interfaceName">Interface Name of the remoting Interface</param>
        /// <param name="methodName">Method Name of the remoting method</param>
        /// <param name="methodId">MethodId of the remoting method</param>
        /// <param name="remotingMessageBodyFactory">MessageFactory for the remoting Interface.</param>
        /// <param name="task">continuation task</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        /// <typeparam name="TRetVal">The response type for the remoting method.</typeparam>
        protected Task<IServiceRemotingResponseMessageBody> ContinueWithResult<TRetVal>(
            string interfaceName,
            string methodName,
            int methodId,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            Task<TRetVal> task)
        {
            return task.ContinueWith(
                t => this.CreateResponseMessageBody(interfaceName, methodName, methodId, remotingMessageBodyFactory, t.GetAwaiter().GetResult()),
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// Internal - used by Service remoting
        /// <summary>
        /// This checks if we are wrapping remoting message or not.
        /// </summary>
        /// <param name="requestMessage">Remoting Request Message</param>
        /// <returns>true or false</returns>
        protected bool CheckIfItsWrappedRequest(IServiceRemotingRequestMessageBody requestMessage)
        {
            if (requestMessage is WrappedMessage)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates Wrapped Response Object for a method
        /// </summary>
        /// <param name="methodId">MethodId of the remoting method.</param>
        /// <param name="retVal">Response for a method</param>
        /// <returns>Wrapped Ressponse object</returns>
        // Generated By Code-gen
        protected abstract object CreateWrappedResponseBody(
            int methodId,
            object retVal);
    }
}
