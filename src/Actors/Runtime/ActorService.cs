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
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Microsoft.ServiceFabric.Actors.Runtime;

/// <summary>
/// Represents the base class for Microsoft Service Fabric based reliable actors service.
/// </summary>
/// <remarks>
/// Derive from this class to implement your own custom actor service if you want to override
/// any service level behavior for your actors.
/// </remarks>
public class ActorService : StatefulServiceBase, IActorService
{
    const string traceType = "ActorService";

    readonly ActorTypeInformation actorTypeInformation;
    readonly IActorStateProvider stateProvider;
    readonly ActorServiceSettings settings;
    readonly IActorActivator actorActivator;
    readonly ActorManagerAdapter actorManagerAdapter;
    readonly Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory;
    ActorMethodFriendlyNameBuilder methodFriendlyNameBuilder;
    ReplicaRole replicaRole;
    Remoting.V2.Runtime.ActorMethodDispatcherMap methodDispatcherMapV2;

    readonly IClock clock = new SystemClock();
    readonly IDiagnostics diagnostics;
    readonly DiagnosticsFactory diagnosticsFactory;

    static Func<ServiceContext, ActorTypeInformation, ActorMethodFriendlyNameBuilder, DiagnosticsFactory> createDiagnosticFactory =
        (serviceContext, actorTypeInformation, methodNameBuilder) => new DiagnosticsFactory(serviceContext, actorTypeInformation, methodNameBuilder);

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
        : base(
            context,
            stateProvider ?? ActorStateProviderHelper.CreateDefaultStateProvider(actorTypeInfo))
    {
        actorTypeInformation = actorTypeInfo;
        this.stateProvider = (IActorStateProvider)StateProviderReplica;
        this.settings = ActorServiceSettings.DeepCopyFromOrDefaultOnNull(settings);

        // Set internal components
        actorActivator = new ActorActivator(actorFactory ?? DefaultActorFactory);
        this.stateManagerFactory = stateManagerFactory ?? DefaultActorStateManagerFactory;
        actorManagerAdapter = new ActorManagerAdapter { ActorManager = new MockActorManager(this) };
        replicaRole = ReplicaRole.Unknown;
        methodFriendlyNameBuilder = new ActorMethodFriendlyNameBuilder(actorTypeInformation);

        diagnosticsFactory = createDiagnosticFactory(context, actorTypeInfo, methodFriendlyNameBuilder);
        diagnostics = diagnosticsFactory.CreateDiagnostics(clock);

        ActorTelemetry.ActorServiceInitializeEvent(ActorManager.ActorService.Context, StateProviderReplica.GetType().ToString());
    }

    /// <summary>
    /// Gets the ActorTypeInformation for actor service.
    /// </summary>
    /// <value>
    /// <see cref="Runtime.ActorTypeInformation"/>
    /// for the actor hosted by the service replica.
    /// </value>
    public ActorTypeInformation ActorTypeInformation => actorTypeInformation;

    /// <summary>
    /// Gets a <see cref="IActorStateProvider"/> that represents the state provider for the actor service.
    /// </summary>
    /// <value>
    /// <see cref="IActorStateProvider"/>
    /// representing the state provider for the actor service.
    /// </value>
    public IActorStateProvider StateProvider => stateProvider;

    /// <summary>
    /// Gets the settings for the actor service.
    /// </summary>
    /// <value>
    /// Settings for the actor service.
    /// </value>
    public ActorServiceSettings Settings => settings;

    internal IActorActivator ActorActivator => actorActivator;

    internal Remoting.V2.Runtime.ActorMethodDispatcherMap MethodDispatcherMapV2
    {
        get => methodDispatcherMapV2; set => methodDispatcherMapV2 = value;
    }

    internal ActorMethodFriendlyNameBuilder MethodFriendlyNameBuilder => methodFriendlyNameBuilder;

    internal IActorManager ActorManager => actorManagerAdapter.ActorManager;

    internal IClock Clock => clock;

