// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal static class MigrationUtility
    {
        private static readonly string TraceType = typeof(MigrationUtility).ToString();

        public static string GetPhaseEndTelemetryKey(MigrationPhase phase)
        {
            if (phase == MigrationPhase.Copy)
            {
                return ActorTelemetryConstants.KVSToRCMigrationCopyPhaseEndEvent;
            }
            else if (phase == MigrationPhase.Catchup)
            {
                return ActorTelemetryConstants.KVSToRCMigrationCatchupPhaseEndEvent;
            }
            else if (phase == MigrationPhase.Downtime)
            {
                return ActorTelemetryConstants.KVSToRCMigrationDowntimePhaseEndEvent;
            }

            return string.Empty;
        }

        public static async Task<DateTime?> ParseDateTimeAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return null;
            }

            if (!DateTime.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<long?> ParseLongAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return null;
            }

            if (!long.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static long ParseLong(string valueString, string traceId)
        {
            if (!long.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<int> ParseIntAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (!int.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<int> ParseIntAsync(Func<Task<string>> func, int defaultValue, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return defaultValue;
            }

            if (!int.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<MigrationPhase> ParseMigrationPhaseAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return MigrationPhase.None;
            }

            if (!Enum.TryParse<MigrationPhase>(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<MigrationState> ParseMigrationStateAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return MigrationState.None;
            }

            if (!Enum.TryParse<MigrationState>(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        private static void TraceAndThrowException<TData>(TData data, string traceId)
        {
            ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Failed to parse {data}");

            throw new Exception($"Failed to parse {data}"); // TODO: SFException.
        }
    }
}
