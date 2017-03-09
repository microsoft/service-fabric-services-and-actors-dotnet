// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    /// <summary>
    /// Provides an implementation of <see cref="IServiceRemotingMessageHandler"/> that can dispatch
    /// messages to the service implementing <see cref="IService"/> interface.
    /// </summary>
    public class ServiceRemotingDispatcher : IServiceRemotingMessageHandler
    {
        private readonly IService service;
        private readonly ServiceRemotingCancellationHelper cancellationHelper;
        private readonly Dictionary<int, ServiceMethodDispatcherBase> methodDispatcherMap;

        /// <summary>
        /// Instantiates the ServiceRemotingDispatcher that uses the given service context and
        /// dispatches messages to the given service implementation.
        /// </summary>
        /// <param name="serviceContext">Service context</param>
        /// <param name="service">Service implementation that implements interfaces of type <see cref="IService"/></param>
        public ServiceRemotingDispatcher(ServiceContext serviceContext, IService service)
        {
            Requires.ThrowIfNull(serviceContext, "serviceContext");

            this.cancellationHelper = new ServiceRemotingCancellationHelper(serviceContext.TraceId);

            this.methodDispatcherMap = new Dictionary<int, ServiceMethodDispatcherBase>();
            this.service = service;

            if (service != null)
            {
                var serviceTypeInformation = ServiceTypeInformation.Get(service.GetType());
                foreach (var interfaceType in serviceTypeInformation.InterfaceTypes)
                {
                    var methodDispatcher = ServiceCodeBuilder.GetOrCreateMethodDispatcher(interfaceType);
                    this.methodDispatcherMap.Add(methodDispatcher.InterfaceId, methodDispatcher);
                }
            }
        }

        /// <summary>
        /// Handles a message from the client that requires a response from the service.
        /// </summary>
        /// <param name="requestContext">Request context - contains additional information about the request</param>
        /// <param name="messageHeaders">Request message headers</param>
        /// <param name="requestBody">Request message body</param>
        /// <returns>Response body</returns>
        public virtual Task<byte[]> RequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            ServiceRemotingMessageHeaders messageHeaders,
            byte[] requestBody)
        {
            if (this.cancellationHelper.IsCancellationRequest(messageHeaders))
            {
                return this.cancellationHelper.CancelRequestAsync(messageHeaders.InterfaceId, messageHeaders.MethodId, messageHeaders.InvocationId);
            }
            else
            {
                return this.cancellationHelper.DispatchRequest(
                    messageHeaders.InterfaceId,
                    messageHeaders.MethodId,
                    messageHeaders.InvocationId,
                    cancellationToken => this.OnDispatch(messageHeaders, requestBody, cancellationToken));
            }
        }

        /// <summary>
        /// Handles a one way message from the client.
        /// </summary>
        /// <param name="requestContext">Request context - contains additional information about the request</param>
        /// <param name="messageHeaders">Request message headers</param>
        /// <param name="requestBody">Request message body</param>
        public virtual void HandleOneWay(IServiceRemotingRequestContext requestContext, ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, SR.ErrorMethodNotImplemented, this.GetType().Name, "HandleOneWay"));
        }

        private Task<byte[]> OnDispatch(ServiceRemotingMessageHeaders headers, byte[] requestBodyBytes, CancellationToken cancellationToken)
        {
            ServiceMethodDispatcherBase methodDispatcher;
            if (!this.methodDispatcherMap.TryGetValue(headers.InterfaceId, out methodDispatcher))
            {
                throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, SR.ErrorInterfaceNotImplemented, headers.InterfaceId, this.service));
            }

            var requestBody = methodDispatcher.DeserializeRequestMessageBody(requestBodyBytes);
            var dispatchTask = methodDispatcher.DispatchAsync(this.service, headers.MethodId, requestBody, cancellationToken);

            return dispatchTask.ContinueWith(
                t =>
                {
                    var responseBody = t.GetAwaiter().GetResult();
                    return methodDispatcher.SerializeResponseMessageBody(responseBody);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
