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

        internal void Initialize(InterfaceDescription description, IReadOnlyDictionary<int, string> methodMap)
        {
            this.SetInterfaceId(description.Id);
            this.SetMethodNameMap(methodMap);
        }

        /// <summary>
        /// This method is used ti create the remoting response from the specified return value
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <param name="remotingMessageBodyFactory"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        protected IServiceRemotingResponseMessageBody CreateResponseMessageBody(string interfaceName, string methodName, IServiceRemotingMessageBodyFactory remotingMessageBodyFactory, object response)
        {
            var msg = remotingMessageBodyFactory.CreateResponse(interfaceName, methodName);
            msg.Set(response);
            return msg;
        }


        ////Why we pass IServiceRemotingMessageBodyFactory to this function instead of
        /// setting at class level?. Since we cache MethodDispatcher for each interface ,
        /// we can't set IServiceRemotingMessageBodyFactory at class level .
        /// These can be cases where multiple IServiceRemotingMessageBodyFactory implmenetation
        ///  but single dispatcher class .
        /// <summary>
        ///This method is used to dispatch request to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="requestBody"></param>
        /// <param name="remotingMessageBodyFactory"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IServiceRemotingResponseMessageBody> DispatchAsync(object objectImplementation, int methodId,
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
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="requestMessageBody"></param>
        public void Dispatch(object objectImplementation, int methodId, IServiceRemotingRequestMessageBody requestMessageBody)
        {
            this.OnDispatch(methodId, objectImplementation, requestMessageBody);
        }

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch request to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        protected abstract Task<IServiceRemotingResponseMessageBody> OnDispatchAsync(int methodId, object remotedObject, IServiceRemotingRequestMessageBody requestBody, IServiceRemotingMessageBodyFactory remotingMessageBodyFactory, CancellationToken cancellationToken);

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch one way messages to the specified methodId of the
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="remotedObject"></param>
        /// <param name="requestBody"></param>
        protected abstract void OnDispatch(int methodId, object remotedObject, IServiceRemotingRequestMessageBody requestBody);


        //Needed as this class inheriting from MethodDispatcherBase
        /// <inheritdoc />
        public override Task<object> DispatchAsync(object objectImplementation, int methodId, object requestBody,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //Needed as this class inheriting from MethodDispatcherBase
        /// <inheritdoc />
        public override void Dispatch(object objectImplementation, int methodId, object messageBody)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal - used by Service remoting
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="remotingMessageBodyFactory"></param>
        /// <param name="task">continuation task</param>
        /// <param name="interfaceName"></param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected Task<IServiceRemotingResponseMessageBody> ContinueWithResult<TRetVal>(string interfaceName, string methodName,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            Task<TRetVal> task)
        {
            return task.ContinueWith(
                t => this.CreateResponseMessageBody(interfaceName, methodName, remotingMessageBodyFactory, t.GetAwaiter().GetResult()),
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
