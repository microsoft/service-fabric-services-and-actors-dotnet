// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains the information necessary to back up a stateful service replica.
    /// </summary>
    public struct BackupDescription
    {
        private readonly BackupOption option;
        private readonly Func<BackupInfo, CancellationToken, Task<bool>> backupCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupDescription"/> struct.
        /// </summary>
        /// <inheritdoc path="/param[@name='backupCallback']" cref="BackupDescription(BackupOption, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="BackupDescription(BackupOption, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// <remarks>
        /// Uses <see cref="BackupOption.Full"/> for the backup option.
        /// </remarks>
        public BackupDescription(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            this.option = BackupOption.Full;
            this.backupCallback = backupCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupDescription"/> struct.
        /// </summary>
        /// <param name="option">One of the enumeration values that specifies the kind of backup to perform.</param>
        /// <param name="backupCallback">
        /// A callback invoked when the backup folder has been created locally and is ready to be moved out of the node.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="backupCallback"/> is <see langword="null"/>.</exception>
        public BackupDescription(BackupOption option, Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            this.option = option;
            this.backupCallback = backupCallback;
        }

        /// <summary>
        /// Gets the kind of backup to perform.
        /// </summary>
        public BackupOption Option
        {
            get
            {
                return this.option;
            }
        }

        /// <summary>
        /// Gets the callback invoked when the backup folder has been created locally and is ready to be moved out of the node.
        /// </summary>
        /// <remarks>
        /// When invoked, the callback returns a <see cref="Task{TResult}"/> whose result indicates whether the service was able to successfully move the backup folder to an external location.
        /// If <see langword="false"/> is returned,
        /// <see cref="IStateProviderReplica.BackupAsync(BackupOption, TimeSpan, CancellationToken, Func{BackupInfo, CancellationToken, Task{bool}})"/>
        /// throws <see cref="InvalidOperationException"/>, and the backup is marked as unsuccessful.
        /// </remarks>
        public Func<BackupInfo, CancellationToken, Task<bool>> BackupCallback
        {
            get
            {
                return this.backupCallback;
            }
        }
    }
}
