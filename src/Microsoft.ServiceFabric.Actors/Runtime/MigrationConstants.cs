// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    internal static class MigrationConstants
    {
        public static readonly string MetadataDictionaryName = "store://kvsrcmigration//metadata";
        public static readonly string ResumeWritesAPIEndpoint = "/ResumeWrites";
        public static readonly string RejectWritesAPIEndpoint = "/RejectWrites";
        public static readonly string GetStartSNEndpoint = "/GetFirstSequenceNumberAsync";
        public static readonly string GetEndSNEndpoint = "/GetLastSequenceNumberAsync";
        public static readonly string EnumeratebySNEndpoint = "/EnumerateBySequenceNumber";
        public static readonly string EnumerateKeysAndTombstonesEndpoint = "/EnumerateKeysAndTombstones";

        public static readonly string MigrationPhaseKey = "MigrationPhase";
        public static readonly string MigrationStatusKey = "MigrationStatus";

        public static readonly string CopyWorkerCountKey = "WorkerCount";
        public static readonly string CopyPhaseStartSNKey = "Migration_Copy_StartSN";
        public static readonly string CopyPhaseEndSNKey = "Migration_Copy_EndSN";

        public static readonly string CatchupIterationKey = "Migration_Catchup_IterationCount";
        public static readonly string CatchupStartSNKey = "Migration_Catchup_StartSN";

        public static readonly string DowntimeWorkerStatusKey = "DowntimeWorker_status";
        public static readonly string DowntimeStartSNKey = "Migration_Downtime_StartSN";
        public static readonly string DowntimeEndSNKey = "Migration_Downtime_EndSN";
        public static readonly string DowntimeWorkerLastAppliedSNKey = "DowntimeWorker_LastAppliedSN";

        public static string GetCopyWorkerStatusKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_status";
        }

        public static string GetCopyWorkerStartSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_StartSN";
        }

        public static string GetCopyWorkerEndSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_EndSN";
        }

        public static string GetCopyWorkerLastAppliedSNKey(int workerIdentifier)
        {
            return "CopyWorker_" + workerIdentifier.ToString() + "_LastAppliedSN";
        }

        public static string GetCatchupWorkerStatusKey(int catchupCount)
        {
            return "CatchupWorker_" + catchupCount.ToString() + "_status";
        }

        public static string GetCatchupWorkerLastAppliedSNKey(int catchupCount)
        {
            return "CatchupWorker_" + catchupCount.ToString() + "_LastAppliedSN";
        }

        public static string GetCatchupWorkerEndSNKey(int catchupCount)
        {
            return "Migration_Catchup_" + catchupCount.ToString() + "_EndSN";
        }

        public static string GetCatchupWorkerStartSNKey(int catchupCount)
        {
            return "Migration_Catchup_" + catchupCount.ToString() + "_StartSN";
        }
    }
}
