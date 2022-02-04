// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;

    internal static class MigrationConstants
    {
        internal static readonly TimeSpan DefaultRCTimeout = TimeSpan.FromMinutes(5);
        internal static readonly string KVSMigrationListenerName = "_KVSMigrationEP_";
        internal static readonly string KVSMigrationControllerName = "KvsMigration";
        internal static readonly string RCMigrationListenerName = "_RCMigrationEP_";
        internal static readonly string MetadataDictionaryName = "store://kvsrcmigration//metadata";
        internal static readonly string ResumeWritesAPIEndpoint = "ResumeWrites";
        internal static readonly string RejectWritesAPIEndpoint = "RejectWrites";
        internal static readonly string GetStartSNEndpoint = "GetFirstSequenceNumber";
        internal static readonly string GetEndSNEndpoint = "GetLastSequenceNumber";
        internal static readonly string EnumeratebySNEndpoint = "EnumerateBySequenceNumber";

        #region Global Migration constants
        internal static readonly string MigrationStartDateTimeUTC = "_MigrationStartDateTimeUTC_";
        internal static readonly string MigrationEndDateTimeUTC = "_MigrationEndDateTimeUTC_";
        internal static readonly string MigrationCurrentStatus = "_MigrationCurrentStatus_";
        internal static readonly string MigrationNoOfKeysMigrated = "_MigrationNoOfKeysMigrated_";
        internal static readonly string MigrationCurrentPhase = "_MigrationCurrentPhase_";
        internal static readonly string MigrationStartSeqNum = "_MigrationStartSeqNum_";
        internal static readonly string MigrationEndSeqNum = "_MigrationEndSeqNum_";
        internal static readonly string MigrationLastAppliedSeqNum = "_MigrationLastAppliedSeqNum_";
        #endregion Global Migration constants

        #region Phase constants
        internal static readonly string PhaseStartDateTimeUTC = "_{0}Phase_Iteration-{1}_StartDateTimeUTC_";
        internal static readonly string PhaseEndDateTimeUTC = "_{0}Phase_Iteration-{1}_EndDateTimeUTC_";
        internal static readonly string PhaseCurrentStatus = "_{0}Phase_Iteration-{1}_CurrentStatus_";
        internal static readonly string PhaseStartSeqNum = "_{0}Phase_Iteration-{1}_StartSeqNum_";
        internal static readonly string PhaseEndSeqNum = "_{0}Phase_Iteration-{1}_EndSeqNum_";
        internal static readonly string PhaseLastAppliedSeqNum = "_{0}Phase_Iteration-{1}_LastAppliedSeqNum_";
        internal static readonly string PhaseNoOfKeysMigrated = "_{0}Phase_Iteration-{1}_NoOfKeysMigrated_";
        internal static readonly string PhaseWorkerCount = "_{0}Phase_Iteration-{1}_WorkerCount_";
        internal static readonly string PhaseIterationCount = "_{0}Phase_IterationCount_";
        #endregion Phase constants

        #region Worker constants
        internal static readonly string PhaseWorkerStartDateTimeUTC = "_{0}Phase_Iteration-{1}_Worker{2}_StartDateTimeUTC_";
        internal static readonly string PhaseWorkerEndDateTimeUTC = "_{0}Phase_Iteration-{1}_Worker{2}_EndDateTimeUTC_";
        internal static readonly string PhaseWorkerCurrentStatus = "_{0}Phase_Iteration-{1}_Worker{2}_CurrentStatus_";
        internal static readonly string PhaseWorkerStartSeqNum = "_{0}Phase_Iteration-{1}_Worker{2}_StartSeqNum_";
        internal static readonly string PhaseWorkerEndSeqNum = "_{0}Phase_Iteration-{1}_Worker{2}_EndSeqNum_";
        internal static readonly string PhaseWorkerLastAppliedSeqNum = "_{0}Phase_Iteration-{1}_Worker{2}_LastAppliedSeqNum_";
        internal static readonly string PhaseWorkerNoOfKeysMigrated = "_{0}Phase_Iteration-{1}_Worker{2}_NoOfKeysMigrated_";
        #endregion Worker constants

        public static string Key(string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
