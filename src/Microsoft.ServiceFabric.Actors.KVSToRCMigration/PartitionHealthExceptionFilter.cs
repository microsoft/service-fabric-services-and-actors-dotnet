// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Health;

    internal class PartitionHealthExceptionFilter
    {
        private Dictionary<string, ExceptionInfo> exceptions;
        private MigrationSettings migrationSettings;

        public PartitionHealthExceptionFilter(MigrationSettings migrationSettings)
        {
            this.migrationSettings = migrationSettings;

            // All the exceptions with out an entry will be reported with health state warning and migration is aborted.
            this.exceptions = new Dictionary<string, ExceptionInfo>()
            {
                // Ignore
                { typeof(FabricNotPrimaryException).FullName, new ExceptionInfo(typeof(FabricNotPrimaryException).FullName, HealthState.Ok, false, false) },
                { typeof(OperationCanceledException).FullName, new ExceptionInfo(typeof(OperationCanceledException).FullName, HealthState.Ok, false, false) },

                // Warning
                //// Add warning list

                // Error
                //// Add error list
            };
        }

        public void ReportPartitionHealthIfNeeded(Exception exception, IStatefulServicePartition partition, out bool abortMigration)
        {
            var actual = exception;
            if (exception is AggregateException)
            {
                actual = ((AggregateException)exception).InnerException;
            }

            ExceptionInfo exceptionInfo = null;
            if (!this.exceptions.TryGetValue(actual.GetType().FullName, out exceptionInfo))
            {
                if (actual.Data.Contains("ActualExceptionType"))
                {
                    this.exceptions.TryGetValue((string)actual.Data["ActualExceptionType"], out exceptionInfo);
                }
            }

            if (exceptionInfo == null)
            {
                exceptionInfo = new ExceptionInfo(actual.GetType().FullName, HealthState.Warning, true, true);
            }

            var healthInfo = new HealthInformation("ActorStateMigration", "MigrationUnhandledException", exceptionInfo.HealthState)
            {
                TimeToLive = exceptionInfo.IsPermanentError ? TimeSpan.MaxValue : TimeSpan.FromMinutes(2),
                RemoveWhenExpired = false,
                Description = actual.Message,
            };

            partition.ReportPartitionHealth(healthInfo);

            abortMigration = exceptionInfo.AbortMigration;
        }

        private void PopulateExceptionListFromMigrationSettings()
        {
            if (!string.IsNullOrWhiteSpace(this.migrationSettings.ExceptionExclusionListForAbort))
            {
                var tokens = this.migrationSettings.ExceptionExclusionListForAbort.Trim().Split(',');
                foreach (var token in tokens)
                {
                    var type = token.Trim();
                    this.exceptions.Add(type, new ExceptionInfo(type, HealthState.Error, false, false));
                }
            }
        }

        public class ExceptionInfo
        {
            public ExceptionInfo(string exceptionType, HealthState healthState, bool abortMigration, bool isPermanentError)
            {
                this.ExceptionType = exceptionType;
                this.HealthState = healthState;
                this.AbortMigration = abortMigration;
                this.IsPermanentError = isPermanentError;
            }

            public string ExceptionType { get; set; }

            public HealthState HealthState { get; set; }

            public bool AbortMigration { get; set; }

            public bool IsPermanentError { get; set; }
        }
    }
}
