// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the status of an inbuild replica from the primary replicator's perspective.
    /// An inbuild replica is a secondary replica that is being built from the primary through a copy process.
    /// This tracks the progress and state of that build operation through phases:
    /// CopyContext (get initial state of the inbuild secondary),
    /// CopyState (get current state of the primary at the start of the copy),
    /// Copy (make decisions on the type and mode of copy and transfer data),
    /// CopyCatchup (catch up on operations received by the primary since the start of the copy),
    /// and CopyComplete (build finished).
    /// </summary>
    public partial class RemoteInbuildReplicaStatus
    {
        /// <summary>
        /// Initializes a new instance of the RemoteInbuildReplicaStatus class.
        /// </summary>
        /// <param name="inbuildPhase">The current phase of the inbuild process. The inbuild process progresses sequentially
        /// through
        /// CopyContext, CopyState, Copy, CopyCatchup, and CopyComplete.
        /// . Possible values include: 'CopyContext', 'CopyState', 'Copy', 'CopyCatchup', 'CopyComplete'
        /// 
        /// The phase of the inbuild process while a secondary replica is brought up to date with the
        /// primary's state, from initial context exchange through data transfer and catch-up to build
        /// completion.
        /// 
        /// </param>
        /// <param name="copyContextPhase">The sub-phase within the CopyContext phase. Only relevant when InbuildPhase is
        /// CopyContext.
        /// Tracks the initial connection establishment and copy state retrieval from the secondary.
        /// . Possible values include: 'EstablishConnection', 'GetCopyContext'
        /// 
        /// The sub-phase within the initial CopyContext phase of the inbuild process, progressing from
        /// creating the replication channel to collecting secondary-side state metadata. This metadata is
        /// used by the primary to decide whether copy can be incremental or must be full.
        /// 
        /// </param>
        /// <param name="lastCopySequenceNumber">The last sequence number that has been quorum committed when the primary
        /// receives the build
        /// secondary request. Data up to and including this LSN will be sent to the secondary during
        /// the Copy phase. Returns -1 if the Copy phase has not started.
        /// </param>
        /// <param name="lastCopyCatchupSequenceNumber">During the CopyCatchup phase, following the Copy phase, the secondary
        /// replica applies
        /// replication operations it received from the primary while the Copy phase was in-progress.
        /// This captures the last sequence number that has been quorum committed when the primary
        /// sends the LastCopySequenceNumber to the secondary. The replica is considered built when
        /// the secondary has caught up to this sequence number. Returns 0 if the CopyCatchup phase
        /// has not started.
        /// </param>
        /// <param name="copyContextPhaseStartTimeUtc">The UTC timestamp when the primary starts to establish connection with
        /// the inbuild secondary
        /// with intent to build the replica. Returns DateTime.MinValue if not yet started.
        /// </param>
        /// <param name="copyStatePhaseStartTimeUtc">The UTC timestamp when the primary replicator calls its local state
        /// provider to retrieve
        /// the primary's current state or copy context to determine the operations that need to be
        /// copied to the secondary. This happens after the secondary's copy context has been obtained
        /// in the CopyContext phase but before the actual streaming of operations begins in the Copy
        /// phase. Returns DateTime.MinValue if not yet started.
        /// </param>
        /// <param name="copyPhaseStartTimeUtc">The UTC timestamp when the primary starts transferring actual data to the
        /// secondary.
        /// The Copy phase is where the bulk of the replica's state is transmitted from primary to
        /// secondary. Returns DateTime.MinValue if not yet started.
        /// </param>
        /// <param name="copyCatchupPhaseStartTimeUtc">The UTC timestamp when the secondary starts applying replication
        /// operations that it
        /// received from the primary during the Copy phase. Returns DateTime.MinValue if not yet
        /// started.
        /// </param>
        /// <param name="copyDetails">Store-specific details about the copy operation being performed for this inbuild replica.
        /// Only populated for key value store backed replicas; null otherwise.
        /// </param>
        public RemoteInbuildReplicaStatus(
            InbuildReplicaPhase? inbuildPhase = default(InbuildReplicaPhase?),
            InbuildReplicaCopyContextPhase? copyContextPhase = default(InbuildReplicaCopyContextPhase?),
            string lastCopySequenceNumber = default(string),
            string lastCopyCatchupSequenceNumber = default(string),
            DateTime? copyContextPhaseStartTimeUtc = default(DateTime?),
            DateTime? copyStatePhaseStartTimeUtc = default(DateTime?),
            DateTime? copyPhaseStartTimeUtc = default(DateTime?),
            DateTime? copyCatchupPhaseStartTimeUtc = default(DateTime?),
            InbuildReplicaCopyDetail copyDetails = default(InbuildReplicaCopyDetail))
        {
            this.InbuildPhase = inbuildPhase;
            this.CopyContextPhase = copyContextPhase;
            this.LastCopySequenceNumber = lastCopySequenceNumber;
            this.LastCopyCatchupSequenceNumber = lastCopyCatchupSequenceNumber;
            this.CopyContextPhaseStartTimeUtc = copyContextPhaseStartTimeUtc;
            this.CopyStatePhaseStartTimeUtc = copyStatePhaseStartTimeUtc;
            this.CopyPhaseStartTimeUtc = copyPhaseStartTimeUtc;
            this.CopyCatchupPhaseStartTimeUtc = copyCatchupPhaseStartTimeUtc;
            this.CopyDetails = copyDetails;
        }

        /// <summary>
        /// Gets the current phase of the inbuild process. The inbuild process progresses sequentially through
        /// CopyContext, CopyState, Copy, CopyCatchup, and CopyComplete.
        /// . Possible values include: 'CopyContext', 'CopyState', 'Copy', 'CopyCatchup', 'CopyComplete'
        /// 
        /// The phase of the inbuild process while a secondary replica is brought up to date with the
        /// primary's state, from initial context exchange through data transfer and catch-up to build
        /// completion.
        /// </summary>
        public InbuildReplicaPhase? InbuildPhase { get; }

        /// <summary>
        /// Gets the sub-phase within the CopyContext phase. Only relevant when InbuildPhase is CopyContext.
        /// Tracks the initial connection establishment and copy state retrieval from the secondary.
        /// . Possible values include: 'EstablishConnection', 'GetCopyContext'
        /// 
        /// The sub-phase within the initial CopyContext phase of the inbuild process, progressing from
        /// creating the replication channel to collecting secondary-side state metadata. This metadata is
        /// used by the primary to decide whether copy can be incremental or must be full.
        /// </summary>
        public InbuildReplicaCopyContextPhase? CopyContextPhase { get; }

        /// <summary>
        /// Gets the last sequence number that has been quorum committed when the primary receives the build
        /// secondary request. Data up to and including this LSN will be sent to the secondary during
        /// the Copy phase. Returns -1 if the Copy phase has not started.
        /// </summary>
        public string LastCopySequenceNumber { get; }

        /// <summary>
        /// Gets during the CopyCatchup phase, following the Copy phase, the secondary replica applies
        /// replication operations it received from the primary while the Copy phase was in-progress.
        /// This captures the last sequence number that has been quorum committed when the primary
        /// sends the LastCopySequenceNumber to the secondary. The replica is considered built when
        /// the secondary has caught up to this sequence number. Returns 0 if the CopyCatchup phase
        /// has not started.
        /// </summary>
        public string LastCopyCatchupSequenceNumber { get; }

        /// <summary>
        /// Gets the UTC timestamp when the primary starts to establish connection with the inbuild secondary
        /// with intent to build the replica. Returns DateTime.MinValue if not yet started.
        /// </summary>
        public DateTime? CopyContextPhaseStartTimeUtc { get; }

        /// <summary>
        /// Gets the UTC timestamp when the primary replicator calls its local state provider to retrieve
        /// the primary's current state or copy context to determine the operations that need to be
        /// copied to the secondary. This happens after the secondary's copy context has been obtained
        /// in the CopyContext phase but before the actual streaming of operations begins in the Copy
        /// phase. Returns DateTime.MinValue if not yet started.
        /// </summary>
        public DateTime? CopyStatePhaseStartTimeUtc { get; }

        /// <summary>
        /// Gets the UTC timestamp when the primary starts transferring actual data to the secondary.
        /// The Copy phase is where the bulk of the replica's state is transmitted from primary to
        /// secondary. Returns DateTime.MinValue if not yet started.
        /// </summary>
        public DateTime? CopyPhaseStartTimeUtc { get; }

        /// <summary>
        /// Gets the UTC timestamp when the secondary starts applying replication operations that it
        /// received from the primary during the Copy phase. Returns DateTime.MinValue if not yet
        /// started.
        /// </summary>
        public DateTime? CopyCatchupPhaseStartTimeUtc { get; }

        /// <summary>
        /// Gets store-specific details about the copy operation being performed for this inbuild replica.
        /// Only populated for key value store backed replicas; null otherwise.
        /// </summary>
        public InbuildReplicaCopyDetail CopyDetails { get; }
    }
}
