// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Fabric;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the members a reliable state provider replica must implement for Service Fabric to interact with it.
    /// </summary>
    public interface IStateProviderReplica
    {
        /// <summary>
        /// Sets the callback invoked during suspected data loss.
        /// </summary>
        /// <value>
        /// The callback that represents the asynchronous processing of the data-loss event. Returning
        /// <see langword="true"/> indicates the replica's state has been restored; <see langword="false"/> indicates it has
        /// not been changed.
        /// </value>
        /// <remarks>
        /// This callback is where <see cref="RestoreAsync(string)"/> may be invoked. It runs while the replica does not have
        /// read or write status, so reads and writes against the state providers are not permitted. Returning
        /// <see langword="true"/> causes the <see cref="ReplicaRole.Primary"/> to rebuild the other replicas in the partition from the restored state.
        /// </remarks>
        Func<CancellationToken, Task<bool>> OnDataLossAsync { set; }

        /// <summary>
        /// Initializes the state provider replica using the service initialization information.
        /// </summary>
        /// <param name="initializationParameters">The service initialization information such as service name, partition id, replica id, and code package information.</param>
        /// <remarks>
        /// No complex processing should be done during <see cref="Initialize(StatefulServiceInitializationParameters)"/>. Expensive or
        /// long-running initialization should be done in <see cref="OpenAsync(ReplicaOpenMode, IStatefulServicePartition, CancellationToken)"/>.
        /// </remarks>
        void Initialize(StatefulServiceInitializationParameters initializationParameters);

        /// <summary>
        /// Asynchronously opens the state provider replica for use and returns the replicator responsible for
        /// replicating state between other state provider replicas in the partition.
        /// </summary>
        /// <param name="openMode">One of the enumeration values that specifies whether this is a new or existing replica.</param>
        /// <param name="partition">The partition this replica belongs to.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <remarks>
        /// Extended state provider initialization tasks can be started at this time.
        /// </remarks>
        Task<IReplicator> OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition,
            CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously notifies the state provider replica that its role is changing, for example to <see cref="ReplicaRole.Primary"/> or Secondary.
        /// </summary>
        /// <param name="newRole">One of the enumeration values that specifies the new replica role, such as primary or secondary.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously closes the state provider replica gracefully.
        /// </summary>
        /// <remarks>
        /// This generally occurs when the replica's code is being upgraded, the replica is being moved
        /// due to load balancing, or a transient fault is detected.
        /// </remarks>
        Task CloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts the state provider replica forcefully.
        /// </summary>
        /// <remarks>
        /// This generally occurs when a permanent fault is detected on the node, or when
        /// Service Fabric cannot reliably manage the replica's life-cycle due to internal failures.
        /// </remarks>
        void Abort();

        /// <summary>
        /// Asynchronously performs a full backup of all reliable state managed by this replica.
        /// </summary>
        /// <inheritdoc path="/param[@name='backupCallback']" cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <remarks>
        /// A full backup will be performed with no timeout. To specify a timeout, use the
        /// <see cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/> overload.
        /// The Boolean returned by <paramref name="backupCallback"/> indicates whether the service successfully moved the backup folder to an external location.
        /// </remarks>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricBackupInProgressException']" cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        Task BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback);

        /// <summary>
        /// Asynchronously performs a backup of all reliable state managed by this replica.
        /// </summary>
        /// <param name="option">One of the enumeration values that specifies the type of backup to perform.</param>
        /// <param name="timeout">The timeout for this operation.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="backupCallback">The callback invoked when the backup folder has been created locally and is ready to be moved out of the node.</param>
        /// <remarks>
        /// The Boolean returned by <paramref name="backupCallback"/> indicates whether the service successfully moved the backup folder to an external location.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="backupCallback"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative (other than <see cref="Timeout.InfiniteTimeSpan"/>) or greater than <see cref="int.MaxValue"/> milliseconds.</exception>
        /// <exception cref="FabricBackupInProgressException">Another backup is already in progress.</exception>
        /// <exception cref="FabricMissingFullBackupException"><paramref name="option"/> is <see cref="BackupOption.Incremental"/> but no valid full backup exists to build upon.</exception>
        /// <exception cref="FabricNotPrimaryException">The replica is not a <see cref="ReplicaRole.Primary"/>, or is no longer the Primary.</exception>
        /// <exception cref="InvalidOperationException">The replica does not have persisted state, or <paramref name="backupCallback"/> returned <see langword="false"/> and the backup is marked unsuccessful.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        /// <exception cref="TimeoutException">The backup did not complete within <paramref name="timeout"/>.</exception>
        Task BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback);

        /// <inheritdoc path="/summary" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        /// <inheritdoc path="/param[@name='backupFolderPath']" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        /// <remarks>
        /// A safe restore will be performed, meaning the restore will only be completed if the data to restore is ahead of the current replica's state.
        /// </remarks>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricMissingFullBackupException']" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.IO.InvalidDataException']" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="RestoreAsync(string, RestorePolicy, CancellationToken)"/>
        Task RestoreAsync(string backupFolderPath);

        /// <summary>
        /// Asynchronously restores a backup taken by <see cref="BackupAsync(Func{BackupInfo, CancellationToken, Task{bool}})"/> or 
        /// <see cref="BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>.
        /// </summary>
        /// <param name="backupFolderPath">The directory to restore the replica from. UNC paths are supported.</param>
        /// <param name="restorePolicy">One of the enumeration values that specifies the policy applied when restoring from backup.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <exception cref="ArgumentException"><paramref name="backupFolderPath"/> is <see langword="null"/>, empty, whitespace, or is not a valid backup folder.</exception>
        /// <exception cref="FabricMissingFullBackupException"><paramref name="backupFolderPath"/> does not contain a valid full backup to use as the head of the backup chain.</exception>
        /// <exception cref="InvalidDataException">The backup data in <paramref name="backupFolderPath"/> is corrupt.</exception>
        /// <exception cref="InvalidOperationException">The method is invoked outside of <see cref="OnDataLossAsync"/> processing.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        Task RestoreAsync(
            string backupFolderPath,
            RestorePolicy restorePolicy,
            CancellationToken cancellationToken);
    }
}
