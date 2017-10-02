// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Diagnostics;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

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

        bool IsClosed { get; }

        #endregion

        #region Actor Method Dispatch

#if !DotNetCoreClr
        Task<byte[]> InvokeAsync(ActorId actorId, int interfaceId, int methodId, string callContext,
            byte[] requestMsgBody, CancellationToken cancellationToken);
#endif

        Task<T> DispatchToActorAsync<T>(
            ActorId actorId,
            ActorMethodContext actorMethodContext,
            bool createIfRequired,
            Func<ActorBase, CancellationToken, Task<T>> actorFunc,
            string callContext,
            bool timerCall,
            CancellationToken cancellationToken);

        //V2 Stack Apis
        Task<IServiceRemotingResponseMessageBody> InvokeAsync(ActorId actorId, int interfaceId, int methodId,
            string callContext,
            IServiceRemotingRequestMessageBody requestMsgBody,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory,
            CancellationToken cancellationToken);

        #endregion

        #region Actor Events

        Task SubscribeAsync(ActorId actorId, int eventInterfaceId, IActorEventSubscriberProxy subscriber);

        Task UnsubscribeAsync(ActorId actorId, int eventInterfaceId, Guid subscriberId);

        TEvent GetEvent<TEvent>(ActorId actorId);

        #endregion

        #region Actor Reminders

        bool HasRemindersLoaded { get; }

        Task<IActorReminder> RegisterOrUpdateReminderAsync(ActorId actorId, string reminderName, byte[] state,
            TimeSpan dueTime, TimeSpan period, bool saveState = true);

        IActorReminder GetReminder(string reminderName, ActorId actorId);

        Task UnregisterReminderAsync(string reminderName, ActorId actorId, bool removeFromStateProvider);

        Task StartLoadingRemindersAsync(CancellationToken cancellationToken);

        void FireReminder(ActorReminder reminder);

        #endregion

        #region Actor Query

        Task DeleteActorAsync(string callContext, ActorId actorId, CancellationToken cancellationToken);

        Task<PagedResult<ActorInformation>> GetActorsFromStateProvider(ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        #endregion

        #region Actor Tracing

        string GetActorTraceId(ActorId actorId);

        ActorEventSource TraceSource { get; }

        #endregion
    }
}