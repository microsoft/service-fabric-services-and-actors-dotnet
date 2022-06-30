// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;

    internal static class MigrationUtility
    {
        private static readonly string TraceType = typeof(MigrationUtility).ToString();

        public static bool ShouldRetryOperation(
            string currentExceptionId,
            int maxRetryCount,
            ref string lastSeenExceptionId,
            ref int currentRetryCount)
        {
            if (maxRetryCount == 0)
            {
                return false;
            }

            if (currentExceptionId == lastSeenExceptionId)
            {
                if (currentRetryCount >= maxRetryCount)
                {
                    // We have retried max number of times.
                    return false;
                }

                ++currentRetryCount;
                return true;
            }

            // The current retriable exception is different from the exception that was last seen,
            // reset the retry tracking variables
            lastSeenExceptionId = currentExceptionId;
            currentRetryCount = 1;
            return true;
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

        public static async Task<bool> ParseBoolAsync(Func<Task<string>> func, string traceId)
        {
            string valueString = await func();
            if (string.IsNullOrEmpty(valueString))
            {
                return false;
            }

            if (!bool.TryParse(valueString, out var value))
            {
                TraceAndThrowException(valueString, traceId);
            }

            return value;
        }

        public static async Task<T> ExecuteWithRetriesAsync<T>(Func<Task<T>> asyncFunc, string traceId, string funcTag, int retryCount = 0, IEnumerable<Type> retryableExceptions = null)
        {
            try
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    traceId,
                    $"Invoking migration func - {funcTag}");
                return await asyncFunc.Invoke();
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Migration func - {funcTag} failed with exception - {ex}");

                throw ex;
            }
            finally
            {
                ActorTrace.Source.WriteInfoWithId(
                   TraceType,
                   traceId,
                   $"Migration func - {funcTag} completed");
            }
        }

        public static T ExecuteWithRetriesAsync<T>(Func<T> func, string traceId, string funcTag, int retryCount = 0, IEnumerable<Type> retryableExceptions = null)
        {
            try
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    traceId,
                    $"Invoking migration func - {funcTag}");
                return func.Invoke();
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Migration func - {funcTag} failed with exception - {ex}");

                throw ex;
            }
            finally
            {
                ActorTrace.Source.WriteInfoWithId(
                   TraceType,
                   traceId,
                   $"Migration func - {funcTag} completed");
            }
        }

        public static async Task ExecuteWithRetriesAsync(Func<Task> asyncFunc, string traceId, string funcTag, int retryCount = 0, IEnumerable<Type> retryableExceptions = null)
        {
            await ExecuteWithRetriesAsync(
                async () =>
                {
                    await asyncFunc.Invoke();
                    return (object)null;
                },
                traceId,
                funcTag,
                retryCount,
                retryableExceptions);
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
