// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;

    using CopyCompletionCallback = System.Action<System.Fabric.KeyValueStoreEnumerator>;
    using ReplicationCallback = System.Action<System.Collections.Generic.IEnumerator<System.Fabric.KeyValueStoreNotification>>;
    using DataLossCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<bool>>;
    using RestoreCompletedCallback = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;

    /// <summary>
    /// Provides an implementation of <see cref="IActorStateProvider"/> which
    /// uses <see cref="KeyValueStoreReplica"/> to store and persist the actor state.
    /// </summary>
    public sealed class KvsActorStateProvider : KvsActorStateProviderBase
    {
        private readonly LocalStoreSettings userDefinedLocalStoreSettings;
        private readonly bool userDefinedEnableIncrementalBackup;
        private readonly int? userDefinedLogTruncationInterval;

        #region C'tors

        /// <summary>
        /// Creates an instance of <see cref="KvsActorStateProvider"/> with default settings.
        /// </summary>
        public KvsActorStateProvider()
            : this(null, null, false, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="KvsActorStateProvider"/> with specified
        /// replicator and local key-value store settings.
        /// </summary>
        /// <param name="replicatorSettings">
        /// A <see cref="ReplicatorSettings"/> that describes replicator settings.
        /// </param>
        /// <param name="localStoreSettings">
        /// A <see cref="LocalStoreSettings"/> that describes local key value store settings.
        /// </param>
        public KvsActorStateProvider(ReplicatorSettings replicatorSettings = null, LocalStoreSettings localStoreSettings = null)
            : this(replicatorSettings, localStoreSettings, false, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="KvsActorStateProvider"/> with specified settings.
        /// </summary>
        /// <param name="enableIncrementalBackup">
        /// Indicates whether to enable incremental backup feature.
        /// This sets the <see cref="LocalEseStoreSettings.EnableIncrementalBackup"/> setting.
        /// </param>
        public KvsActorStateProvider(bool enableIncrementalBackup)
            : this(null, null, enableIncrementalBackup, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="KvsActorStateProvider"/> with specified settings.
        /// </summary>
        /// <param name="enableIncrementalBackup">
        /// Indicates whether to enable incremental backup feature.
        /// This sets the <see cref="LocalEseStoreSettings.EnableIncrementalBackup"/> setting.
        /// </param>
        /// <param name="logTruncationIntervalInMinutes">
        /// Indicates the interval after which <see cref="KeyValueStoreReplica"/> tries to truncate local store logs.
        /// </param>
        /// <remarks>
        /// When an incremental backup is enabled for <see cref="KeyValueStoreReplica"/>, it does not use circular buffer
        /// to manage its transaction logs and periodically truncates the logs both on primary and secondary replica(s).
        /// The process of taking backup(s) automatically truncates logs. On the primary replica, if no user backup
        /// is initiated for <paramref name="logTruncationIntervalInMinutes"/>, <see cref="KeyValueStoreReplica"/>
        /// automatically truncates the logs.
        /// </remarks>
        public KvsActorStateProvider(bool enableIncrementalBackup, int logTruncationIntervalInMinutes)
            : this(null, null, enableIncrementalBackup, logTruncationIntervalInMinutes)
        {
        }

        private KvsActorStateProvider(
            ReplicatorSettings replicatorSettings,
            LocalStoreSettings localStoreSettings,
            bool enableIncrementalBackup,
            int? logTruncationIntervalInMinutes)
            : base(replicatorSettings)
        {
            this.userDefinedLocalStoreSettings = localStoreSettings;
            this.userDefinedEnableIncrementalBackup = enableIncrementalBackup;
            this.userDefinedLogTruncationInterval = logTruncationIntervalInMinutes;
        }

        #endregion

        internal override KeyValueStoreReplica OnCreateAndInitializeReplica(
            StatefulServiceInitializationParameters initParams,
            CopyCompletionCallback copyHandler,
            ReplicationCallback replicationHandler,
            DataLossCallback onDataLossHandler,
            RestoreCompletedCallback restoreCompletedHandler)
        {
            var kvs = new KeyValueStoreWrapper(
                this.GetLocalStoreSettings(),
                this.GetReplicatorSettings(),
                this.GetKvsReplicaSettings(),
                copyHandler,
                replicationHandler,
                onDataLossHandler,
                restoreCompletedHandler);

            kvs.Initialize(initParams);

            return kvs;
        }

        private LocalStoreSettings GetLocalStoreSettings()
        {
            // check if user provided the settings
            var settings = this.userDefinedLocalStoreSettings;

            if (settings == null)
            {
                // try load from configuration
                var configPackageName = ActorNameFormat.GetConfigPackageName(this.ActorTypeInformation.ImplementationType);
                var localEseStoreConfigSectionName = ActorNameFormat.GetLocalEseStoreConfigSectionName(this.ActorTypeInformation.ImplementationType);
                var configPackageObj = this.InitParams.CodePackageActivationContext.GetConfigurationPackageObject(configPackageName);

                if (configPackageObj.Settings.Sections.Contains(localEseStoreConfigSectionName))
                {
                    settings = LocalEseStoreSettings.LoadFrom(
                        this.InitParams.CodePackageActivationContext,
                        ActorNameFormat.GetConfigPackageName(this.ActorTypeInformation.ImplementationType),
                        localEseStoreConfigSectionName);
                }
            }

            if (settings == null)
            {
                settings = new LocalEseStoreSettings()
                {
                    MaxAsyncCommitDelay = TimeSpan.FromMilliseconds(100),
                    MaxVerPages = 8192 * 4,
                    EnableIncrementalBackup = this.userDefinedEnableIncrementalBackup,
                };
            }

            if (settings is LocalEseStoreSettings eseLocalStoreSettings)
            {
                if (string.IsNullOrEmpty(eseLocalStoreSettings.DbFolderPath))
                {
                    eseLocalStoreSettings.DbFolderPath = this.InitParams.CodePackageActivationContext.WorkDirectory;
                }
            }

            return settings;
        }

        private KeyValueStoreReplicaSettings GetKvsReplicaSettings()
        {
            var kvsReplicaSettings = new KeyValueStoreReplicaSettings
            {
                SecondaryNotificationMode = KeyValueStoreReplica.SecondaryNotificationMode.NonBlockingQuorumAcked,
            };

            if (this.userDefinedLogTruncationInterval.HasValue)
            {
                kvsReplicaSettings.LogTruncationIntervalInMinutes = this.userDefinedLogTruncationInterval.Value;
            }

            return kvsReplicaSettings;
        }

        private class KeyValueStoreWrapper : KeyValueStoreReplica
        {
            private readonly CopyCompletionCallback copyHandler;
            private readonly ReplicationCallback replicationHandler;
            private readonly DataLossCallback onDataLossHandler;
            private readonly RestoreCompletedCallback restoreCompletedHandler;

            public KeyValueStoreWrapper(
                LocalStoreSettings localStoreSettings,
                ReplicatorSettings replicatorSettings,
                KeyValueStoreReplicaSettings kvsSettings,
                CopyCompletionCallback copyHandler,
                ReplicationCallback replicationHandler,
                DataLossCallback onDataLossHandler,
                RestoreCompletedCallback restoreCompletedHandler)
                : base(
                    "ActorStateStore",
                    localStoreSettings,
                    replicatorSettings,
                    kvsSettings)
            {
                this.copyHandler = copyHandler;
                this.replicationHandler = replicationHandler;
                this.onDataLossHandler = onDataLossHandler;
                this.restoreCompletedHandler = restoreCompletedHandler;
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
