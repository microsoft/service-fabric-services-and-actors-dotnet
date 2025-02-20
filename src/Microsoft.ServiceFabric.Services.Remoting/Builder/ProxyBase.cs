// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// The base class used by remoting code generator to generate the proxy for the remoted interfaces.
    /// </summary>
    public abstract class ProxyBase
    {
        private IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBase"/> class.
        /// </summary>
        protected ProxyBase()
        {
        }

        internal IServiceRemotingMessageBodyFactory ServiceRemotingMessageBodyFactory
        {
            get
            {
                return this.serviceRemotingMessageBodyFactory;
            }

            set
            {
                this.serviceRemotingMessageBodyFactory = value;
            }
        }

#if !DotNetCoreClr
        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal abstract DataContractSerializer GetRequestMessageBodySerializer(int interfaceId);

        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal abstract DataContractSerializer GetResponseMessageBodySerializer(int interfaceId);

        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal abstract object GetResponseMessageBodyValue(object responseMessageBody);

        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal abstract object CreateRequestMessageBody(object requestMessageBodyValue);

        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use InvokeAsyncImplV2() instead.")]
        internal abstract Task<byte[]> InvokeAsync(int interfaceId, int methodId, byte[] requestMsgBodyBytes, CancellationToken cancellationToken);

        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use V2 implementation instead.")]
        internal abstract void Invoke(int interfaceId, int methodId, byte[] requestMsgBodyBytes);
#endif

        // V2 Stack Internal Api
        internal void InitializeV2(
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory)
        {
            this.ServiceRemotingMessageBodyFactory = serviceRemotingMessageBodyFactory;
        }

        internal abstract void InvokeImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue);

        internal abstract Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken);

#if !DotNetCoreClr
        /// <summary>
        /// Called by the generated proxy class to send the message to the remote object.
        /// </summary>
        /// <param name="interfaceId">Id of the remote interface.</param>
        /// <param name="methodId">Id of the remote method to be invokved.</param>
        /// <param name="requestMsgBodyValue">Message body to be sent to remote object.</param>
        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use V2 implementation instead.")]
        protected void Invoke(
            int interfaceId,
            int methodId,
            object requestMsgBodyValue)
        {
            object requestMsgBody = null;
            if (requestMsgBodyValue != null)
            {
                requestMsgBody = this.CreateRequestMessageBody(requestMsgBodyValue);
            }

            var requestMsgBodyBytes = SerializationUtility.Serialize(
                this.GetRequestMessageBodySerializer(interfaceId),
                requestMsgBody);

            this.Invoke(interfaceId, methodId, requestMsgBodyBytes);
        }

        /// <summary>
        /// Called by the generated proxy class to send the request to the remote object and get the response back.
        /// </summary>
        /// <param name="interfaceId">Id of the remote interface.</param>
        /// <param name="methodId">Id of the remote method to be invokved.</param>
        /// <param name="requestMsgBodyValue">Request body.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A task that represents the asynchronous operation async call to remote object.</returns>
        [Obsolete("This method is part of the deprecated V1 service remoting stack. Use InvokeAsyncImplV2() instead.")]
        protected async Task<object> InvokeAsync(
            int interfaceId,
            int methodId,
            object requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            object requestMsgBody = null;
            if (requestMsgBodyValue != null)
            {
                requestMsgBody = this.CreateRequestMessageBody(requestMsgBodyValue);
            }

            var requestMsgBodyBytes = SerializationUtility.Serialize(
                this.GetRequestMessageBodySerializer(interfaceId),
                requestMsgBody);

            var responseMsgBodyBytes = await this.InvokeAsync(
                interfaceId,
                methodId,
                requestMsgBodyBytes,
                cancellationToken);

            var responseMsgBody = SerializationUtility.Deserialize(
                this.GetResponseMessageBodySerializer(interfaceId),
                responseMsgBodyBytes);

            return responseMsgBody != null ? this.GetResponseMessageBodyValue(responseMsgBody) : null;
        }

        /// <summary>
        /// Called by the generated proxy class to get the result from the response body.
        /// </summary>
        /// <typeparam name="TRetval"><see cref="System.Type"/> of the remote method return value.</typeparam>
        /// <param name="interfaceId">Interface Id for the remoted interface.</param>
        /// <param name="methodId">Method Id for the remote method.</param>
        /// <param name="task">A task that represents the asynchronous operation for remote method call.</param>
        /// <returns>A task that represents the asynchronous operation for remote method call.
        /// The value of the TRetval contains the remote method return value. </returns>
        protected async Task<TRetval> ContinueWithResult<TRetval>(
            int interfaceId,
            int methodId,
            Task<object> task)
        {
            var responseBody = await task;
            return (TRetval)this.GetReturnValue(interfaceId, methodId, responseBody);
        }

