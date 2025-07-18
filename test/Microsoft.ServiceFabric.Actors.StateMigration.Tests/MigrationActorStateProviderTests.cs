// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services;
    using Xunit;

    /// <summary>
    /// Migration state provider tests.
    /// </summary>
    public class MigrationActorStateProviderTests
    {
        private DataContractSerializer reminderSerializer = new DataContractSerializer(typeof(ActorReminderData));
        private DataContractSerializer reminderCompletedDataSerializer = new DataContractSerializer(typeof(ReminderCompletedData));

        /// <summary>
        /// Save state test.
        /// </summary>
        /// <returns>Task to represent asynchronous operation.</returns>
        [Fact]
        public async Task SaveStateTypesTest()
        {
            var migrationSP = new KVStoRCMigrationActorStateProvider(new MockTypes.MockReliableCollectionsStateProvider());

            var stateSerializer = new ActorStateProviderSerializer();
            var kvsData = new List<KVSToRCMigration.Models.KeyValuePair>();

            int j = 0;
            for (int i = 0; i < 10; i++)
            {
                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"@@_String_Actor{i}",
                     Value = new byte[0],
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Actor_String_Actor{i}_State{0}",
                     Value = stateSerializer.Serialize(typeof(string), $"Value{0}"),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Actor_String_Actor{i}_State{1}",
                     Value = stateSerializer.Serialize(typeof(long), 1),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Actor_String_Actor{i}_State{2}",
                     Value = stateSerializer.Serialize(typeof(Guid), Guid.NewGuid()),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Actor_String_Actor{i}_State{3}",
                     Value = stateSerializer.Serialize(typeof(byte[]), Encoding.UTF8.GetBytes("ValueString")),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Actor_String_Actor{i}_State{4}",
                     Value = stateSerializer.Serialize(typeof(double), 12354.1234),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                     IsDeleted = false,
                     Key = $"Reminder_String_Actor{i}_Reminder{i}",
                     Value = Serialize(
                         this.reminderSerializer,
                         new ActorReminderData(
                             new ActorId($"Actor{i}"),
                             $"Reminder{i}",
                             TimeSpan.FromSeconds(10),
                             TimeSpan.FromSeconds(10),
                             Encoding.UTF8.GetBytes("ReminderState"),
                             TimeSpan.Zero)),
                     Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                    IsDeleted = false,
                    Key = $"RC@@_String_Actor{i}_Reminder{i}",
                    Value = Serialize(this.reminderCompletedDataSerializer, new ReminderCompletedData(TimeSpan.Zero, DateTime.UtcNow)),
                    Version = i + j++,
                 });

                 kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                 {
                    IsDeleted = false,
                    Key = $"Actor_String_Actor{i}_StateComplexObject",
                    Value = stateSerializer.Serialize(typeof(MigrationSettings), new MigrationSettings
                    {
                        ChunksPerEnumeration = 10,
                        CopyPhaseParallelism = 2,
                        DowntimeThreshold = 100,
                        EnableDataIntegrityChecks = true,
                        MigrationMode = Runtime.Migration.MigrationMode.Auto,
                        TargetServiceUri = new Uri("fabric:/blah/blahblah"),
                    }),
                    Version = i + j++,
                 });
            }

            var valueList = new List<byte[]>();
            foreach (var data in kvsData)
            {
                valueList.Add(data.Value);
            }

            var sourceCRC = CRC64.ToCRC64(valueList.ToArray());

            Assert.Equal(await migrationSP.SaveStateAsync(kvsData, CancellationToken.None), kvsData.Count);
            await migrationSP.ValidateDataPostMigrationAsync(kvsData, sourceCRC.ToString("X", CultureInfo.InvariantCulture), true, CancellationToken.None);
        }

        /// <summary>
        /// Ignore keys test.
        /// </summary>
        /// <returns>Task to represent asynchronous operation.</returns>
        [Fact]
        public async Task IgnoreKeysTest()
        {
            var migrationSP = new KVStoRCMigrationActorStateProvider(new MockTypes.MockReliableCollectionsStateProvider());

            var stateSerializer = new ActorStateProviderSerializer();
            var kvsData = new List<KVSToRCMigration.Models.KeyValuePair>();

            int j = 0;
            for (int i = 0; i < 1; i++)
            {
                kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                {
                    IsDeleted = false,
                    Key = MigrationConstants.RejectWritesKey,
                    Value = new byte[0],
                    Version = i + j++,
                });

                kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                {
                    IsDeleted = false,
                    Key = MigrationConstants.LogicalTimestampKey,
                    Value = stateSerializer.Serialize(typeof(TimeSpan), TimeSpan.FromSeconds(10)),
                    Version = i + j++,
                });
            }

            var valueList = new List<byte[]>();
            var sourceCRC = CRC64.ToCRC64(valueList.ToArray());

            Assert.Equal(0L, await migrationSP.SaveStateAsync(kvsData, CancellationToken.None));
            await migrationSP.ValidateDataPostMigrationAsync(kvsData, sourceCRC.ToString("X", CultureInfo.InvariantCulture), true, CancellationToken.None);
        }

        /// <summary>
        /// Deleted test.
        /// </summary>
        /// <returns>Task to represent asynchronous operation.</returns>
        [Fact]
        public async Task DeletedKeysTest()
        {
            var migrationSP = new KVStoRCMigrationActorStateProvider(new MockTypes.MockReliableCollectionsStateProvider());

            var stateSerializer = new ActorStateProviderSerializer();
            var kvsData = new List<KVSToRCMigration.Models.KeyValuePair>();

            int j = 0;
            for (int i = 0; i < 10; i++)
            {
                kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                {
                    IsDeleted = false,
                    Key = $"@@_String_Actor{i}",
                    Value = new byte[0],
                    Version = i + j++,
                });
            }

            Assert.Equal(await migrationSP.SaveStateAsync(kvsData, CancellationToken.None), kvsData.Count);

            kvsData = new List<KVSToRCMigration.Models.KeyValuePair>();
            kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
            {
                IsDeleted = true,
                Key = $"@@_String_Actor2",
                Value = new byte[0],
                Version = 1000,
            });

            kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
            {
                IsDeleted = true,
                Key = $"@@_String_Actor5",
                Value = new byte[0],
                Version = 1001,
            });

            var valueList = new List<byte[]>();
            var sourceCRC = CRC64.ToCRC64(valueList.ToArray());
            Assert.Equal(2, await migrationSP.SaveStateAsync(kvsData, CancellationToken.None));

            await migrationSP.ValidateDataPostMigrationAsync(kvsData, sourceCRC.ToString("X", CultureInfo.InvariantCulture), true, CancellationToken.None);
        }

        /// <summary>
        /// Data validation failure test.
        /// </summary>
        /// <returns>Task to represent asynchronous operation.</returns>
        [Fact]
        public async Task DataValidationFailureTest()
        {
            var migrationSP = new KVStoRCMigrationActorStateProvider(new MockTypes.MockReliableCollectionsStateProvider());

            var stateSerializer = new ActorStateProviderSerializer();
            var kvsData = new List<KVSToRCMigration.Models.KeyValuePair>();

            int j = 0;
            for (int i = 0; i < 10; i++)
            {
                kvsData.Add(new KVSToRCMigration.Models.KeyValuePair()
                {
                    IsDeleted = false,
                    Key = $"@@_String_Actor{i}",
                    Value = new byte[0],
                    Version = i + j++,
                });
            }

            await Assert.ThrowsAsync<MigrationDataValidationException>(() => migrationSP.ValidateDataPostMigrationAsync(kvsData, string.Empty, true, CancellationToken.None));
            Assert.Equal(await migrationSP.SaveStateAsync(kvsData, CancellationToken.None), kvsData.Count);
            kvsData[0].IsDeleted = true;
            await Assert.ThrowsAsync<MigrationDataValidationException>(() => migrationSP.ValidateDataPostMigrationAsync(kvsData, string.Empty, true, CancellationToken.None));
            kvsData[0].IsDeleted = false;
            await Assert.ThrowsAsync<MigrationDataValidationException>(() => migrationSP.ValidateDataPostMigrationAsync(kvsData, "BLAH", true, CancellationToken.None));
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
    }
}
