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
    /// Provides the ability to restore a replica's state from a backup.
    /// </summary>
    public struct RestoreContext
    {
        private readonly IStateProviderReplica stateProviderReplica;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreContext"/> struct.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="stateProviderReplica"/> is <see langword="null"/>.</exception>
        public RestoreContext(IStateProviderReplica stateProviderReplica)
        {
            this.stateProviderReplica = stateProviderReplica;
        }

        /// <inheritdoc path="/summary" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/remarks" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricMissingFullBackupException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.IO.DirectoryNotFoundException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.IO.FileNotFoundException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.IO.InvalidDataException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        /// <inheritdoc path="/exception[@cref='T:System.NotImplementedException']" cref="RestoreAsync(RestoreDescription, CancellationToken)"/>
        public Task RestoreAsync(RestoreDescription restoreDescription)
        {
            return this.stateProviderReplica.RestoreAsync(
                restoreDescription.BackupFolderPath, 
                restoreDescription.Policy, 
                CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously restores the replica's state from the backup described by <paramref name="restoreDescription"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API must be called from the callback assigned to <see cref="IStateProviderReplica.OnDataLossAsync"/>.
        /// Only one restore can be in flight per replica at a time.
        /// </para>
        /// <para>
        /// Exceptions thrown by this API differ depending on the underlying state provider. The exceptions that are currently
        /// documented for this API apply only to the out-of-box state providers that support restore: any Reliable Services
        /// state provider, the <c>KvsActorStateProvider</c> used for actor types with <c>StatePersistence.Persisted</c>
        /// (on .NET Framework and on Windows .NET), and the <c>ReliableCollectionsActorStateProvider</c> used for
        /// <c>StatePersistence.Persisted</c> on non-Windows .NET.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <para>
        /// The specified <see cref="RestoreDescription.BackupFolderPath"/> is <see langword="null"/>, empty, or contains only whitespace.
        /// </para>
        /// <para>
        /// For Reliable Services, this also occurs when <see cref="RestoreDescription.Policy"/> is set to <see cref="RestorePolicy.Safe"/>
        /// but the input backup folder contains a version that is not ahead of the state maintained in the current replica.
        /// </para>
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The supplied backup folder does not exist.
        /// </exception>
        /// <exception cref="FabricException">
        /// The restore operation failed. The <see cref="FabricException.ErrorCode"/> property indicates the specific reason.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="FabricErrorCode.InvalidBackup"/></term>
        ///         <description>
        ///         The backup files supplied in the backup folder are either missing or contain extra unexpected files.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="FabricErrorCode.InvalidRestoreData"/></term>
        ///         <description>
        ///         The metadata files (restore.dat) present in the backup folder are either corrupt or contain invalid information.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="FabricErrorCode.InvalidBackupChain"/></term>
        ///         <description>
        ///         The backup chain (i.e. one full backup and zero or more contiguous incremental backups that were taken after it)
        ///         supplied in the backup folder is broken.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="FabricErrorCode.DuplicateBackups"/></term>
        ///         <description>
        ///         The backup chain (i.e. one full backup and zero or more contiguous incremental backups that were taken after it)
        ///         supplied in the backup folder contains duplicate backups.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="FabricErrorCode.RestoreSafeCheckFailed"/></term>
        ///         <description>
        ///         <see cref="RestorePolicy.Safe"/> is specified as part of <see cref="RestoreDescription"/> and
        ///         the backup provided is not ahead of the state currently present in the service.
        ///         </description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <exception cref="FabricMissingFullBackupException">
        /// The input backup folder does not contain a full backup.
        /// For a backup folder to be restorable, it must contain exactly one full backup and any number of incremental backups.
        /// </exception>
        /// <exception cref="FabricObjectClosedException">
        /// The replica is closing.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The expected backup files under the supplied backup folder are not found.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// For Reliable Services, the backup or checkpoint data in the supplied backup folder is corrupt, such as when a
        /// backup file fails checksum verification or full-backup metadata is missing or inconsistent.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The call is made outside the callback assigned to <see cref="IStateProviderReplica.OnDataLossAsync"/>, another
        /// restore is already in flight on the same replica, or the restore is otherwise invalid for the target partition.
        /// For example, the <see cref="ServicePartitionKind"/> of the partition from which the backup was taken differs from
        /// that of the current partition being restored.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// The actor service is backed by <c>VolatileActorStateProvider</c> or <c>NullActorStateProvider</c>, neither of
        /// which supports restore.
        /// <c>NullActorStateProvider</c> is selected for actor types whose <c>[StatePersistence]</c> attribute specifies
        /// <c>StatePersistence.None</c> or that omit the attribute entirely;
        /// <c>VolatileActorStateProvider</c> is selected for <c>StatePersistence.Volatile</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">The operation was canceled via <paramref name="cancellationToken"/>.</exception>
        public Task RestoreAsync(RestoreDescription restoreDescription, CancellationToken cancellationToken)
        {
            return this.stateProviderReplica.RestoreAsync(
                restoreDescription.BackupFolderPath, 
                restoreDescription.Policy, 
                cancellationToken);
        }
    }
}