#endif

        // V2 Stack protected APIs

        /// <summary>
        /// This method is used by the generated proxy type and should be used directly. This method converts the Task with object
        /// return value to a Task without the return value for the void method invocation
        /// </summary>
        /// <param name="interfaceId">Interface Id for the actor interface.</param>
        /// <param name="methodId">Method Id for the actor method.</param>
        /// <param name="responseBody">Response body.</param>
        /// <returns>Return value of method call as <see cref="object"/>.</returns>
        protected abstract object GetReturnValue(int interfaceId, int methodId, object responseBody);

        /// <summary>
        /// This method is used by the generated proxy type and should be used directly. This method converts the Task with object
        /// return value to a Task without the return value for the void method invocation.
        /// </summary>
        /// <param name="task">A task returned from the method that contains null return value.</param>
        /// <returns>A task that represents the asynchronous operation for remote method call without the return value.</returns>
        protected Task ContinueWith(Task<object> task)
        {
            return task;
        }

        /// <summary>
        /// Creates the Remoting request message Body
        /// </summary>
        /// <param name="interfaceName">Full Name of the service interface for which this call is invoked</param>
        /// <param name="methodName">Method Name of the service interface for which this call is invoked</param>
        /// <param name="parameterCount">Number of Parameters in the service interface Method</param>
        /// <param name="wrappedRequest">Wrapped Request Object</param>
        /// <returns>A request message body for V2 remoting stack.</returns>
        protected virtual IServiceRemotingRequestMessageBody CreateRequestMessageBodyV2(
            string interfaceName,
            string methodName,
            int parameterCount,
            object wrappedRequest)
        {
            return this.ServiceRemotingMessageBodyFactory.CreateRequest(interfaceName, methodName, parameterCount, wrappedRequest);
        }

        /// <summary>
        /// Called by the generated proxy class to get the result from the response body.
        /// </summary>
        /// <typeparam name="TRetval"><see cref="System.Type"/> of the remote method return value.</typeparam>
        /// <param name="interfaceId">InterfaceId of the remoting interface.</param>
        /// <param name="methodId">MethodId of the remoting Method</param>
        /// <param name="task">A task that represents the asynchronous operation for remote method call.</param>
        /// <returns>A task that represents the asynchronous operation for remote method call.
        /// The value of the TRetval contains the remote method return value. </returns>
        protected async Task<TRetval> ContinueWithResultV2<TRetval>(
            int interfaceId,
            int methodId,
            Task<IServiceRemotingResponseMessageBody> task)
        {
            var responseBody = await task;
            var wrappedMessage = responseBody as WrappedMessage;
            if (wrappedMessage != null)
            {
                return (TRetval)this.GetReturnValue(
                    interfaceId,
                    methodId,
                    wrappedMessage.Value);
            }

            return (TRetval)responseBody.Get(typeof(TRetval));
        }

        /// <summary>
        /// Called by the generated proxy class to send the request to the remote object and get the response back.
        /// </summary>
        /// <param name="interfaceId">Id of the remote interface.</param>
        /// <param name="methodId">Id of the remote method to be invokved.</param>
        /// <param name="methodName">Name of the remoting method to be invoked</param>
        /// <param name="requestMsgBodyValue">Request body.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A task that represents the asynchronous operation async call to remote object.</returns>
        protected async Task<IServiceRemotingResponseMessageBody> InvokeAsyncV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            var responseMsg = await this.InvokeAsyncImplV2(
                interfaceId,
                methodId,
                methodName,
                requestMsgBodyValue,
                cancellationToken);

            return responseMsg != null ? responseMsg.GetBody()
                   : null;
        }

        /// <summary>
        /// Called by the generated proxy class to send the requestMessage to the remote object.
        /// </summary>
        /// <param name="interfaceId">Id of the remote interface.</param>
        /// <param name="methodId">Id of the remote method to be invokved.</param>
        /// <param name="requestMsgBodyValue">Message body to be sent to remote object.</param>
        protected void InvokeV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue)
        {
            this.InvokeImplV2(
                interfaceId,
                methodId,
                requestMsgBodyValue);
        }

        // Called By Code-Gen

        /// <summary>
        /// This check if we are wrapping remoting message or not.
        /// </summary>
        /// <param name="requestMessage">Remoting Request Message</param>
        /// <returns>true or false </returns>
        protected bool CheckIfItsWrappedRequest(IServiceRemotingRequestMessageBody requestMessage)
        {
            if (requestMessage is WrappedMessage)
            {
                return true;
            }

            return false;
        }
    }
}
