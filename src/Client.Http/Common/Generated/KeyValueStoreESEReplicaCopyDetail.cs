// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Detailed metadata about the copy operation for an ESE-backed key value store inbuild replica.
    /// Includes information about both the primary and secondary state, the type and mode of copy
    /// being performed, and the reasons why certain copy decisions were made.
    /// </summary>
    public partial class KeyValueStoreESEReplicaCopyDetail : KeyValueStoreProviderCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the KeyValueStoreESEReplicaCopyDetail class.
        /// </summary>
        /// <param name="primaryEpoch">The primary replica's current epoch at the time the copy operation was initiated.
        /// Contains data loss number and configuration number.
        /// </param>
        /// <param name="primaryLastOperationSequenceNumber">The highest LSN the primary had processed when the copy operation
        /// began. The secondary
        /// will need to build to this point during the Copy phase.
        /// </param>
        /// <param name="isCopyContextValid">Whether the secondary provided valid copy context. If false, the secondary either
        /// sent
        /// no context (brand new replica) or invalid context data, which typically results in a full
        /// copy. If true, the secondary provided valid state information allowing the primary to
        /// determine whether a partial (incremental) copy is possible.
        /// </param>
        /// <param name="secondaryEpoch">The secondary replica's epoch as reported in its copy context. This epoch is compared
        /// with the primary's epoch history to determine if a full or partial copy is needed.
        /// </param>
        /// <param name="secondaryLastOperationSequenceNumber">The highest LSN the secondary had processed, as reported in its
        /// copy context. The
        /// primary uses this to determine the starting point for a partial copy or to decide if
        /// a full copy is needed.
        /// </param>
        /// <param name="storeFormatVersion">The KVS store schema format version used by the key value store replica. This is a
        /// Service
        /// Fabric-defined version tag — it is independent of the underlying ESE engine version. If
        /// the secondary's format is incompatible with the primary's, a logical (row-by-row) copy is
        /// required even when a physical (file streaming) copy would otherwise be possible.
        /// - Legacy - The original ESE database format (LOCAL_STORE_FORMAT_LEGACY). The value is 0.
        /// - Hop1 - First-generation updated ESE format (LOCAL_STORE_FORMAT_HOP1). The value is 1.
        /// - Hop2Legacy - Second-generation ESE format with legacy compatibility
        /// (LOCAL_STORE_FORMAT_HOP2_LEGACY). The value is 2.
        /// . Possible values include: 'Legacy', 'Hop1', 'Hop2Legacy'</param>
        /// <param name="copyType">The type of copy being performed for an ESE-backed key value store replica.
        /// - Unknown - The copy type has not been determined. The value is 0.
        /// - Full - All state data is copied from primary to secondary. Occurs for new replicas,
        /// replicas with invalid or missing copy context, or when the secondary has fallen too
        /// far behind for incremental copy. The value is 1.
        /// - Partial - Only incremental changes since the secondary's last known sequence number
        /// are copied. Requires the secondary to have valid copy context with an epoch found in
        /// the primary's epoch history. The value is 2.
        /// . Possible values include: 'Unknown', 'Full', 'Partial'</param>
        /// <param name="copyTypeReason">The reason why a full or partial copy type was chosen for an ESE-backed key value
        /// store replica.
        /// - Unknown - The reason has not been determined. The value is 0.
        /// - InvalidSecondaryEpoch - The secondary's epoch is invalid or corrupted, requiring a full
        /// copy. The value is 1.
        /// - EmptySecondary - The secondary has no data (brand new replica), requiring a full copy.
        /// The value is 2.
        /// - FalseProgress - The secondary's LSN is higher than the primary's history allows, indicating
        /// the secondary made progress on operations the primary does not recognize. Full copy
        /// required. The value is 3.
        /// - MatchedConfigurationNumber - The secondary's configuration number matches an entry in the
        /// primary's epoch history, allowing a partial (incremental) copy. The value is 4.
        /// - EpochNotFound - The secondary's epoch is not found in the primary's epoch history,
        /// requiring a full copy. The value is 5.
        /// - StaleSecondary - The secondary has fallen too far behind and the primary no longer has
        /// the operations needed for incremental copy. The value is 6.
        /// - PrimaryTombstonesNotTruncated - The primary's tombstone data has not been truncated yet,
        /// requiring a full copy. The value is 7.
        /// . Possible values include: 'Unknown', 'InvalidSecondaryEpoch', 'EmptySecondary', 'FalseProgress',
        /// 'MatchedConfigurationNumber', 'EpochNotFound', 'StaleSecondary', 'PrimaryTombstonesNotTruncated'</param>
        /// <param name="copyMode">The copy mode used for an ESE-backed key value store replica build.
        /// - Unknown - The copy mode has not been determined. The value is 0.
        /// - Physical - The primary streams the ESE database file directly to the secondary. Faster
        /// than logical mode but requires compatible store formats between primary and secondary.
        /// Only used for full copies. The value is 1.
        /// - Logical - The primary transfers state to the secondary row-by-row as individual
        /// operations. Required when store formats are incompatible or when performing a partial
        /// copy. The value is 2.
        /// . Possible values include: 'Unknown', 'Physical', 'Logical'</param>
        /// <param name="copyModeReason">The reason why a physical or logical copy mode was chosen for an ESE-backed key value
        /// store
        /// replica. Partial copies always use logical mode regardless of this value.
        /// - DefaultPhysicalCopy - Physical copy is the default mode for full copies when all
        /// conditions allow it. The value is 0.
        /// - LogicalCopyProbability - Logical copy was randomly selected based on configured
        /// probability for testing and validation purposes. The value is 1.
        /// - IncompatibleStoreFormatVersion - The secondary's store format version is incompatible
        /// with the primary's, requiring logical copy. The value is 2.
        /// - EmptyDatabase - The primary database is empty, so physical copy is not needed.
        /// The value is 3.
        /// - FileStreamFullCopyNotSupportedBySecondary - The secondary replica does not support the
        /// file stream full copy protocol. The value is 4.
        /// - FullCopyModeConfiguredAsLogical - The FullCopyMode configuration is explicitly set to
        /// Logical. The value is 5.
        /// - EnableFileStreamFullCopySetToFalse - The EnableFileStreamFullCopy configuration setting
        /// is disabled. The value is 6.
        /// . Possible values include: 'DefaultPhysicalCopy', 'LogicalCopyProbability', 'IncompatibleStoreFormatVersion',
        /// 'EmptyDatabase', 'FileStreamFullCopyNotSupportedBySecondary', 'FullCopyModeConfiguredAsLogical',
        /// 'EnableFileStreamFullCopySetToFalse'</param>
        public KeyValueStoreESEReplicaCopyDetail(
            Epoch primaryEpoch = default(Epoch),
            string primaryLastOperationSequenceNumber = default(string),
            bool? isCopyContextValid = default(bool?),
            Epoch secondaryEpoch = default(Epoch),
            string secondaryLastOperationSequenceNumber = default(string),
            KeyValueStoreEseFormat? storeFormatVersion = default(KeyValueStoreEseFormat?),
            KvsReplicaCopyType? copyType = default(KvsReplicaCopyType?),
            KvsReplicaCopyTypeReason? copyTypeReason = default(KvsReplicaCopyTypeReason?),
            KvsReplicaCopyMode? copyMode = default(KvsReplicaCopyMode?),
            KvsReplicaCopyModeReason? copyModeReason = default(KvsReplicaCopyModeReason?))
            : base(
                Common.KeyValueStoreProviderKind.ESE)
        {
            this.PrimaryEpoch = primaryEpoch;
            this.PrimaryLastOperationSequenceNumber = primaryLastOperationSequenceNumber;
            this.IsCopyContextValid = isCopyContextValid;
            this.SecondaryEpoch = secondaryEpoch;
            this.SecondaryLastOperationSequenceNumber = secondaryLastOperationSequenceNumber;
            this.StoreFormatVersion = storeFormatVersion;
            this.CopyType = copyType;
            this.CopyTypeReason = copyTypeReason;
            this.CopyMode = copyMode;
            this.CopyModeReason = copyModeReason;
        }

        /// <summary>
        /// Gets the primary replica's current epoch at the time the copy operation was initiated.
        /// Contains data loss number and configuration number.
        /// </summary>
        public Epoch PrimaryEpoch { get; }

        /// <summary>
        /// Gets the highest LSN the primary had processed when the copy operation began. The secondary
        /// will need to build to this point during the Copy phase.
        /// </summary>
        public string PrimaryLastOperationSequenceNumber { get; }

        /// <summary>
        /// Gets whether the secondary provided valid copy context. If false, the secondary either sent
        /// no context (brand new replica) or invalid context data, which typically results in a full
        /// copy. If true, the secondary provided valid state information allowing the primary to
        /// determine whether a partial (incremental) copy is possible.
        /// </summary>
        public bool? IsCopyContextValid { get; }

        /// <summary>
        /// Gets the secondary replica's epoch as reported in its copy context. This epoch is compared
        /// with the primary's epoch history to determine if a full or partial copy is needed.
        /// </summary>
        public Epoch SecondaryEpoch { get; }

        /// <summary>
        /// Gets the highest LSN the secondary had processed, as reported in its copy context. The
        /// primary uses this to determine the starting point for a partial copy or to decide if
        /// a full copy is needed.
        /// </summary>
        public string SecondaryLastOperationSequenceNumber { get; }

        /// <summary>
        /// Gets the KVS store schema format version used by the key value store replica. This is a Service
        /// Fabric-defined version tag — it is independent of the underlying ESE engine version. If
        /// the secondary's format is incompatible with the primary's, a logical (row-by-row) copy is
        /// required even when a physical (file streaming) copy would otherwise be possible.
        /// - Legacy - The original ESE database format (LOCAL_STORE_FORMAT_LEGACY). The value is 0.
        /// - Hop1 - First-generation updated ESE format (LOCAL_STORE_FORMAT_HOP1). The value is 1.
        /// - Hop2Legacy - Second-generation ESE format with legacy compatibility
        /// (LOCAL_STORE_FORMAT_HOP2_LEGACY). The value is 2.
        /// . Possible values include: 'Legacy', 'Hop1', 'Hop2Legacy'
        /// </summary>
        public KeyValueStoreEseFormat? StoreFormatVersion { get; }

        /// <summary>
        /// Gets the type of copy being performed for an ESE-backed key value store replica.
        /// - Unknown - The copy type has not been determined. The value is 0.
        /// - Full - All state data is copied from primary to secondary. Occurs for new replicas,
        /// replicas with invalid or missing copy context, or when the secondary has fallen too
        /// far behind for incremental copy. The value is 1.
        /// - Partial - Only incremental changes since the secondary's last known sequence number
        /// are copied. Requires the secondary to have valid copy context with an epoch found in
        /// the primary's epoch history. The value is 2.
        /// . Possible values include: 'Unknown', 'Full', 'Partial'
        /// </summary>
        public KvsReplicaCopyType? CopyType { get; }

        /// <summary>
        /// Gets the reason why a full or partial copy type was chosen for an ESE-backed key value store replica.
        /// - Unknown - The reason has not been determined. The value is 0.
        /// - InvalidSecondaryEpoch - The secondary's epoch is invalid or corrupted, requiring a full
        /// copy. The value is 1.
        /// - EmptySecondary - The secondary has no data (brand new replica), requiring a full copy.
        /// The value is 2.
        /// - FalseProgress - The secondary's LSN is higher than the primary's history allows, indicating
        /// the secondary made progress on operations the primary does not recognize. Full copy
        /// required. The value is 3.
        /// - MatchedConfigurationNumber - The secondary's configuration number matches an entry in the
        /// primary's epoch history, allowing a partial (incremental) copy. The value is 4.
        /// - EpochNotFound - The secondary's epoch is not found in the primary's epoch history,
        /// requiring a full copy. The value is 5.
        /// - StaleSecondary - The secondary has fallen too far behind and the primary no longer has
        /// the operations needed for incremental copy. The value is 6.
        /// - PrimaryTombstonesNotTruncated - The primary's tombstone data has not been truncated yet,
        /// requiring a full copy. The value is 7.
        /// . Possible values include: 'Unknown', 'InvalidSecondaryEpoch', 'EmptySecondary', 'FalseProgress',
        /// 'MatchedConfigurationNumber', 'EpochNotFound', 'StaleSecondary', 'PrimaryTombstonesNotTruncated'
        /// </summary>
        public KvsReplicaCopyTypeReason? CopyTypeReason { get; }

        /// <summary>
        /// Gets the copy mode used for an ESE-backed key value store replica build.
        /// - Unknown - The copy mode has not been determined. The value is 0.
        /// - Physical - The primary streams the ESE database file directly to the secondary. Faster
        /// than logical mode but requires compatible store formats between primary and secondary.
        /// Only used for full copies. The value is 1.
        /// - Logical - The primary transfers state to the secondary row-by-row as individual
        /// operations. Required when store formats are incompatible or when performing a partial
        /// copy. The value is 2.
        /// . Possible values include: 'Unknown', 'Physical', 'Logical'
        /// </summary>
        public KvsReplicaCopyMode? CopyMode { get; }

        /// <summary>
        /// Gets the reason why a physical or logical copy mode was chosen for an ESE-backed key value store
        /// replica. Partial copies always use logical mode regardless of this value.
        /// - DefaultPhysicalCopy - Physical copy is the default mode for full copies when all
        /// conditions allow it. The value is 0.
        /// - LogicalCopyProbability - Logical copy was randomly selected based on configured
        /// probability for testing and validation purposes. The value is 1.
        /// - IncompatibleStoreFormatVersion - The secondary's store format version is incompatible
        /// with the primary's, requiring logical copy. The value is 2.
        /// - EmptyDatabase - The primary database is empty, so physical copy is not needed.
        /// The value is 3.
        /// - FileStreamFullCopyNotSupportedBySecondary - The secondary replica does not support the
        /// file stream full copy protocol. The value is 4.
        /// - FullCopyModeConfiguredAsLogical - The FullCopyMode configuration is explicitly set to
        /// Logical. The value is 5.
        /// - EnableFileStreamFullCopySetToFalse - The EnableFileStreamFullCopy configuration setting
        /// is disabled. The value is 6.
        /// . Possible values include: 'DefaultPhysicalCopy', 'LogicalCopyProbability', 'IncompatibleStoreFormatVersion',
        /// 'EmptyDatabase', 'FileStreamFullCopyNotSupportedBySecondary', 'FullCopyModeConfiguredAsLogical',
        /// 'EnableFileStreamFullCopySetToFalse'
        /// </summary>
        public KvsReplicaCopyModeReason? CopyModeReason { get; }
    }
}
