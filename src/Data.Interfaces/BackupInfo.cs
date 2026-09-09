// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Fabric;
    using System.Globalization;

    /// <summary>
    /// Describes a backup produced by a Service Fabric Reliable State Provider.
    /// </summary>
    public class BackupInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupInfo"/> class without supplying the backup identifier, parent identifier, or start backup version.
        /// </summary>
        /// <inheritdoc path="/param[@name='directory']" cref="BackupInfo(string, BackupOption, BackupVersion, BackupVersion, Guid, Guid)"/>
        /// <inheritdoc path="/param[@name='option']" cref="BackupInfo(string, BackupOption, BackupVersion, BackupVersion, Guid, Guid)"/>
        /// <inheritdoc path="/param[@name='version']" cref="BackupInfo(string, BackupOption, BackupVersion, BackupVersion, Guid, Guid)"/>
        public BackupInfo(string directory, BackupOption option, BackupVersion version)
        {
            this.Directory = directory;
            this.Option = option;
            this.Version = version;
            this.StartBackupVersion = BackupVersion.InvalidBackupVersion;
            this.BackupId = Guid.Empty;
            this.ParentBackupId = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupInfo"/> class.
        /// </summary>
        /// <param name="directory">The folder path that contains the backup.</param>
        /// <param name="option">One of the enumeration values that specifies the kind of backup that was taken.</param>
        /// <param name="version">The latest <see cref="System.Fabric.Epoch"/> and LSN included in the backup.</param>
        /// <param name="startBackupVersion">The epoch and LSN of the first logical log record in the backup.</param>
        /// <param name="backupId">The identifier of this backup.</param>
        /// <param name="parentBackupId">The identifier of the backup used as the parent of this backup.</param>
        public BackupInfo(string directory, BackupOption option, BackupVersion version, BackupVersion startBackupVersion, Guid backupId, Guid parentBackupId)
        {
            this.Directory = directory;
            this.Option = option;
            this.Version = version;
            this.StartBackupVersion = startBackupVersion;
            this.BackupId = backupId;
            this.ParentBackupId = parentBackupId;
        }

        /// <summary>
        /// Gets the folder path that contains the backup.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Gets the kind of backup that was taken.
        /// </summary>
        public BackupOption Option { get; private set; }

        /// <summary>
        /// Gets the latest <see cref="System.Fabric.Epoch"/> and LSN included in the backup.
        /// </summary>
        public BackupVersion Version { get; private set; }

        /// <summary>
        /// Gets the identifier of this backup.
        /// </summary>
        /// <value>The backup identifier, or <see cref="Guid.Empty"/> when the instance was constructed without one.</value>
        public Guid BackupId { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Fabric.Epoch"/> and LSN of the first logical log record included in this backup.
        /// </summary>
        /// <value>The starting version, or <see cref="BackupVersion.InvalidBackupVersion"/> when the instance was constructed without one.</value>
        public BackupVersion StartBackupVersion { get; private set; }

        /// <summary>
        /// Gets the identifier of the parent backup.
        /// </summary>
        /// <value>The parent backup identifier, or <see cref="Guid.Empty"/> when the instance was constructed without one.</value>
        public Guid ParentBackupId { get; private set; }

        /// <summary>
        /// Returns a string representation of this <see cref="BackupInfo"/> that includes the backup folder and option.
        /// </summary>
        /// <remarks>The format is intended for diagnostics and may change between releases; do not parse it.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Backup folder: {0}, backup option: {1}", this.Directory, this.Option);
        }

        /// <summary>
        /// Represents a version of replicated state as an <see cref="System.Fabric.Epoch"/> and a logical sequence number (LSN).
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1815")]
        public struct BackupVersion : IComparable<BackupVersion>, IEquatable<BackupVersion>
        {
            /// <summary>
            /// Represents a <see cref="BackupVersion"/> with sentinel values that indicate the version is not valid.
            /// </summary>
            /// <remarks>Compare against this field rather than <c>default(BackupVersion)</c> to detect an unset version.</remarks>
            public static readonly BackupVersion InvalidBackupVersion = new BackupVersion(new Epoch(-1, -1), -1);

            private Epoch _epoch;
            private long _lsn;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackupVersion"/> struct.
            /// </summary>
            /// <param name="epoch">The <see cref="System.Fabric.Epoch"/> component of this version.</param>
            /// <param name="lsn">The logical sequence number (LSN) of this version.</param>
            public BackupVersion(Epoch epoch, long lsn)
            {
                this._epoch = epoch;
                this._lsn = lsn;
            }

            /// <summary>
            /// Gets the <see cref="System.Fabric.Epoch"/> component of this version.
            /// </summary>
            public Epoch Epoch { get { return this._epoch; } private set { this._epoch = value; } }


            /// <summary>
            /// Gets the logical sequence number (LSN) of this version.
            /// </summary>
            public long Lsn { get { return this._lsn; } private set { this._lsn = value; } }


            /// <inheritdoc/>
            /// <remarks>Versions are ordered by <see cref="Epoch"/> first and then by <see cref="Lsn"/>.</remarks>
            public int CompareTo(BackupVersion other)
            {
                var compareEpochResult = this.Epoch.CompareTo(other.Epoch);

                if (compareEpochResult != 0)
                {
                    return compareEpochResult;
                }

                return this.Lsn.CompareTo(other.Lsn);
            }

            /// <inheritdoc/>
            /// <remarks>Two versions are equal when they have the same <see cref="Epoch"/> and <see cref="Lsn"/>.</remarks>
            public bool Equals(BackupVersion other)
            {
                if (this.CompareTo(other) == 0)
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                if (obj is BackupVersion == false)
                {
                    return false;
                }

                return this.Equals((BackupVersion)obj);
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                // Note that Epoch.GetHashCode uses "+". Since it cannot be changed without breaking, I do not use this method.
                return this.Epoch.DataLossNumber.GetHashCode() ^ this.Epoch.ConfigurationNumber.GetHashCode() ^ this.Lsn.GetHashCode();
            }
        }
    }
}
