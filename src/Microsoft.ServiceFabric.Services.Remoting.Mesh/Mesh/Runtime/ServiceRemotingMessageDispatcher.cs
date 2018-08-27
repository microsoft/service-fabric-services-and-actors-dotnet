// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Base;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Mesh.Builder;

    /// <summary>
    /// Provides an implementation of <see cref="IServiceRemotingMessageHandler"/> that can dispatch
    /// messages to the service implementing <see cref="IService"/> interface.
    /// </summary>
    public class ServiceRemotingMessageDispatcher : IServiceRemotingMessageHandler, IDisposable
    {
        private Base.V2.Runtime.ServiceRemotingMessageDispatcher messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingMessageDispatcher"/> class
        /// that uses the given service context and dispatches messages to the given service implementation.
        /// </summary>
        /// <param name="traceId">traceId</param>
        /// <param name="serviceImplementation">Object that implements the speciifed remoted interfaces.</param>
        /// <param name="serviceRemotingMessageBodyFactory">The factory that will be used by the dispatcher to create response message bodies.</param>
        /// <param name="partitionId">partitionId</param>
        /// <param name="replicaId">replicaId</param>
        public ServiceRemotingMessageDispatcher(
            Guid partitionId,
            long replicaId,
            string traceId,
            object serviceImplementation,
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory = null)
        {
            var serviceTypeInformation = ServiceTypeInformation.Get(serviceImplementation.GetType());

            var allRemotingTypes = new List<Type>();
            foreach (var type in serviceTypeInformation.InterfaceTypes)
            {
                foreach (var baseType in type.GetAllBaseInterfaces())
                {
                    if (!allRemotingTypes.Contains(baseType))
                    {
                        allRemotingTypes.Add(baseType);
                    }
                }
            }

            this.messageDispatcher = new Remoting.Base.V2.Runtime.ServiceRemotingMessageDispatcher(
                partitionId,
                replicaId,
                traceId,
                serviceImplementation,
                this.GetDispatcherMap(serviceImplementation, serviceTypeInformation.InterfaceTypes),
                this.GetKnownTypes,
                false,
                serviceRemotingMessageBodyFactory);
        }

        /// <summary>
        /// Handles a message from the client that requires a response from the service. This Api can be used for the short-circuiting where client is in same process as service.
        /// Client can now directly dispatch request to service instead of using ServiceProxy.
        /// </summary>
        /// <param name="requestMessageDispatchHeaders">Request message headers</param>
        /// <param name="requestMessageBody">Request message body</param>
        /// <param name="cancellationToken">Cancellation token. It can be used to cancel the request</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task is the response for the received request.</returns>
        public virtual Task<IServiceRemotingResponseMessageBody> HandleRequestResponseAsync(
            ServiceRemotingDispatchHeaders requestMessageDispatchHeaders,
            IServiceRemotingRequestMessageBody requestMessageBody,
            CancellationToken cancellationToken)
        {
            return this.messageDispatcher.HandleRequestResponseAsync(requestMessageDispatchHeaders, requestMessageBody, cancellationToken);
        }

        /// <summary>
        /// Handles a message from the client that requires a response from the service.
        /// </summary>
        /// <param name="requestContext">Request context - contains additional information about the request</param>
        /// <param name="requestMessage">Request message</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task is the response for the received request.</returns>
        public virtual async Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            return await this.messageDispatcher.HandleRequestResponseAsync(
                requestContext,
                requestMessage);
        }

        /// <summary>
        /// Handles a one way message from the client.
        /// </summary>
        /// <param name="requestMessage">Request message</param>
        public virtual void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            throw new NotImplementedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    SR.ErrorMethodNotImplemented,
                    this.GetType().Name,
                    "HandleOneWay"));
        }

        /// <summary>
        /// Gets the factory used for creating the remoting response message bodies.
        /// </summary>
        /// <returns>The factory used by this dispatcher for creating the remoting response message bodies.</returns>
        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.messageDispatcher.GetRemotingMessageBodyFactory();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.messageDispatcher.Dispose();
        }

        private Dictionary<int, MethodDispatcherBase> GetDispatcherMap(
            object serviceImplementation,
            IEnumerable<Type> remotedInterfaces)
        {
            var methodDispatcherMap = new Dictionary<int, MethodDispatcherBase>();
            if (serviceImplementation != null)
            {
                foreach (var interfaceType in remotedInterfaces)
                {
                    MethodDispatcherBase methodDispatcher;
                    methodDispatcher =
                        ServiceCodeBuilder.GetOrCreateMethodDispatcherForNonMarkerInterface(interfaceType);
                    methodDispatcherMap.Add(methodDispatcher.InterfaceId, methodDispatcher);
                }
            }

            return methodDispatcherMap;
        }

        private InterfaceDetails GetKnownTypes(string interfaceName)
        {
            InterfaceDetails details = null;
            ServiceCodeBuilder.TryGetKnownTypes(
                interfaceName,
                out details);
            return details;
        }
    }
}
