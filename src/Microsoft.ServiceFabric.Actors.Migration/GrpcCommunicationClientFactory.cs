// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
////    using System.Collections.Concurrent;
////    using System.Collections.Generic;
////    using System.Threading;
////    using System.Threading.Tasks;
////    using Grpc.Core;
////    using Microsoft.ServiceFabric.Services.Client;
////    using Microsoft.ServiceFabric.Services.Communication.Client;

////#pragma warning disable CS3024 // Constraint type is not CLS-compliant
////    /// <summary>
////    /// GrpcCommunicationClientFactory
////    /// </summary>
////    /// <typeparam name="TClient">TClient</typeparam>
////    public class GrpcCommunicationClientFactory<TClient> : CommunicationClientFactoryBase<GrpcCommunicationClient<TClient>>
////#pragma warning restore CS3024 // Constraint type is not CLS-compliant
////            where TClient : ClientBase<TClient>
////    {
////        private ConcurrentDictionary<string, Channel> channels = new ConcurrentDictionary<string, Channel>();

////        /// <summary>
////        /// Initializes a new instance of the <see cref="GrpcCommunicationClientFactory{TClient}"/> class.
////        /// </summary>
////        /// <param name="resolver">resolver</param>
////        /// <param name="exceptionHandlers">exceptionHandlers</param>
////        public GrpcCommunicationClientFactory(
////            IServicePartitionResolver resolver = null,
////            IEnumerable<IExceptionHandler> exceptionHandlers = null)
////            : base(resolver, exceptionHandlers)
////        {
////        }

////        /// <summary>
////        /// AbortClient
////        /// </summary>
////        /// <param name="client">client</param>
////        protected override void AbortClient(GrpcCommunicationClient<TClient> client)
////        {
////            // Do nothing, since other clients could be using the same channel
////        }

////        /// <summary>
////        /// CreateClientAsync
////        /// </summary>
////        /// <param name="endpoint">endpoint</param>
////        /// <param name="cancellationToken">cancellationToken</param>
////        /// <returns>GrpcCommunicationClient</returns>
////        protected override Task<GrpcCommunicationClient<TClient>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
////        {
////            var address = endpoint
////                .Replace("http://", string.Empty)
////                .Replace("Http://", string.Empty);

////            Channel channel = this.channels.GetOrAdd(
////                address,
////                target => new Channel(
////                    target,
////                    ChannelCredentials.Insecure));

////            return Task.FromResult(new GrpcCommunicationClient<TClient>(channel));
////        }

////        /// <summary>
////        /// ValidateClient
////        /// </summary>
////        /// <param name="client">client</param>
////        /// <returns>true or false</returns>
////        protected override bool ValidateClient(GrpcCommunicationClient<TClient> client)
////        {
////            return client.Channel.State == ChannelState.Ready;
////        }

////        /// <summary>
////        /// ValidateClient
////        /// </summary>
////        /// <param name="endpoint">endpoint</param>
////        /// <param name="client">client</param>
////        /// <returns>true or false</returns>
////        protected override bool ValidateClient(string endpoint, GrpcCommunicationClient<TClient> client)
////        {
////            return client.Channel.State == ChannelState.Ready && endpoint.Contains(client.Channel.Target);
////        }
////    }
}
