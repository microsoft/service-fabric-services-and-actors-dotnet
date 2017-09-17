// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// Contains methods to register actor and actor service types with Service Fabric runtime. Registering the types allows the runtime to create instances of the actor and the actor service. See https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-actors-lifecycle for more information on the lifecycle of an actor.
    /// </summary>
    public static class ActorRuntime
    {
        private static readonly string NodeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRuntime"/> class.
        /// </summary>
        static ActorRuntime()
        {
            NodeName = FabricRuntime.GetNodeContext().NodeName;
        }

        /// <summary>
        /// Registers an actor type with Service Fabric runtime. This allows the runtime to create instances of this actor.
        /// </summary>
        /// <typeparam name="TActor">The type implementing the actor.</typeparam>
        /// <param name="timeout">A timeout period after which the registration operation will be canceled.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>returns a task that represents the asynchronous operation to register actor type with Service Fabric runtime.</returns>
        public static Task RegisterActorAsync<TActor>(
            TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
            where TActor : ActorBase
        {
            return RegisterActorAsync<TActor>(
                (context, actorTypeInfo) => new ActorService(context, actorTypeInfo),
                timeout,
                cancellationToken);
        }

        /// <summary>
        /// Registers an actor service with Service Fabric runtime. This allows the runtime to create instances of the replicas for the actor service.
        /// </summary>
        /// <typeparam name="TActor">The Type implementing actor.</typeparam>
        /// <param name="actorServiceFactory">The delegate that creates new actor service.</param>
        /// <param name="timeout">A timeout period after which the registration operation will be canceled.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that repre
        /// sents the asynchronous operation to register actor service with Service Fabric runtime.</returns>        
        public static async Task RegisterActorAsync<TActor>(
            Func<StatefulServiceContext, ActorTypeInformation, ActorService> actorServiceFactory,
            TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
            where TActor : ActorBase
        {
            actorServiceFactory.ThrowIfNull("actorServiceFactory");

            var actorType = typeof (TActor);
            var actorServiceType = actorServiceFactory.GetMethodInfo().ReturnType;
            var actorTypeInformation = ActorTypeInformation.Get(actorType);
            var serviceTypeName = ActorNameFormat.GetFabricServiceTypeName(actorTypeInformation.ImplementationType);

            try
            {
                var customActorServiceFactory = new ActorServiceFactory(
                    actorTypeInformation,
                    new ActorMethodFriendlyNameBuilder(actorTypeInformation),
                    actorServiceFactory);

                await ServiceRuntime.RegisterServiceAsync(
                    serviceTypeName,
                    context => customActorServiceFactory.CreateActorService(context),
                    timeout,
                    cancellationToken);

                ActorFrameworkEventSource.Writer.ActorTypeRegistered(
                    actorType.ToString(),
                    actorServiceType.ToString(),
                    NodeName);
            }
            catch (Exception e)
            {
                ActorFrameworkEventSource.Writer.ActorTypeRegistrationFailed(
                    e.ToString(),
                    actorType.ToString(),
                    actorServiceType.ToString(),
                    NodeName);
                throw;
            }
        }

    }
}
