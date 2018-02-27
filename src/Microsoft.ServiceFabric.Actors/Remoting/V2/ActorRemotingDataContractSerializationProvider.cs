// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    ///     This is the default implmentation  for <see cref="IServiceRemotingMessageSerializationProvider" />used by actor
    ///     remoting.
    ///     It uses DataContractSerializer for serialization of remoting request and response message bodies.
    /// </summary>
    public class ActorRemotingDataContractSerializationProvider : ServiceRemotingDataContractSerializationProvider
    {
        /// <summary>
        ///     Creates an ActorRemotingDataContractSerializationProvider with default IBufferPoolManager
        /// </summary>
        public ActorRemotingDataContractSerializationProvider()
        {
        }

        /// <summary>
        ///     Creates an ActorRemotingDataContractSerializationProvider with user specified IBufferPoolManager.
        ///     If the specified buffer pool manager is null, the buffer pooling will be turned off.
        /// </summary>
        /// <param name="bodyBufferPoolManager"></param>
        public ActorRemotingDataContractSerializationProvider(
            IBufferPoolManager bodyBufferPoolManager)
            : base(bodyBufferPoolManager)
        {
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing request message body.
        /// </summary>
        /// <param name="serviceInterfaceType">The remoted service interface.</param>
        /// <param name="methodReturnTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing request message body.</returns>
        protected override DataContractSerializerSettings GetRequestMessageBodySerializerSettings(
            Type serviceInterfaceType,
            IEnumerable<Type> methodReturnTypes)
        {
            // TBD: Add Surrogage
            return base.GetRequestMessageBodySerializerSettings(serviceInterfaceType, methodReturnTypes);
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing response message body.
        /// </summary>
        /// <param name="serviceInterfaceType">The remoted service interface.</param>
        /// <param name="methodParameterTypes">The union of parameter types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing response message body.</returns>
        protected override DataContractSerializerSettings GetResponseMessageBodySerializerSettings(
            Type serviceInterfaceType,
            IEnumerable<Type> methodParameterTypes)
        {
            // TBD: Add Surrogate
            return base.GetResponseMessageBodySerializerSettings(serviceInterfaceType, methodParameterTypes);
        }
    }
}