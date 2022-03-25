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
    /// Migration result.
    /// </summary>
    [DataContract]
    public class MigrationResult
    {
        private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MigrationResult), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
        });

        /// <summary>
        /// Gets or sets the migration start time.
        /// </summary>
        [DataMember]
        public DateTime? StartDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the migration end time.
        /// </summary>
        [DataMember]
        public DateTime? EndDateTimeUTC { get; set; }

        /// <summary>
        /// Gets or sets the migration status.
        /// </summary>
        [DataMember]
        public MigrationState Status { get; set; }

        /// <summary>
        /// Gets or sets the current migration phase.
        /// </summary>
        [DataMember]
        public MigrationPhase CurrentPhase { get; set; }

        /// <summary>
        /// Gets or sets the start sequence num.
        /// </summary>
        [DataMember]
        public long? StartSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the end sequence nummber.
        /// </summary>
        [DataMember]
        public long? EndSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the last applied sequence number.
        /// </summary>
        [DataMember]
        public long? LastAppliedSeqNum { get; set; }

        /// <summary>
        /// Gets or sets the number of sequence numbers migrated.
        /// </summary>
        [DataMember]
        public long? NoOfKeysMigrated { get; set; }

        /// <summary>
        /// Gets or sets the phase wise results.
        /// </summary>
        [DataMember]
        public PhaseResult[] PhaseResults { get; set; }

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
