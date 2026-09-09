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
    /// Corresponds to FABRIC_KEY_VALUE_STORE_ESE_COPY_DETAIL.
    /// </summary>
    public partial class KeyValueStoreEseReplicaCopyDetail : KeyValueStoreProviderCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the KeyValueStoreEseReplicaCopyDetail class.
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
        /// <param name="storeFormatVersion">The format version of the underlying Extensible Storage Engine (ESE) database
        /// engine used by the
        /// key value store replica.
        /// . Possible values include: 'Legacy', 'Hop1', 'Hop2Legacy'</param>
        /// <param name="copyType">The type of copy being performed for an ESE-backed key value store replica.
        /// . Possible values include: 'Unknown', 'Full', 'Partial'</param>
        /// <param name="copyTypeReason">The reason why a full or partial copy type was chosen for an ESE-backed key value
        /// store replica.
        /// . Possible values include: 'Unknown', 'InvalidSecondaryEpoch', 'EmptySecondary', 'FalseProgress',
        /// 'MatchedConfigurationNumber', 'EpochNotFound', 'StaleSecondary', 'PrimaryTombstonesNotTruncated'</param>
        /// <param name="copyMode">The copy mode used for an ESE-backed key value store replica build.
        /// . Possible values include: 'Unknown', 'Physical', 'Logical'</param>
        /// <param name="copyModeReason">The reason why a physical or logical copy mode was chosen for an ESE-backed key value
        /// store
        /// replica. Partial copies always use logical mode regardless of this value.
        /// . Possible values include: 'DefaultPhysicalCopy', 'LogicalCopyProbability', 'IncompatibleStoreFormatVersion',
        /// 'EmptyDatabase', 'FileStreamFullCopyNotSupportedBySecondary', 'FullCopyModeConfiguredAsLogical',
        /// 'EnableFileStreamFullCopySetToFalse'</param>
        public KeyValueStoreEseReplicaCopyDetail(
            Epoch primaryEpoch = default(Epoch),
            string primaryLastOperationSequenceNumber = default(string),
            bool? isCopyContextValid = default(bool?),
            Epoch secondaryEpoch = default(Epoch),
            string secondaryLastOperationSequenceNumber = default(string),
            KeyValueStoreEseFormat? storeFormatVersion = default(KeyValueStoreEseFormat?),
            KeyValueStoreReplicaCopyType? copyType = default(KeyValueStoreReplicaCopyType?),
            KeyValueStoreReplicaCopyTypeReason? copyTypeReason = default(KeyValueStoreReplicaCopyTypeReason?),
            KeyValueStoreReplicaCopyMode? copyMode = default(KeyValueStoreReplicaCopyMode?),
            KeyValueStoreReplicaCopyModeReason? copyModeReason = default(KeyValueStoreReplicaCopyModeReason?))
            : base(
                Common.KeyValueStoreProviderKind.Ese)
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
        /// Gets the format version of the underlying Extensible Storage Engine (ESE) database engine used by the
        /// key value store replica.
        /// . Possible values include: 'Legacy', 'Hop1', 'Hop2Legacy'
        /// </summary>
        public KeyValueStoreEseFormat? StoreFormatVersion { get; }

        /// <summary>
        /// Gets the type of copy being performed for an ESE-backed key value store replica.
        /// . Possible values include: 'Unknown', 'Full', 'Partial'
        /// </summary>
        public KeyValueStoreReplicaCopyType? CopyType { get; }

        /// <summary>
        /// Gets the reason why a full or partial copy type was chosen for an ESE-backed key value store replica.
        /// . Possible values include: 'Unknown', 'InvalidSecondaryEpoch', 'EmptySecondary', 'FalseProgress',
        /// 'MatchedConfigurationNumber', 'EpochNotFound', 'StaleSecondary', 'PrimaryTombstonesNotTruncated'
        /// </summary>
        public KeyValueStoreReplicaCopyTypeReason? CopyTypeReason { get; }

        /// <summary>
        /// Gets the copy mode used for an ESE-backed key value store replica build.
        /// . Possible values include: 'Unknown', 'Physical', 'Logical'
        /// </summary>
        public KeyValueStoreReplicaCopyMode? CopyMode { get; }

        /// <summary>
        /// Gets the reason why a physical or logical copy mode was chosen for an ESE-backed key value store
        /// replica. Partial copies always use logical mode regardless of this value.
        /// . Possible values include: 'DefaultPhysicalCopy', 'LogicalCopyProbability', 'IncompatibleStoreFormatVersion',
        /// 'EmptyDatabase', 'FileStreamFullCopyNotSupportedBySecondary', 'FullCopyModeConfiguredAsLogical',
        /// 'EnableFileStreamFullCopySetToFalse'
        /// </summary>
        public KeyValueStoreReplicaCopyModeReason? CopyModeReason { get; }
    }
}
