// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    /// <summary>
    /// The class dispatches the requests from the client to the interface/method of the remoted objectts.
    /// This class is used by remoting code generator. This class caches the Serializer.
    /// </summary>
    public abstract class MethodDispatcherBaseWithSerializer : MethodDispatcherBase
    {
        private DataContractSerializer requestBodySerializer;
        private DataContractSerializer responseBodySerializer;

        internal void SetRequestKnownTypes(IEnumerable<Type> requestBodyTypes, IEnumerable<Type> responseBodyTypes)
        {
            this.requestBodySerializer = this.CreateRequestMessageBodySerializer(requestBodyTypes);
            this.responseBodySerializer = this.CreateResponseMessageBodySerializer(responseBodyTypes);
        }

        internal void Initialize(InterfaceDescription description, IReadOnlyDictionary<int, string> methodMap, IEnumerable<Type> requestBodyTypes, IEnumerable<Type> responseBodyTypes)
        {
            this.SetInterfaceId(description.Id);
            this.SetMethodNameMap(methodMap);
            this.SetRequestKnownTypes(requestBodyTypes, responseBodyTypes);
        }



        /// <summary>
        ///This method is used to dispatch request to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<object> DispatchAsync(object objectImplementation, int methodId, object requestBody, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dispatchTask = this.OnDispatchAsync(
                methodId,
                objectImplementation,
                (requestBody == null) ? null : this.GetRequestMessageBodyValue(requestBody),
                cancellationToken);

            return dispatchTask.ContinueWith(
                t =>
                {
                    var retval = t.GetAwaiter().GetResult();
                    return (retval == null) ? null : this.CreateResponseMessageBody(retval);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// This method is used to dispatch one way messages to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="messageBody"></param>
        public override void Dispatch(object objectImplementation, int methodId, object messageBody)
        {
            this.OnDispatch(methodId, objectImplementation, (messageBody == null) ? null : this.GetRequestMessageBodyValue(messageBody));
        }

        /// <summary>
        /// Internal - used by Service remoting
        /// </summary>
        /// <typeparam name="TRetval">Return value</typeparam>
        /// <param name="methodId">method id</param>
        /// <param name="task">continuation task</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected Task<object> ContinueWithResult<TRetval>(int methodId, Task<TRetval> task)
        {
            return task.ContinueWith(t => this.CreateResponseBody(methodId, t.GetAwaiter().GetResult()), TaskContinuationOptions.ExecuteSynchronously);
        }


        /// <summary>
        /// This method is implemented by the generated method dispatcher to create the response from the specified return value 
        /// as a result of dispatching the method to the remoted object. 
        /// </summary>
        /// <param name="methodId">Id of the method.</param>
        /// <param name="retval">The returned value from the method.</param>
        /// <returns>A <see cref="System.Object">Object</see> that represents the response body to be sent back to the client.</returns>
        protected abstract object CreateResponseBody(int methodId, object retval);



        internal object DeserializeRequestMessageBody(byte[] requestMsgBodyBytes)
        {
            return SerializationUtility.Deserialize(this.requestBodySerializer, requestMsgBodyBytes);
        }

        internal byte[] SerializeResponseMessageBody(object responseMsgBody)
        {
            return SerializationUtility.Serialize(this.responseBodySerializer, responseMsgBody);
        }

        internal abstract DataContractSerializer CreateRequestMessageBodySerializer(IEnumerable<Type> requestBodyValueTypes);

        internal abstract DataContractSerializer CreateResponseMessageBodySerializer(IEnumerable<Type> responseBodyValueTypes);

        internal abstract object CreateResponseMessageBody(object responseMessageBodyValue);

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch request to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="methodId">Id of the method.</param>
        /// <param name="remotedObject">The remoted object instance.</param>
        /// <param name="requestBody">Request body</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. 
        /// The result of the task is the return value from the method.
        /// </returns>
        protected abstract Task<object> OnDispatchAsync(int methodId, object remotedObject, object requestBody, CancellationToken cancellationToken);

        /// <summary>
        /// This method is implemented by the generated method dispatcher to dispatch one way messages to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="methodId">Id of the method.</param>
        /// <param name="remotedObject">The remoted object instance.</param>
        /// <param name="messageBody">message body</param>
        protected abstract void OnDispatch(int methodId, object remotedObject, object messageBody);

        internal abstract object GetRequestMessageBodyValue(object requestMessageBody);
    }
}
