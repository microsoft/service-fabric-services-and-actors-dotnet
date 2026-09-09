// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converter for <see cref="RemoteInbuildReplicaStatus" />.
    /// </summary>
    internal class RemoteInbuildReplicaStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteInbuildReplicaStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteInbuildReplicaStatus GetFromJsonProperties(JsonReader reader)
        {
            var inbuildPhase = default(InbuildReplicaPhase?);
            var copyContextPhase = default(InbuildReplicaCopyContextPhase?);
            var lastCopySequenceNumber = default(string);
            var lastCopyCatchupSequenceNumber = default(string);
            var copyContextPhaseStartTimeUtc = default(DateTime?);
            var copyStatePhaseStartTimeUtc = default(DateTime?);
            var copyPhaseStartTimeUtc = default(DateTime?);
            var copyCatchupPhaseStartTimeUtc = default(DateTime?);
            var copyDetails = default(InbuildReplicaCopyDetail);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("InbuildPhase", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    inbuildPhase = InbuildReplicaPhaseConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyContextPhase", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyContextPhase = InbuildReplicaCopyContextPhaseConverter.Deserialize(reader);
                }
                else if (string.Compare("LastCopySequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastCopySequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("LastCopyCatchupSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastCopyCatchupSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("CopyContextPhaseStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyContextPhaseStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("CopyStatePhaseStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyStatePhaseStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("CopyPhaseStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyPhaseStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("CopyCatchupPhaseStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyCatchupPhaseStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("CopyDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyDetails = InbuildReplicaCopyDetailConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RemoteInbuildReplicaStatus(
                inbuildPhase: inbuildPhase,
                copyContextPhase: copyContextPhase,
                lastCopySequenceNumber: lastCopySequenceNumber,
                lastCopyCatchupSequenceNumber: lastCopyCatchupSequenceNumber,
                copyContextPhaseStartTimeUtc: copyContextPhaseStartTimeUtc,
                copyStatePhaseStartTimeUtc: copyStatePhaseStartTimeUtc,
                copyPhaseStartTimeUtc: copyPhaseStartTimeUtc,
                copyCatchupPhaseStartTimeUtc: copyCatchupPhaseStartTimeUtc,
                copyDetails: copyDetails);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RemoteInbuildReplicaStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.InbuildPhase, "InbuildPhase", InbuildReplicaPhaseConverter.Serialize);
            writer.WriteProperty(obj.CopyContextPhase, "CopyContextPhase", InbuildReplicaCopyContextPhaseConverter.Serialize);
            if (obj.LastCopySequenceNumber != null)
            {
                writer.WriteProperty(obj.LastCopySequenceNumber, "LastCopySequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastCopyCatchupSequenceNumber != null)
            {
                writer.WriteProperty(obj.LastCopyCatchupSequenceNumber, "LastCopyCatchupSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CopyContextPhaseStartTimeUtc != null)
            {
                writer.WriteProperty(obj.CopyContextPhaseStartTimeUtc, "CopyContextPhaseStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.CopyStatePhaseStartTimeUtc != null)
            {
                writer.WriteProperty(obj.CopyStatePhaseStartTimeUtc, "CopyStatePhaseStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.CopyPhaseStartTimeUtc != null)
            {
                writer.WriteProperty(obj.CopyPhaseStartTimeUtc, "CopyPhaseStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.CopyCatchupPhaseStartTimeUtc != null)
            {
                writer.WriteProperty(obj.CopyCatchupPhaseStartTimeUtc, "CopyCatchupPhaseStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.CopyDetails != null)
            {
                writer.WriteProperty(obj.CopyDetails, "CopyDetails", InbuildReplicaCopyDetailConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
