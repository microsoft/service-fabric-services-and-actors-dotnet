// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Microsoft.ServiceFabric.Actors.Migration;

    [DataContract]
    internal class PhaseInput
    {
        private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PhaseInput), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        });

        [DataMember]
        public DateTime StartDateTimeUTC { get; set; }

        [DataMember]
        public DateTime? EndDateTimeUTC { get; set; }

        [DataMember]
        public long StartSeqNum { get; set; }

        [DataMember]
        public long EndSeqNum { get; set; }

        [DataMember]
        public long? LastAppliedSeqNum { get; set; }

        [DataMember]
        public MigrationState Status { get; set; }

        [DataMember]
        public int WorkerCount { get; set; }

        [DataMember]
        public int IterationCount { get; set; }

        [DataMember]
        public MigrationPhase Phase { get; set; }

        [DataMember]
        public WorkerInput[] WorkerInputs { get; set; }

        public override string ToString()
        {
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);

                var returnVal = Encoding.ASCII.GetString(stream.GetBuffer());

                return returnVal;
            }
        }

        [DataContract]
        public class WorkerInput
        {
            private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WorkerInput), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
            });

            [DataMember]
            public int WorkerId { get; set; }

            [DataMember]
            public int Iteration { get; set; }

            [DataMember]
            public DateTime StartDateTimeUTC { get; set; }

            [DataMember]
            public DateTime? EndDateTimeUTC { get; set; }

            [DataMember]
            public long StartSeqNum { get; set; }

            [DataMember]
            public long EndSeqNum { get; set; }

            [DataMember]
            public long? LastAppliedSeqNum { get; set; }

            [DataMember]
            public MigrationPhase Phase { get; set; }

            [DataMember]
            public MigrationState Status { get; set; }

            public override string ToString()
            {
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, this);

                    var returnVal = Encoding.ASCII.GetString(stream.GetBuffer());

                    return returnVal;
                }
            }
        }
    }
}
