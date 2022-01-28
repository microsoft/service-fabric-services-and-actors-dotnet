// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    internal static class MigrationConstants
    {
        internal static readonly string KVSMigrationListenerName = "_KVSMigrationEP_";
        internal static readonly string KVSMigrationControllerName = "KvsMigration";
        internal static readonly string RCMigrationListenerName = "_RCMigrationEP_";
        internal static readonly string MetadataDictionaryName = "store://kvsrcmigration//metadata";
        internal static readonly string ResumeWritesAPIEndpoint = "ResumeWrites";
        internal static readonly string RejectWritesAPIEndpoint = "RejectWrites";
        internal static readonly string GetStartSNEndpoint = "GetFirstSequenceNumber";
        internal static readonly string GetEndSNEndpoint = "GetLastSequenceNumber";
        internal static readonly string EnumeratebySNEndpoint = "EnumerateBySequenceNumber";
        internal static readonly string EnumerateKeysAndTombstonesEndpoint = "EnumerateKeysAndTombstones";

        internal static readonly string MigrationPhaseKey = "MigrationPhase";
        internal static readonly string MigrationStatusKey = "MigrationStatus";

        internal static readonly string CopyWorkerCountKey = "WorkerCount";
        internal static readonly string CopyPhaseStartSNKey = "Migration_Copy_StartSN";
        internal static readonly string CopyPhaseEndSNKey = "Migration_Copy_EndSN";

        internal static readonly string CatchupIterationKey = "Migration_Catchup_IterationCount";
        internal static readonly string CatchupStartSNKey = "Migration_Catchup_StartSN";

        internal static readonly string DowntimeWorkerStatusKey = "DowntimeWorker_status";
        internal static readonly string DowntimeStartSNKey = "Migration_Downtime_StartSN";
        internal static readonly string DowntimeEndSNKey = "Migration_Downtime_EndSN";
        internal static readonly string DowntimeWorkerLastAppliedSNKey = "DowntimeWorker_LastAppliedSN";

        internal static string GetCopyWorkerStatusKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_status";
        }

        internal static string GetCopyWorkerStartSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_StartSN";
        }

        internal static string GetCopyWorkerEndSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_EndSN";
        }

        internal static string GetCopyWorkerLastAppliedSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_LastAppliedSN";
        }

        internal static string GetCatchupWorkerStatusKey(int catchupCount)
        {
            return "CatchupWorker_" + catchupCount.ToString() + "_status";
        }

        internal static string GetCatchupWorkerLastAppliedSNKey(int catchupCount)
        {
            return "CatchupWorker_" + catchupCount.ToString() + "_LastAppliedSN";
        }

        internal static string GetCatchupWorkerEndSNKey(int catchupCount)
        {
            return "Migration_Catchup_" + catchupCount.ToString() + "_EndSN";
        }

        internal static string GetCatchupWorkerStartSNKey(int catchupCount)
        {
            return "Migration_Catchup_" + catchupCount.ToString() + "_StartSN";
        }
    }
}
