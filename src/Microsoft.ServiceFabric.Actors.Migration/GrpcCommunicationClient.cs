// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
////    using System;
////    using System.Fabric;
////    using Grpc.Core;
////    using Microsoft.ServiceFabric.Services.Communication.Client;

////#pragma warning disable CS3024 // Constraint type is not CLS-compliant
////    /// <summary>
////    /// GrpcCommunicationClient
////    /// </summary>
////    /// <typeparam name="TClient">TClient</typeparam>
////    public class GrpcCommunicationClient<TClient> : ICommunicationClient
////#pragma warning restore CS3024 // Constraint type is not CLS-compliant
////        where TClient : ClientBase<TClient>
////    {
////#pragma warning disable CS3001 // Argument type is not CLS-compliant
////        /// <summary>
////        /// Initializes a new instance of the <see cref="GrpcCommunicationClient{TClient}"/> class.
////        /// </summary>
////        /// <param name="channel">channel</param>
////        public GrpcCommunicationClient(Channel channel)
////#pragma warning restore CS3001 // Argument type is not CLS-compliant
////        {
////            this.Channel = channel;
////            this.Client = (TClient)Activator.CreateInstance(typeof(TClient), this.Channel);
////        }

////        /// <summary>
////        /// Gets or sets client
////        /// </summary>
////        public TClient Client { get; set; }

////#pragma warning disable CS3003 // Type is not CLS-compliant
////        /// <summary>
////        /// Gets or sets channel
////        /// </summary>
////        public Channel Channel { get; set; }
////#pragma warning restore CS3003 // Type is not CLS-compliant

////        ResolvedServicePartition ICommunicationClient.ResolvedServicePartition { get; set; }

////        string ICommunicationClient.ListenerName { get; set; }

////        ResolvedServiceEndpoint ICommunicationClient.Endpoint { get; set; }
////    }
}
