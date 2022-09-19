// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data.Collections;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;

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

        public static async Task<string> GetValueOrDefaultAsync(ActorStateProviderHelper stateProviderHelper, Func<Data.ITransaction> txFactory, IReliableDictionary2<string, string> metadataDict, string key, CancellationToken cancellationToken)
        {
            return await stateProviderHelper.ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var tx = txFactory.Invoke())
                    {
                        var res = await metadataDict.TryGetValueAsync(
                                tx,
                                key,
                                DefaultRCTimeout,
                                cancellationToken);
                        return res.HasValue ? res.Value : null;
                    }
                },
                $"MigrationPhaseWorkloadBase.TryGetValueAsync.{key}",
                cancellationToken);
        }

        public static async Task<string> GetValueAsync(ActorStateProviderHelper stateProviderHelper, Func<Data.ITransaction> txFactory, IReliableDictionary2<string, string> metadataDict, string key, CancellationToken cancellationToken)
        {
            return await stateProviderHelper.ExecuteWithRetriesAsync(
                    async () =>
                    {
                        using (var tx = txFactory.Invoke())
                        {
                            var res = await metadataDict.TryGetValueAsync(
                                tx,
                                key,
                                DefaultRCTimeout,
                                cancellationToken);
                            if (res.HasValue)
                            {
                                return res.Value;
                            }

                            throw new KeyNotFoundException(key);
                        }
                    },
                    $"MigrationPhaseWorkloadBase.TryGetValueAsync.{key}",
                    cancellationToken);
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
                return await ExecuteWithRetriesInternalAsync(asyncFunc, traceId, funcTag, retryCount);
            }
            finally
            {
                ActorTrace.Source.WriteInfoWithId(
                   TraceType,
                   traceId,
                   $"Migration func - {funcTag} completed");
            }
        }

        public static T ExecuteWithRetries<T>(Func<T> func, string traceId, string funcTag, int retryCount = 0, IEnumerable<Type> retryableExceptions = null)
        {
            try
            {
                ActorTrace.Source.WriteInfoWithId(
                    TraceType,
                    traceId,
                    $"Invoking migration func - {funcTag}");
                return ExecuteWithRetriesInternal(func, traceId, funcTag, retryCount);
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

        public static bool IgnoreKey(string key)
        {
            return key == MigrationConstants.RejectWritesKey
                || key == MigrationConstants.LogicalTimestampKey;
        }

        private static async Task<T> ExecuteWithRetriesInternalAsync<T>(Func<Task<T>> func, string traceId, string funcTag, int retriesLeft = 0, IEnumerable<Type> retryableExceptions = null)
        {
            Exception exToThrow = null;
            try
            {
                return await func.Invoke();
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Migration func - {funcTag} failed with exception - {ex}, retries left - {retriesLeft}");

                exToThrow = ex;
            }

            if (exToThrow != null)
            {
                var exMatch = retryableExceptions != null ? retryableExceptions.FirstOrDefault(type => type.IsAssignableFrom(exToThrow.GetType())) : default(Type);
                if (exMatch == default(Type) || retriesLeft <= 0)
                {
                    throw exToThrow;
                }
            }

            await Task.Delay(MigrationConstants.ConstantBackoffInterval);

            return await ExecuteWithRetriesInternalAsync(func, traceId, funcTag, retriesLeft - 1);
        }

        private static T ExecuteWithRetriesInternal<T>(Func<T> func, string traceId, string funcTag, int retriesLeft = 0, IEnumerable<Type> retryableExceptions = null)
        {
            Exception exToThrow = null;
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Migration func - {funcTag} failed with exception - {ex}, retries left - {retriesLeft}");

                exToThrow = ex;
            }

            if (exToThrow != null)
            {
                var exMatch = retryableExceptions != null ? retryableExceptions.FirstOrDefault(type => type.IsAssignableFrom(exToThrow.GetType())) : default(Type);
                if (exMatch == default(Type) || retriesLeft <= 0)
                {
                    throw exToThrow;
                }
            }

            return ExecuteWithRetriesInternal(func, traceId, funcTag, retriesLeft - 1);
        }

        private static void TraceAndThrowException<TData>(TData data, string traceId)
        {
            ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    traceId,
                    $"Failed to parse {data}");

            throw new Exception($"Failed to parse {data}"); // TODO: SFException.
        }

        internal static class RC
        {
            private static readonly string TraceType = "MigrationUtility.RC";

            internal static byte[] SerializeReminderCompletedData(string key, ReminderCompletedData data, string traceId)
            {
                try
                {
                    var res = ReminderCompletedDataSerializer.Serialize(data);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully serialized Reminder Completed Data - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to serialize Reminder Completed Data - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static ReminderCompletedData DeserializeReminderCompletedData(string key, byte[] data, string traceId)
            {
                try
                {
                    var res = ReminderCompletedDataSerializer.Deserialize(data);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully deserialized Reminder Completed Data - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder Completed Data - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static byte[] SerializeReminder(string key, ActorReminderData data, string traceId)
            {
                try
                {
                    var res = ActorReminderDataSerializer.Serialize(data);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully serialized Reminder - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to serialize Reminder - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static ActorReminderData DeserializeReminder(string key, byte[] data, string traceId)
            {
                try
                {
                    var res = ActorReminderDataSerializer.Deserialize(data);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully deserialized Reminder - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder - Key : {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }
        }

        internal static class KVS
        {
            private static readonly string TraceType = "MigrationUtility.KVS";

            private static DataContractSerializer reminderSerializer = new DataContractSerializer(typeof(ActorReminderData));
            private static DataContractSerializer reminderCompletedDataSerializer = new DataContractSerializer(typeof(ReminderCompletedData));

            internal static byte[] SerializeReminder(string key, ActorReminderData reminder, string traceId)
            {
                try
                {
                    var res = Serialize(reminderSerializer, reminder);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully serialized Reminder - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder - Key : {key}, ActorId : {reminder.ActorId}, DueTime : {reminder.DueTime}, IsReadOnly : {reminder.IsReadOnly}, LogicalCreationTime : {reminder.LogicalCreationTime}, Name : {reminder.Name}, Period : {reminder.Period}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static byte[] SerializeReminderCompletedData(string key, ReminderCompletedData reminderCompletedData, string traceId)
            {
                try
                {
                    var res = Serialize(reminderCompletedDataSerializer, reminderCompletedData);
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully serialized Reminder Completed data - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder Completed data - Key : {key}, {reminderCompletedData}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static ActorReminderData DeserializeReminder(string key, byte[] reminder, string traceId)
            {
                try
                {
                    var res = Deserialize(reminderSerializer, reminder) as ActorReminderData;
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully deserialized Reminder - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            internal static ReminderCompletedData DeserializeReminderCompletedData(string key, byte[] reminder, string traceId)
            {
                try
                {
                    var res = Deserialize(reminderCompletedDataSerializer, reminder) as ReminderCompletedData;
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        traceId,
                        $"Successfully deserialized Reminder Completed data - Key : {key}");

                    return res;
                }
                catch (Exception ex)
                {
                    ActorTrace.Source.WriteErrorWithId(
                        TraceType,
                        traceId,
                        $"Failed to deserialize Reminder Completed Data - {key}, ErrorMessage : {ex.Message}");

                    throw ex;
                }
            }

            private static byte[] Serialize<T>(DataContractSerializer serializer, T data)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                    serializer.WriteObject(binaryWriter, data);
                    binaryWriter.Flush();

                    return memoryStream.ToArray();
                }
            }

            private static object Deserialize(DataContractSerializer serializer, byte[] data)
            {
                using (var memoryStream = new MemoryStream(data))
                {
                    var binaryReader = XmlDictionaryReader.CreateBinaryReader(
                        memoryStream,
                        XmlDictionaryReaderQuotas.Max);

                    return serializer.ReadObject(binaryReader);
                }
            }
        }
    }
}
