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
        private const int MaxHealthDescriptionLength = (4 * 1024) - 1;
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

            this.PopulateExceptionListFromMigrationSettings();
        }

        public void ReportPartitionHealth(Exception exception, IStatefulServicePartition partition, string healthMessage)
        {
            var actual = exception;
            if (exception is AggregateException)
            {
                actual = ((AggregateException)exception).InnerException;
            }

            var healthInfo = new HealthInformation("ActorStateMigration", "MigrationUnhandledException", HealthState.Warning)
            {
                TimeToLive = TimeSpan.MaxValue,
                RemoveWhenExpired = true,
                Description = this.GetPartitionHealthMesssage(actual, false, healthMessage),
            };

            partition.ReportPartitionHealth(healthInfo);
        }

        public void ReportPartitionHealthIfNeeded(Exception exception, IStatefulServicePartition partition, out bool abortMigration, out bool rethrow)
        {
            var actual = exception;
            if (exception is AggregateException)
            {
                actual = ((AggregateException)exception).InnerException;
            }

            var exceptionInfo = this.GetExceptionInfo(actual, out var rethrowT);
            var healthInfo = new HealthInformation("ActorStateMigration", "MigrationUnhandledException", exceptionInfo.HealthState)
            {
                TimeToLive = exceptionInfo.IsPermanentError ? TimeSpan.MaxValue : TimeSpan.FromMinutes(2),
                RemoveWhenExpired = true,
                Description = this.GetPartitionHealthMesssage(actual, exceptionInfo.AbortMigration, null),
            };

            partition.ReportPartitionHealth(healthInfo);
            abortMigration = exceptionInfo.AbortMigration;
            rethrow = rethrowT;
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

        private string GetPartitionHealthMesssage(Exception exception, bool abortMigration, string healthMessage)
        {
            string healthDesc = string.Empty;
            if (abortMigration)
            {
                healthDesc = "Aborting migration. ";
            }

            if (!string.IsNullOrEmpty(healthMessage))
            {
                healthDesc += $" {healthMessage}.";
            }

            healthDesc += $"Exception message : {exception.Message}";

            return healthDesc.Length <= MaxHealthDescriptionLength ? healthDesc : healthDesc.Substring(0, MaxHealthDescriptionLength);
        }

        private ExceptionInfo GetExceptionInfo(Exception exception, out bool rethrow)
        {
            ExceptionInfo exceptionInfo = null;
            rethrow = true;
            if (!this.exceptions.TryGetValue(exception.GetType().FullName, out exceptionInfo))
            {
                if (exception.Data.Contains("ActualExceptionType"))
                {
                    if (!this.exceptions.TryGetValue((string)exception.Data["ActualExceptionType"], out exceptionInfo))
                    {
                        exceptionInfo = new ExceptionInfo
                        {
                            ExceptionType = (string)exception.Data["ActualExceptionType"],
                            AbortMigration = true,
                            HealthState = HealthState.Warning,
                            IsPermanentError = true,
                        };
                    }

                    // Exception at source. Need not rethrow
                    rethrow = false;
                }
                else
                {
                    exceptionInfo = new ExceptionInfo
                    {
                        ExceptionType = exception.GetType().FullName,
                        AbortMigration = true,
                        HealthState = HealthState.Warning,
                        IsPermanentError = true,
                    };
                }
            }

            return exceptionInfo;
        }

        public class ExceptionInfo
        {
            public ExceptionInfo()
            {
            }

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
