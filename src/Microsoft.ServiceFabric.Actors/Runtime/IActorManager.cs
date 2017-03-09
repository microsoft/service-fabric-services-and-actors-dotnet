// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Common.Tracing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;

    internal interface IActorManager
    {
        ActorService ActorService { get; }

        #region Actor Diagnostics

        DiagnosticsEventManager DiagnosticsEventManager { get; }
        
        #endregion

        #region Actor Manager Life Cycle

        Task OpenAsync(IServicePartition partition, CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);

        void Abort();

        #endregion

        #region Actor Method Dispatch

        Task<byte[]> InvokeAsync(ActorId actorId, int interfaceId, int methodId, string callContext, byte[] requestMsgBody, CancellationToken cancellationToken);

        Task<byte[]> DispatchToActorAsync(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<byte[]>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken);

        #endregion

        #region Actor Events

        Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber);

        Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId);

        TEvent GetEvent<TEvent>(ActorId actorId);

        #endregion

        #region Actor Reminders

        bool HasRemindersLoaded { get; }

        Task<IActorReminder> RegisterOrUpdateReminderAsync(ActorId actorId, string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period, bool saveState = true);

        IActorReminder GetReminder(string reminderName, ActorId actorId);

        Task UnregisterReminderAsync(string reminderName, ActorId actorId, bool removeFromStateProvider);

        Task StartLoadingRemindersAsync(CancellationToken cancellationToken);

        void FireReminder(ActorReminder reminder);

        #endregion

        #region Actor Query

        Task DeleteActorAsync(string callContext, ActorId actorId, CancellationToken cancellationToken);

        Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(ContinuationToken continuationToken, CancellationToken cancellationToken);

        #endregion

        #region Actor Tracing

        string GetActorTraceId(ActorId actorId);

        FabricEvents.ExtensionsEvents TraceSource { get; }

        #endregion
    }
}