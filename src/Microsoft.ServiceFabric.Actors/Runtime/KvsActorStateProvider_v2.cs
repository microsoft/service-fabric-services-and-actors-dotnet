// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using CopyCompletionCallback = System.Action<System.Fabric.KeyValueStoreEnumerator>;
    using DataLossCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<bool>>;
    using ReplicationCallback = System.Action<System.Collections.Generic.IEnumerator<System.Fabric.KeyValueStoreNotification>>;
    using RestoreCompletedCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;

    internal sealed class KvsActorStateProvider_V2 : KvsActorStateProviderBase
    {
        public KvsActorStateProvider_V2()
            : base(null)
        {
        }

        internal override KeyValueStoreReplica OnCreateAndInitializeReplica(
            StatefulServiceInitializationParameters initParams,
            CopyCompletionCallback copyHandler,
            ReplicationCallback replicationHandler,
            DataLossCallback onDataLossHandler,
            RestoreCompletedCallback restoreCompletedHandler)
        {
            var kvs = new KeyValueStoreWrapper(
                this.GetReplicatorSettings(),
                this.GetKvsReplicaSettings(),
                copyHandler,
                replicationHandler,
                onDataLossHandler,
                restoreCompletedHandler);

            kvs.InitializeAndOverrideNativeStore(initParams);

            return kvs;
        }

        private KeyValueStoreReplicaSettings_V2 GetKvsReplicaSettings()
        {
            return new KeyValueStoreReplicaSettings_V2(this.InitParams.CodePackageActivationContext.WorkDirectory);
        }

        private class KeyValueStoreWrapper : KeyValueStoreReplica_V2
        {
            private readonly CopyCompletionCallback copyHandler;
            private readonly ReplicationCallback replicationHandler;
            private readonly DataLossCallback onDataLossHandler;
            private readonly RestoreCompletedCallback restoreCompletedHandler;

            public KeyValueStoreWrapper(
                ReplicatorSettings replicatorSettings,
                KeyValueStoreReplicaSettings_V2 kvsSettings,
                CopyCompletionCallback copyHandler,
                ReplicationCallback replicationHandler,
                DataLossCallback onDataLossHandler,
                RestoreCompletedCallback restoreCompletedHandler)
                : base(
                    replicatorSettings,
                    kvsSettings)
            {
                this.copyHandler = copyHandler;
                this.replicationHandler = replicationHandler;
                this.onDataLossHandler = onDataLossHandler;
                this.restoreCompletedHandler = restoreCompletedHandler;
            }

            public void InitializeAndOverrideNativeStore(StatefulServiceInitializationParameters initParams)
            {
                this.Initialize_OverrideNativeKeyValueStore(initParams);
            }

            protected override void OnCopyComplete(KeyValueStoreEnumerator enumerator)
            {
                this.copyHandler(enumerator);
            }

            protected override void OnReplicationOperation(
                IEnumerator<KeyValueStoreNotification> enumerator)
            {
                this.replicationHandler(enumerator);
            }

            protected override Task<bool> OnDataLossAsync(CancellationToken cancellationToken)
            {
                return this.onDataLossHandler.Invoke(cancellationToken);
            }

            protected override Task OnRestoreCompletedAsync(CancellationToken cancellationToken)
            {
                return this.restoreCompletedHandler.Invoke(cancellationToken);
            }
        }
    }
}
