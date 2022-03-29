// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// Represents the base class for Microsoft Service Fabric based reliable actors service.
    /// </summary>
    /// <remarks>
    /// Derive from this class to implement your own custom actor service if you want to override
    /// any service level behavior for your actors.
    /// </remarks>
    public class ActorService : StatefulServiceBase, IActorService
    {
        private const string TraceType = "ActorService";

        private readonly ActorTypeInformation actorTypeInformation;
        private readonly IActorStateProvider stateProvider;
        private readonly ActorServiceSettings settings;
        private readonly IActorActivator actorActivator;
        private readonly ActorManagerAdapter actorManagerAdapter;
        private readonly Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory;
#if !DotNetCoreClr
        private Remoting.V1.Runtime.ActorMethodDispatcherMap methodDispatcherMapV1;
#endif
        private ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder;
        private ReplicaRole replicaRole;
        private Remoting.V2.Runtime.ActorMethodDispatcherMap methodDispatcherMapV2;

        private IMigrationOrchestrator migrationOrchestrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorService"/> class.
        /// </summary>
        /// <param name="context">Service context the actor service is operating under.</param>
        /// <param name="actorTypeInfo">The type information of the Actor.</param>
        /// <param name="actorFactory">The factory method to create Actor objects.</param>
        /// <param name="stateManagerFactory">The factory method to create <see cref="IActorStateManager"/></param>
        /// <param name="stateProvider">The state provider to store and access the state of the Actor objects.</param>
        /// <param name="settings">The settings used to configure the behavior of the Actor service.</param>
        public ActorService(
            StatefulServiceContext context,
            ActorTypeInformation actorTypeInfo,
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null,
            IActorStateProvider stateProvider = null,
            ActorServiceSettings settings = null)
            : this(
                context,
                actorTypeInfo,
                migrationSettings: null,
                actorFactory,
                stateManagerFactory,
                stateProvider,
                settings)
        {
        }

        internal ActorService(
           StatefulServiceContext context,
           ActorTypeInformation actorTypeInfo,
           MigrationSettings migrationSettings,
           Func<ActorService, ActorId, ActorBase> actorFactory = null,
           Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null,
           IActorStateProvider stateProvider = null,
           ActorServiceSettings settings = null)
           : this(
               context,
               actorTypeInfo,
               MigrationReflectionHelper.GetMigrationOrchestrator(
                   stateProvider ?? ActorStateProviderHelper.CreateDefaultStateProvider(actorTypeInfo),
                   actorTypeInfo,
                   context,
                   migrationSettings),
               actorFactory,
               stateManagerFactory,
               stateProvider ?? ActorStateProviderHelper.CreateDefaultStateProvider(actorTypeInfo),
               settings)
        {
        }

        internal ActorService(
            StatefulServiceContext context,
            ActorTypeInformation actorTypeInfo,
            IMigrationOrchestrator migrationOrchestrator,
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null,
            IActorStateProvider stateProvider = null,
            ActorServiceSettings settings = null)
            : base(
                context,
                migrationOrchestrator != null ? migrationOrchestrator.GetMigrationActorStateProvider() : stateProvider)
        {
            this.actorTypeInformation = actorTypeInfo;
            this.stateProvider = (IActorStateProvider)this.StateProviderReplica;
            this.settings = ActorServiceSettings.DeepCopyFromOrDefaultOnNull(settings);

            // Set internal components
            this.actorActivator = new ActorActivator(actorFactory ?? this.DefaultActorFactory);
            this.stateManagerFactory = stateManagerFactory ?? DefaultActorStateManagerFactory;
            this.actorManagerAdapter = new ActorManagerAdapter { ActorManager = new MockActorManager(this) };
            this.replicaRole = ReplicaRole.Unknown;

            if (migrationOrchestrator != null)
            {
                // Migration initialization
                this.migrationOrchestrator = migrationOrchestrator;
                this.migrationOrchestrator.RegisterCompletionCallback(this.StartRemindersIfNeededAsync);
            }

            ActorTelemetry.ActorServiceInitializeEvent(
                this.ActorManager.ActorService.Context,
                this.StateProviderReplica.GetType().ToString());
        }

        /// <summary>
        /// Gets the ActorTypeInformation for actor service.
        /// </summary>
        /// <value>
        /// <see cref="Runtime.ActorTypeInformation"/>
        /// for the actor hosted by the service replica.
        /// </value>
        public ActorTypeInformation ActorTypeInformation
        {
            get { return this.actorTypeInformation; }
        }

        /// <summary>
        /// Gets a <see cref="IActorStateProvider"/> that represents the state provider for the actor service.
        /// </summary>
        /// <value>
        /// <see cref="IActorStateProvider"/>
        /// representing the state provider for the actor service.
        /// </value>
        public IActorStateProvider StateProvider
        {
            get { return this.stateProvider; }
        }

        /// <summary>
        /// Gets the settings for the actor service.
        /// </summary>
        /// <value>
        /// Settings for the actor service.
        /// </value>
        public ActorServiceSettings Settings
        {
            get { return this.settings; }
        }

        internal IActorActivator ActorActivator
        {
            get { return this.actorActivator; }
        }

#if !DotNetCoreClr

        internal Remoting.V1.Runtime.ActorMethodDispatcherMap MethodDispatcherMapV1
        {
            get { return this.methodDispatcherMapV1; }
            set { this.methodDispatcherMapV1 = value; }
        }
#endif

        internal Remoting.V2.Runtime.ActorMethodDispatcherMap MethodDispatcherMapV2
        {
            get { return this.methodDispatcherMapV2; }
            set { this.methodDispatcherMapV2 = value; }
        }

        internal ActorMethodFriendlyNameBuilder MethodFriendlyNameBuilder
        {
            get { return this.methodFriendlyNameBuilder; }
        }

        internal IActorManager ActorManager
        {
            get { return this.actorManagerAdapter.ActorManager; }
        }

        #region Migration
        internal bool AreActorCallsAllowed
        {
            get
            {
                if (this.migrationOrchestrator != null)
                {
                    return this.migrationOrchestrator.AreActorCallsAllowed();
                }

                return true;
            }
        }

        internal bool IsActorCallToBeForwarded
        {
            get
            {
                if (this.migrationOrchestrator != null)
                {
                    return this.migrationOrchestrator.IsActorCallToBeForwarded();
                }

                return false;
            }
        }

        internal IMigrationOrchestrator MigrationOrchestrator { get => this.migrationOrchestrator; }
        #endregion Migration

        #region IActorService Members

        /// <summary>
        /// Deletes an Actor from the Actor service.
        /// </summary>
        /// <param name="actorId">The <see cref="ActorId"/> of the actor to be deleted.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        /// <remarks>
        /// <para>An active actor, will be deactivated and its state will also be deleted from state provider.</para>
        /// <para>An in-active actor's state will be deleted from state provider.</para>
        /// <para>If this method is called for a non-existent actor id in the system, it will be a no-op.</para>
        /// </remarks>
        Task IActorService.DeleteActorAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            return this.ActorManager.DeleteActorAsync(
                Guid.NewGuid().ToString(),
                actorId,
                cancellationToken);
        }

        /// <summary>
        /// Gets the list of Actors by querying the actor service.
        /// </summary>
        /// <param name="continuationToken">A continuation token to start querying the results from.
        /// A null value of continuation token means start returning values form the beginning.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation of call to server.</returns>
        Task<PagedResult<ActorInformation>> IActorService.GetActorsAsync(
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return this.ActorManager.GetActorsFromStateProvider(
                continuationToken,
                cancellationToken);
        }

        /// <inheritdoc/>
        Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> IActorService.GetRemindersAsync(
           ActorId actorId,
           ContinuationToken continuationToken,
           CancellationToken cancellationToken)
        {
            return this.ActorManager.GetRemindersFromStateProviderAsync(
                actorId,
                continuationToken,
                cancellationToken);
        }

        #endregion

        internal async Task StartRemindersIfNeededAsync(bool actorCallsAllowed, CancellationToken cancellationToken)
        {
            if (actorCallsAllowed)
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.Context.TraceId,
                    "ActorCallsAllowed : TRUE - Starting reminders.");
                try
                {
                    await this.ActorManager.StartLoadingRemindersAsync(cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    var healthInfo = new HealthInformation("ActorService", "LoadReminders", HealthState.Error)
                    {
                        TimeToLive = TimeSpan.MaxValue,
                        RemoveWhenExpired = false,
                        Description = ex.Message,
                    };

                    this.Partition.ReportPartitionHealth(healthInfo, new HealthReportSendOptions { Immediate = true });

                    throw ex;
                }
            }
            else
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.Context.TraceId,
                    "ActorCallsAllowed : FALSE");

                //// TODO: Stop reminders from firing
            }
        }

        internal IActorStateManager CreateStateManager(ActorBase actor)
        {
            return this.stateManagerFactory.Invoke(actor, this.StateProvider);
        }

        internal void InitializeInternal(ActorMethodFriendlyNameBuilder methodNameBuilder)
        {
            this.methodFriendlyNameBuilder = methodNameBuilder;
#if !DotNetCoreClr
            this.MethodDispatcherMapV1 =
                new Actors.Remoting.V1.Runtime.ActorMethodDispatcherMap(this.ActorTypeInformation);
#endif
            this.MethodDispatcherMapV2 =
                new Actors.Remoting.V2.Runtime.ActorMethodDispatcherMap(this.ActorTypeInformation);
        }

        #region Migration
        internal bool IsConfiguredForMigration()
        {
            return this.migrationOrchestrator != null;
        }

        internal void ThrowIfActorCallsDisallowed()
        {
            if (this.migrationOrchestrator != null)
            {
                this.migrationOrchestrator.ThrowIfActorCallsDisallowed();
            }
        }
        #endregion Migration

        #region StatefulServiceBase Overrides

        /// <summary>
        /// Overrides <see cref="Microsoft.ServiceFabric.Services.Runtime.StatefulServiceBase.CreateServiceReplicaListeners()"/>.
        /// </summary>
        /// <returns>Endpoint string pairs like
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            var types = new List<Type> { this.ActorTypeInformation.ImplementationType };
            types.AddRange(this.ActorTypeInformation.InterfaceTypes);

            var provider = ActorRemotingProviderAttribute.GetProvider(types);
            var serviceReplicaListeners = new List<ServiceReplicaListener>();
