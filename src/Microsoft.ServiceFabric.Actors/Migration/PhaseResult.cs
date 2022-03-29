// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// Migration phase result.
    /// </summary>
    [DataContract]
    public class PhaseResult
    {
        private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PhaseResult), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        });

        /// <summary>
        /// Gets or sets the migration start time.
        /// </summary>
        [DataMember]
        public DateTime StartDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the migration end time.
        /// </summary>
        [DataMember]
        public DateTime? EndDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the start sequence number.
        /// </summary>
        [DataMember]
        public long StartSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the end sequence number.
        /// </summary>
        [DataMember]
        public long EndSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the last applied sequence number.
        /// </summary>
        [DataMember]
        public long? LastAppliedSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the migration status.
        /// </summary>
        [DataMember]
        public MigrationState Status { get; set; }

        /// <summary>
        /// Gets or sets the worker count.
        /// </summary>
        [DataMember]
        public int WorkerCount { get; set; }

        /// <summary>
        /// Gets or sets the current iteration.
        /// </summary>
        [DataMember]
        public int Iteration { get; set; }

        /// <summary>
        /// Gets or sets the number of keys migrated.
        /// </summary>
        [DataMember]
        public long? NoOfKeysMigrated { get; set; }

        /// <summary>
        /// Gets or sets the migration phase.
        /// </summary>
        [DataMember]
        public MigrationPhase Phase { get; set; }

        /// <summary>
        /// Gets or sets the worker results.
        /// </summary>
        [DataMember]
        public WorkerResult[] WorkerResults { get; set; }

        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>Returns the string representation of the object.</returns>
        public override string ToString()
        {
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);

                var returnVal = Encoding.ASCII.GetString(stream.GetBuffer());

                return returnVal;
            }
        }

        /// <summary>
        /// Migration worker result.
        /// </summary>
        [DataContract]
        public class WorkerResult
        {
            private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WorkerResult), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
            });

            /// <summary>
            /// Gets or sets the worker id.
            /// </summary>
            [DataMember]
            public int WorkerId { get; set; }

            /// <summary>
            /// Gets or sets the migration start time.
            /// </summary>
            [DataMember]
            public DateTime StartDateTimeUTC { get; set; }

            /// <summary>
            /// Gets or sets the migration end time.
            /// </summary>
            [DataMember]
            public DateTime? EndDateTimeUTC { get; set; }

            /// <summary>
            /// Gets or sets the start sequence number.
            /// </summary>
            [DataMember]
            public long StartSeqNum { get; set; }

            /// <summary>
            /// Gets or sets the end sequence number.
            /// </summary>
            [DataMember]
            public long EndSeqNum { get; set; }

            /// <summary>
            /// Gets or sets the last applied sequence number.
            /// </summary>
            [DataMember]
            public long? LastAppliedSeqNum { get; set; }

            /// <summary>
            /// Gets or sets the migration status.
            /// </summary>
            [DataMember]
            public MigrationState Status { get; set; }

            /// <summary>
            /// Gets or sets the worker count.
            /// </summary>
            [DataMember]
            public int WorkerCount { get; set; }

            /// <summary>
            /// Gets or sets the current iteration.
            /// </summary>
            [DataMember]
            public int Iteration { get; set; }

            /// <summary>
            /// Gets or sets the number of keys migrated.
            /// </summary>
            [DataMember]
            public long? NoOfKeysMigrated { get; set; }

            /// <summary>
            /// Gets or sets the migration phase.
            /// </summary>
            [DataMember]
            public MigrationPhase Phase { get; set; }

            /// <summary>
            /// String representation of the object.
            /// </summary>
            /// <returns>Returns the string representation of the object.</returns>
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
