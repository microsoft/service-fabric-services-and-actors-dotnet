// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Remoting.V2;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace Microsoft.ServiceFabric.Actors.Client
{
    /// <summary>
    /// Provides the base implementation for the proxy to the remote actor objects implementing <see cref="IActor"/> interfaces.
    /// The proxy object can be used used for client-to-actor and actor-to-actor communication.
    /// </summary>
    public abstract class ActorProxy : ProxyBase, IActorProxy
    {
        internal static readonly ActorProxyFactory DefaultProxyFactory = new ActorProxyFactory();
        private Remoting.V2.Client.ActorServicePartitionClient servicePartitionClientV2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProxy"/> class.
        /// </summary>
        protected ActorProxy()
        {
        }

        /// <summary>
        /// Gets <see cref="Actors.ActorId"/> associated with the proxy object.
        /// </summary>
        /// <value><see cref="Actors.ActorId"/> associated with the proxy object.</value>
        public ActorId ActorId
        {
            get { return this.servicePartitionClientV2.ActorId; }
        }

        /// <summary>
        /// Gets the <see cref="Remoting.V2.Client.IActorServicePartitionClient"/> interface that this proxy is using to communicate with the actor.
        /// </summary>
        /// <value><see cref="Remoting.V2.Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.</value>
        public Remoting.V2.Client.IActorServicePartitionClient ActorServicePartitionClientV2
        {
            get { return this.servicePartitionClientV2; }
        }

        /// <summary>
        /// Creates a proxy to the actor object that implements an actor interface.
        /// </summary>
        /// <typeparam name="TActorInterface">
        /// The actor interface implemented by the remote actor object.
        /// The returned proxy object will implement this interface.
        /// </typeparam>
        /// <param name="actorId">The actor ID of the proxy actor object. Methods called on this proxy will result in requests
        /// being sent to the actor with this ID.</param>
        /// <param name="applicationName">
        /// The name of the Service Fabric application that contains the actor service hosting the actor objects.
        /// This parameter can be null if the client is running as part of that same Service Fabric application. For more information, see Remarks.
        /// </param>
        /// <param name="serviceName">
        /// The name of the Service Fabric service as configured by <see cref="ActorServiceAttribute"/> on the actor implementation.
        /// By default, the name of the service is derived from the name of the actor interface. However, <see cref="ActorServiceAttribute"/>
        /// is required when an actor implements more than one actor interface or an actor interface derives from another actor interface since
        /// the service name cannot be determined automatically.
        /// </param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However, it is possible to configure an actor service with more than one listener. This parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        /// <remarks><para>The applicationName parameter can be null if the client is running as part of the same Service Fabric
        /// application as the actor service it intends to communicate with. In this case, the application name is determined from
        /// <see cref="System.Fabric.CodePackageActivationContext"/>, and is obtained by calling the
        /// <see cref="System.Fabric.CodePackageActivationContext.ApplicationName"/> property.</para>
        /// </remarks>
        public static TActorInterface Create<TActorInterface>(
            ActorId actorId,
            string applicationName = null,
            string serviceName = null,
            string listenerName = null)
            where TActorInterface : IActor
        {
            return DefaultProxyFactory.CreateActorProxy<TActorInterface>(
                actorId,
                applicationName,
                serviceName,
                listenerName);
        }

        /// <summary>
        /// Creates a proxy to the actor object that implements an actor interface.
        /// </summary>
        /// <typeparam name="TActorInterface">
        /// The actor interface implemented by the remote actor object.
        /// The returned proxy object will implement this interface.
        /// </typeparam>
        /// <param name="actorId">Actor Id of the proxy actor object. Methods called on this proxy will result in requests
        /// being sent to the actor with this id.</param>
        /// <param name="serviceUri">Uri of the actor service.</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        public static TActorInterface Create<TActorInterface>(
            ActorId actorId,
            Uri serviceUri,
            string listenerName = null)
            where TActorInterface : IActor
        {
            return DefaultProxyFactory.CreateActorProxy<TActorInterface>(serviceUri, actorId, listenerName);
        }

        internal void Initialize(
            Remoting.V2.Client.ActorServicePartitionClient client,
            IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory)
        {
            this.servicePartitionClientV2 = client;
            this.InitializeV2(serviceRemotingMessageBodyFactory);
        }

        internal override void InvokeImplV2(
            int interfaceId,
            int methodId,
            IServiceRemotingRequestMessageBody requestMsgBodyValue)
        {
            // no - op as events/one way messages are not supported for services
        }

        internal override Task<IServiceRemotingResponseMessage> InvokeAsyncImplV2(
            int interfaceId,
            int methodId,
            string methodName,
            IServiceRemotingRequestMessageBody requestMsgBodyValue,
            CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid();
            LogContext.Set(new LogContext
            {
                RequestId = requestId,
            });

            var headers = new ActorRemotingMessageHeaders
            {
                ActorId = this.servicePartitionClientV2.ActorId,
                InterfaceId = interfaceId,
                MethodId = methodId,
                CallContext = Actors.Helper.GetCallContext(),
                MethodName = methodName,
                RequestId = requestId,
            };

            ServiceTrace.Source.WriteInfo(
                "ActorProxy",
                $"Invoking actor proxy - RequestId : {requestId.ToString()}, ActorId : {headers.ActorId}, MethodName : {headers.MethodName}, CallContext : {headers.CallContext}");

            return this.servicePartitionClientV2.InvokeAsync(
                new ServiceRemotingRequestMessage(
                headers,
                requestMsgBodyValue),
                methodName,
                cancellationToken);
        }

        internal async Task SubscribeAsyncV2(Type eventType, object subscriber, TimeSpan resubscriptionInterval)
        {
            var actorId = this.servicePartitionClientV2.ActorId;
            var info = Remoting.V2.Client.ActorEventSubscriberManager.Instance.RegisterSubscriber(
                actorId,
                eventType,
                subscriber);

            Exception error = null;
            try
            {
                await this.servicePartitionClientV2.SubscribeAsync(info.Subscriber.EventId, info.Id);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error != null)
            {
                try
                {
                    await this.UnsubscribeAsyncV2(eventType, subscriber);
                }
                catch
                {
                    // ignore
                }

                throw error;
            }

            this.ResubscribeAsyncV2(info, resubscriptionInterval);
        }

        internal async Task UnsubscribeAsyncV2(Type eventType, object subscriber)
        {
            var actorId = this.servicePartitionClientV2.ActorId;
            if (Remoting.V2.Client.ActorEventSubscriberManager.Instance.TryUnregisterSubscriber(
                actorId,
                eventType,
                subscriber,
                out var info))
            {
                await this.servicePartitionClientV2.UnsubscribeAsync(info.Subscriber.EventId, info.Id);
            }
        }

        #region Event Subscription

        internal Task SubscribeAsync(Type eventType, object subscriber, TimeSpan resubscriptionInterval)
        {
            return this.SubscribeAsyncV2(eventType, subscriber, resubscriptionInterval);
        }

        internal Task UnsubscribeAsync(Type eventType, object subscriber)
        {
            return this.UnsubscribeAsyncV2(eventType, subscriber);
        }

        private void ResubscribeAsyncV2(SubscriptionInfo info, TimeSpan resubscriptionInterval)
        {
#pragma warning disable 4014
            // ReSharper disable once UnusedVariable
            var ignore = Task.Run(
                async () =>
#pragma warning restore 4014
                {
                    while (true)
                    {
                        await Task.Delay(resubscriptionInterval);

                        if (!info.IsActive)
                        {
                            break;
                        }

                        try
                        {
                            await
                                this.servicePartitionClientV2.SubscribeAsync(info.Subscriber.EventId, info.Id)
                                    .ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                });
        }

        #endregion
    }
}
