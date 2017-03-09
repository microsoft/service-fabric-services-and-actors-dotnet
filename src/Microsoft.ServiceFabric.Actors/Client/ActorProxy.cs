// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Client
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.Builder;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    /// <summary>
    /// Provides the base implementation for the proxy to the remote actor objects implementing IActor interfaces.
    /// </summary>
    public abstract class ActorProxy : ProxyBase, IActorProxy
    {
        private ActorProxyGeneratorWith proxyGeneratorWith;
        private ActorServicePartitionClient servicePartitionClient;

        internal static readonly ActorProxyFactory DefaultProxyFactory = new ActorProxyFactory();

        /// <summary>
        /// Initializes a new instance of the ActorProxy class.
        /// </summary>
        protected ActorProxy()
        {
        }

        /// <summary>
        /// Gets <see cref="ServiceFabric.Actors.ActorId"/> associated with the proxy object.
        /// </summary>
        /// <value><see cref="ServiceFabric.Actors.ActorId"/> associated with the proxy object.</value>
        ActorId IActorProxy.ActorId
        {
            get { return this.servicePartitionClient.ActorId; }
        }

        /// <summary>
        /// Gets <see cref="Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.
        /// </summary>
        /// <value><see cref="Client.IActorServicePartitionClient"/> that this proxy is using to communicate with the actor.</value>
        IActorServicePartitionClient IActorProxy.ActorServicePartitionClient
        {
            get { return this.servicePartitionClient; }
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
            string listenerName = null) where TActorInterface : IActor
        {
            return DefaultProxyFactory.CreateActorProxy<TActorInterface>(actorId, applicationName, serviceName, listenerName);
        }

        /// <summary>
        /// Creates a proxy to the actor object that implements an actor interface.
        /// </summary>
        /// <typeparam name="TActorInterface">
        /// The actor interface implemented by the remote actor object. 
        /// The returned proxy object will implement this interface.
        /// </typeparam>
        /// <param name="serviceUri">Uri of the actor service.</param>
        /// <param name="actorId">Actor Id of the proxy actor object. Methods called on this proxy will result in requests 
        /// being sent to the actor with this id.</param>
        /// <param name="listenerName">
        /// By default an actor service has only one listener for clients to connect to and communicate with.
        /// However it is possible to configure an actor service with more than one listeners, the listenerName parameter specifies the name of the listener to connect to.
        /// </param>
        /// <returns>An actor proxy object that implements <see cref="IActorProxy"/> and TActorInterface.</returns>
        public static TActorInterface Create<TActorInterface>(
            ActorId actorId,
            Uri serviceUri,
            string listenerName = null) where TActorInterface : IActor
        {
            return DefaultProxyFactory.CreateActorProxy<TActorInterface>(serviceUri, actorId, listenerName);
        }

        internal override DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetRequestMessageBodySerializer(interfaceId);
        }

        internal override DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.proxyGeneratorWith.GetResponseMessageBodySerializer(interfaceId);
        }

        internal override object GetResponseMessageBodyValue(object responseMessageBody)
        {
            return ((ActorMessageBody)responseMessageBody).Value;
        }

        internal override object CreateRequestMessageBody(object requestMessageBodyValue)
        {
            return new ActorMessageBody() { Value = requestMessageBodyValue };
        }

        internal override Task<byte[]> InvokeAsync(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes,
            CancellationToken cancellationToken)
        {
            var actorMsgHeaders = new ActorMessageHeaders()
            {
                ActorId = this.servicePartitionClient.ActorId,
                InterfaceId = interfaceId,
                MethodId = methodId,
                CallContext = GetCallContext()
            };

            return this.servicePartitionClient.InvokeAsync(actorMsgHeaders, requestMsgBodyBytes, cancellationToken);
        }

        internal override void Invoke(
            int interfaceId,
            int methodId,
            byte[] requestMsgBodyBytes)
        {
            // actor proxy does not support one way messages
            // actor events are sent from actor event proxy
            throw new NotImplementedException();
        }

        internal void Initialize(ActorProxyGeneratorWith actorProxyGeneratorWith, ActorServicePartitionClient actorServicePartitionClient)
        {
            this.proxyGeneratorWith = actorProxyGeneratorWith;
            this.servicePartitionClient = actorServicePartitionClient;
        }

        #region Event Subscription

        internal async Task SubscribeAsync(Type eventType, object subscriber, TimeSpan resubscriptionInterval)
        {
            var actorId = this.servicePartitionClient.ActorId;
            var info = ActorEventSubscriberManager.Singleton.RegisterSubscriber(actorId, eventType, subscriber);

            Exception error = null;
            try
            {
                await this.servicePartitionClient.SubscribeAsync(info.Subscriber.EventId, info.Id);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error != null)
            {
                try
                {
                    await this.UnsubscribeAsync(eventType, subscriber);
                }
                catch
                {
                    // ignore
                }

                throw error;
            }

            this.ResubscribeAsync(info, resubscriptionInterval);
        }

        internal async Task UnsubscribeAsync(Type eventType, object subscriber)
        {
            var actorId = this.servicePartitionClient.ActorId;
            ActorEventSubscriberManager.SubscriptionInfo info;
            if (ActorEventSubscriberManager.Singleton.TryUnregisterSubscriber(actorId, eventType, subscriber, out info))
            {
                await this.servicePartitionClient.UnsubscribeAsync(info.Subscriber.EventId, info.Id);
            }
        }

        private void ResubscribeAsync(ActorEventSubscriberManager.SubscriptionInfo info, TimeSpan resubscriptionInterval)
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
                            await this.servicePartitionClient.SubscribeAsync(info.Subscriber.EventId, info.Id).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                });
        }

        #endregion

        private static string GetCallContext()
        {
            string callContextValue;
            if (ActorLogicalCallContext.TryGet(out callContextValue))
            {
                return string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}{1}", 
                    callContextValue, 
                    Guid.NewGuid().ToString());
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