#if !DotNetCoreClr
            if (Services.Remoting.Helper.IsRemotingV1(provider.RemotingListenerVersion))
            {
               serviceReplicaListeners.Add(
                    new ServiceReplicaListener((t) => { return provider.CreateServiceRemotingListener(this); }));
            }
#endif
            if (Services.Remoting.Helper.IsEitherRemotingV2(provider.RemotingListenerVersion))
            {
                var listeners = provider.CreateServiceRemotingListeners();
                foreach (var kvp in listeners)
                {
                    serviceReplicaListeners.Add(new ServiceReplicaListener(
                    t =>
                    {
                        return kvp.Value(this);
                    }, kvp.Key));
                }
            }

            this.AddMigrationListener(serviceReplicaListeners);

            return serviceReplicaListeners;
        }

        /// <summary>
        /// Adds migration specific listeners.
        /// </summary>
        /// <param name="serviceReplicaListeners">Existing listener list.</param>
        /// <remarks>To be used when CreateServiceReplicaListeners() is overriden by Custom implementation of Actor Service.</remarks>
        protected void AddMigrationListener(IList<ServiceReplicaListener> serviceReplicaListeners)
        {
            // Add migration endpoint
            if (this.migrationOrchestrator != null)
            {
                serviceReplicaListeners.Add(new ServiceReplicaListener(_ => this.migrationOrchestrator.GetMigrationCommunicationListener(), Migration.Constants.MigrationListenerName));
            }
        }

        /// <summary>
        /// Overrides <see cref="StatefulServiceBase.RunAsync(CancellationToken)"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous operation of loading reminders when the replica becomes primary.
        /// </returns>
        /// <remarks>
        /// If you need to override this method, please make sure to call this method from your overridden method.
        /// Also make sure your implementation of overridden method conforms to the guideline specified for
        /// <see cref="StatefulServiceBase.RunAsync(CancellationToken)"/>.
        /// <para>
        /// Failing to do so can cause failover, reconfiguration or upgrade of your actor service to get stuck and
        /// can impact availibility of your service.
        /// </para>
        /// </remarks>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            if (this.migrationOrchestrator != null)
            {
                try
                {
                    bool isResumed = await this.migrationOrchestrator.TryResumeMigrationAsync(cancellationToken);
                    if (!isResumed && this.migrationOrchestrator.IsAutoStartMigration())
                    {
                        await this.migrationOrchestrator.StartMigrationAsync(cancellationToken);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    var healthInfo = new HealthInformation("ActorService", "ActorStateMigration", HealthState.Error)
                    {
                        TimeToLive = TimeSpan.MaxValue,
                        RemoveWhenExpired = false,
                        Description = ex.Message,
                    };

                    this.Partition.ReportPartitionHealth(healthInfo, new HealthReportSendOptions { Immediate = true });

                    throw ex;
                }
            }
            else
            {
                await this.ActorManager.StartLoadingRemindersAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Overrides <see cref="StatefulServiceBase.OnChangeRoleAsync(ReplicaRole, CancellationToken)"/>.
        /// </summary>
        /// <param name="newRole">The new role for the replica.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation performed when the replica becomes primary.</returns>
        protected override async Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.Context.TraceId,
                "Begin change role. New role: {0}.",
                newRole);

            if (newRole == ReplicaRole.Primary)
            {
                this.actorManagerAdapter.ActorManager = new ActorManager(this);
                await this.actorManagerAdapter.OpenAsync(this.Partition, cancellationToken);
                this.ActorManager.DiagnosticsEventManager.ActorChangeRole(this.replicaRole, newRole);
            }
            else
            {
                if ((this.ActorManager != null) && (this.ActorManager.DiagnosticsEventManager != null))
                {
                    this.ActorManager.DiagnosticsEventManager.ActorChangeRole(this.replicaRole, newRole);
                }

                await this.actorManagerAdapter.CloseAsync(cancellationToken);
            }

            this.replicaRole = newRole;

            ActorTrace.Source.WriteInfoWithId(
                TraceType,
                this.Context.TraceId,
                "End change role. New role: {0}.",
                newRole);
        }

        /// <summary>
        /// Overrides <see cref="StatefulServiceBase.OnCloseAsync(CancellationToken)"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation performed when the replica is closed.</returns>
        protected override async Task OnCloseAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.Context.TraceId, "Begin close.");

            ActorTelemetry.ActorServiceReplicaCloseEvent(this.ActorManager.ActorService.Context);

            await this.actorManagerAdapter.CloseAsync(cancellationToken);

            ActorTrace.Source.WriteInfoWithId(TraceType, this.Context.TraceId, "End close.");
        }

        /// <summary>
        /// Overrides <see cref="StatefulServiceBase.OnAbort()"/>.
        /// </summary>
        protected override void OnAbort()
        {
            ActorTrace.Source.WriteInfoWithId(TraceType, this.Context.TraceId, "Abort.");

            this.actorManagerAdapter.Abort();
        }

#endregion
        private static IActorStateManager DefaultActorStateManagerFactory(
            ActorBase actorBase,
            IActorStateProvider actorStateProvider)
        {
            return new ActorStateManager(actorBase, actorStateProvider);
        }

        private ActorBase DefaultActorFactory(ActorService actorService, ActorId actorId)
        {
            return (ActorBase)Activator.CreateInstance(
                this.ActorTypeInformation.ImplementationType,
                actorService,
                actorId);
        }
    }
}
