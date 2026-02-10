// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Diagnostics;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Represents the base class for Microsoft Service Fabric based reliable actors service.
    /// </summary>
    /// <remarks>
    /// Derive from this class to implement your own custom actor service if you want to override
    /// any service level behavior for your actors.
    /// </remarks>
    public class ActorService : StatefulServiceBase, IActorService
    {
        const string TraceType = "ActorService";

        readonly ActorManagerAdapter actorManagerAdapter;
        readonly Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory;
        ReplicaRole replicaRole;
        readonly DiagnosticsFactory diagnosticsFactory;

        static Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory> createDiagnosticFactory = 
            (serviceContext, actorTypeInformation, methodNameBuilder) => new DiagnosticsFactory(serviceContext, actorTypeInformation, methodNameBuilder);

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorService"/> class.
        /// </summary>
        public ActorService(
            StatefulServiceContext context,
            ActorTypeInformation actorTypeInfo,
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null,
            IActorStateProvider stateProvider = null,
            ActorServiceSettings settings = null)
            : base(context, stateProvider ?? ActorStateProviderHelper.CreateDefaultStateProvider(actorTypeInfo))
        {
            ActorTypeInformation = actorTypeInfo;
            StateProvider = (IActorStateProvider)StateProviderReplica;
            Settings = ActorServiceSettings.DeepCopyFromOrDefaultOnNull(settings);

            // Set internal components
            ActorActivator = new ActorActivator(actorFactory ?? DefaultActorFactory);
            this.stateManagerFactory = stateManagerFactory ?? DefaultActorStateManagerFactory;
            actorManagerAdapter = new ActorManagerAdapter { ActorManager = new MockActorManager(this) };

            var methodFriendlyNameBuilder = new ActorMethodFriendlyNameBuilder(ActorTypeInformation);
            diagnosticsFactory = createDiagnosticFactory(context, actorTypeInfo, methodFriendlyNameBuilder);
            Diagnostics = diagnosticsFactory.CreateDiagnostics(Clock);

            ActorTelemetry.ActorServiceInitializeEvent(ActorManager.ActorService.Context, StateProviderReplica.GetType().ToString());
        }

        /// <summary>
        /// Gets the <see cref="ActorTypeInformation"/> for the actor service.
        /// </summary>
        public ActorTypeInformation ActorTypeInformation { get; }

        /// <summary>
        /// Gets a <see cref="IActorStateProvider"/> for the actor service.
        /// </summary>
        public IActorStateProvider StateProvider { get; }

        /// <summary>
        /// Gets the settings for the actor service.
        /// </summary>
        public ActorServiceSettings Settings { get; }

        internal IActorActivator ActorActivator { get; }
        internal ActorMethodDispatcherMap MethodDispatcherMapV2 { get; private set; }
        internal IActorManager ActorManager => actorManagerAdapter.ActorManager;
        internal IClock Clock { get; } = new SystemClock();
        internal IDiagnostics Diagnostics { get; }

        #region IActorService Members

        /// <inheritdoc/>
        Task IActorService.DeleteActorAsync(ActorId actorId, CancellationToken cancellation)
        {
            Guid requestId = LogContext.GetRequestIdOrDefault();
            string callContext = requestId == default ? Guid.NewGuid().ToString() : requestId.ToString();
            return ActorManager.DeleteActorAsync(callContext, actorId, cancellation);
        }

        /// <inheritdoc/>
        Task<PagedResult<ActorInformation>> IActorService.GetActorsAsync(ContinuationToken continuation, CancellationToken cancellation) =>
            ActorManager.GetActorsFromStateProvider(continuation, cancellation);

        /// <inheritdoc/>
        Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> IActorService.GetRemindersAsync(ActorId actorId, ContinuationToken continuation, CancellationToken cancellation) =>
           ActorManager.GetRemindersFromStateProviderAsync(actorId, continuation, cancellation);

        #endregion

        internal IActorStateManager CreateStateManager(ActorBase actor) => 
            stateManagerFactory.Invoke(actor, StateProvider);

        internal void InitializeInternal(ActorMethodFriendlyNameBuilder methodNameBuilder)
        {
            MethodDispatcherMapV2 = new ActorMethodDispatcherMap(ActorTypeInformation);
        }

        #region StatefulServiceBase Overrides

        /// <inheritdoc/>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            var types = new List<Type> { ActorTypeInformation.ImplementationType };
            types.AddRange(ActorTypeInformation.InterfaceTypes);

            var provider = ActorRemotingProviderAttribute.GetProvider(types);
            var serviceReplicaListeners = new List<ServiceReplicaListener>();
            Dictionary<string, Func<ActorService, IServiceRemotingListener>> listeners = provider.CreateServiceRemotingListeners();
            foreach (KeyValuePair<string, Func<ActorService, IServiceRemotingListener>> kvp in listeners)
                serviceReplicaListeners.Add(new ServiceReplicaListener(t => kvp.Value(this), kvp.Key));

            return serviceReplicaListeners;
        }

        /// <summary>
        /// Overrides <see cref="StatefulServiceBase.RunAsync(CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// If you need to override this method, please make sure to call this method from your overridden method.
        /// Also make sure your implementation of overridden method conforms to the guideline specified for
        /// <see cref="StatefulServiceBase.RunAsync(CancellationToken)"/>.
        /// Failing to do so can cause failover, reconfiguration or upgrade of your actor service to get stuck and
        /// can impact availability of your service.
        /// </remarks>
        protected override Task RunAsync(CancellationToken cancellation) =>
            ActorManager.StartLoadingRemindersAsync(cancellation);

        /// <inheritdoc/>
        protected override async Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellation)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, Context.TraceId, "Begin change role. New role: {0}.", newRole);

            if (newRole == ReplicaRole.Primary)
            {
                actorManagerAdapter.ActorManager = new ActorManager(this, Clock, Diagnostics);
                await actorManagerAdapter.OpenAsync(Partition, cancellation);
                Diagnostics.ActorChangeRole(replicaRole, newRole);
            }
            else
            {
                Diagnostics.ActorChangeRole(replicaRole, newRole);
                await actorManagerAdapter.CloseAsync(cancellation);
            }

            replicaRole = newRole;

            ActorTrace.Source.WriteInfoWithId(TraceType, Context.TraceId, "End change role. New role: {0}.", newRole);
        }

        /// <inheritdoc/>
        protected override async Task OnCloseAsync(CancellationToken cancellation)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, Context.TraceId, "Begin close.");
            ActorTelemetry.ActorServiceReplicaCloseEvent(ActorManager.ActorService.Context);

            await actorManagerAdapter.CloseAsync(cancellation);
            diagnosticsFactory.Dispose();

            ActorTrace.Source.WriteInfoWithId(TraceType, Context.TraceId, "End close.");
        }

        /// <inheritdoc/>
        protected override void OnAbort()
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, Context.TraceId, "Abort.");
            actorManagerAdapter.Abort();
        }

        #endregion

        static IActorStateManager DefaultActorStateManagerFactory(ActorBase actor, IActorStateProvider stateProvider) =>
            new ActorStateManager(actor, stateProvider, actor.ActorService.Diagnostics, actor.ActorService.Clock);

        ActorBase DefaultActorFactory(ActorService service, ActorId id) => 
            (ActorBase)Activator.CreateInstance(ActorTypeInformation.ImplementationType, service, id);
    }
}