    internal IDiagnostics Diagnostics => diagnostics;

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
        var requestId = LogContext.GetRequestIdOrDefault();
        return ActorManager.DeleteActorAsync(
            requestId == default(Guid) ? Guid.NewGuid().ToString() : requestId.ToString(),
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
    Task<PagedResult<ActorInformation>> IActorService.GetActorsAsync(ContinuationToken continuationToken, CancellationToken cancellationToken) => ActorManager.GetActorsFromStateProvider(continuationToken, cancellationToken);

    /// <inheritdoc/>
    Task<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>> IActorService.GetRemindersAsync(ActorId actorId, ContinuationToken continuationToken, CancellationToken cancellationToken) => ActorManager.GetRemindersFromStateProviderAsync(actorId, continuationToken, cancellationToken);

    #endregion

    internal async Task StartRemindersIfNeededAsync(bool actorCallsAllowed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (actorCallsAllowed)
        {
            ActorTrace.Source.WriteInfoWithId(
                traceType,
                Context.TraceId,
                "ActorCallsAllowed : TRUE - Starting reminders.");
            try
            {
                await ActorManager.StartLoadingRemindersAsync(cancellationToken);
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

                Partition.ReportPartitionHealth(healthInfo, new HealthReportSendOptions { Immediate = true });

                throw;
            }
        }
        else
        {
            ActorTrace.Source.WriteInfoWithId(
                traceType,
                Context.TraceId,
                "ActorCallsAllowed : FALSE");
        }
    }

    internal IActorStateManager CreateStateManager(ActorBase actor) => stateManagerFactory.Invoke(actor, StateProvider);

    internal void InitializeInternal(ActorMethodFriendlyNameBuilder methodNameBuilder)
    {
        methodFriendlyNameBuilder = methodNameBuilder;
        MethodDispatcherMapV2 = new Actors.Remoting.V2.Runtime.ActorMethodDispatcherMap(ActorTypeInformation);
    }

    #region StatefulServiceBase Overrides

    /// <summary>
    /// Overrides <see cref="Microsoft.ServiceFabric.Services.Runtime.StatefulServiceBase.CreateServiceReplicaListeners()"/>.
    /// </summary>
    /// <returns>Endpoint string pairs like
    /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}</returns>
    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
    {
        var types = new List<Type> { ActorTypeInformation.ImplementationType };
        types.AddRange(ActorTypeInformation.InterfaceTypes);

        var provider = ActorRemotingProviderAttribute.GetProvider(types);
        var serviceReplicaListeners = new List<ServiceReplicaListener>();

        var listeners = provider.CreateServiceRemotingListeners();
        foreach (var kvp in listeners)
        {
            serviceReplicaListeners.Add(new ServiceReplicaListener(t => kvp.Value(this), kvp.Key));
        }

        return serviceReplicaListeners;
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
    protected override Task RunAsync(CancellationToken cancellationToken) => ActorManager.StartLoadingRemindersAsync(cancellationToken);

    /// <summary>
    /// Overrides <see cref="StatefulServiceBase.OnChangeRoleAsync(ReplicaRole, CancellationToken)"/>.
    /// </summary>
    /// <param name="newRole">The new role for the replica.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation performed when the replica becomes primary.</returns>
    protected override async Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
    {
        ActorTrace.Source.WriteInfoWithId(
            traceType,
            Context.TraceId,
            "Begin change role. New role: {0}.",
            newRole);

        if (newRole == ReplicaRole.Primary)
        {
            actorManagerAdapter.ActorManager = new ActorManager(this, clock, diagnostics);
            await actorManagerAdapter.OpenAsync(Partition, cancellationToken);
            diagnostics.ActorChangeRole(replicaRole, newRole);
        }
        else
        {
            diagnostics.ActorChangeRole(replicaRole, newRole);
            await actorManagerAdapter.CloseAsync(cancellationToken);
        }

        replicaRole = newRole;

        ActorTrace.Source.WriteInfoWithId(traceType, Context.TraceId, "End change role. New role: {0}.", newRole);
    }

    /// <summary>
    /// Overrides <see cref="StatefulServiceBase.OnCloseAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation performed when the replica is closed.</returns>
    protected override async Task OnCloseAsync(CancellationToken cancellationToken)
    {
        ActorTrace.Source.WriteInfoWithId(traceType, Context.TraceId, "Begin close.");

        ActorTelemetry.ActorServiceReplicaCloseEvent(ActorManager.ActorService.Context);

        await actorManagerAdapter.CloseAsync(cancellationToken);

        DisposeDiagnostics();

        ActorTrace.Source.WriteInfoWithId(traceType, Context.TraceId, "End close.");
    }

    /// <summary>
    /// Overrides <see cref="StatefulServiceBase.OnAbort()"/>.
    /// </summary>
    protected override void OnAbort()
    {
        ActorTrace.Source.WriteInfoWithId(traceType, Context.TraceId, "Abort.");

        actorManagerAdapter.Abort();

        DisposeDiagnostics();
    }

    void DisposeDiagnostics()
    {
        diagnostics.Dispose();
        diagnosticsFactory.Dispose();
    }

    #endregion
    private static IActorStateManager DefaultActorStateManagerFactory(ActorBase actorBase, IActorStateProvider actorStateProvider) =>
        new ActorStateManager(actorBase, actorStateProvider, actorBase.ActorService.Diagnostics, actorBase.ActorService.Clock);

    private ActorBase DefaultActorFactory(ActorService actorService, ActorId actorId) => (ActorBase)Activator.CreateInstance(ActorTypeInformation.ImplementationType, actorService, actorId);
}
