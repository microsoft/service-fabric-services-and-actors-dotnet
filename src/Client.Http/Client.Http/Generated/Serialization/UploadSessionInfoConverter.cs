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
    /// Converter for <see cref="UploadSessionInfo" />.
    /// </summary>
    internal class UploadSessionInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UploadSessionInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UploadSessionInfo GetFromJsonProperties(JsonReader reader)
        {
            var storeRelativePath = default(string);
            var sessionId = default(Guid?);
            var modifiedDate = default(DateTime?);
            var fileSize = default(string);
            var expectedRanges = default(IEnumerable<UploadChunkRange>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StoreRelativePath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeRelativePath = reader.ReadValueAsString();
                }
                else if (string.Compare("SessionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sessionId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("ModifiedDate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    modifiedDate = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("FileSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fileSize = reader.ReadValueAsString();
                }
                else if (string.Compare("ExpectedRanges", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    expectedRanges = reader.ReadList(UploadChunkRangeConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UploadSessionInfo(
                storeRelativePath: storeRelativePath,
                sessionId: sessionId,
                modifiedDate: modifiedDate,
                fileSize: fileSize,
                expectedRanges: expectedRanges);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UploadSessionInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.StoreRelativePath != null)
            {
                writer.WriteProperty(obj.StoreRelativePath, "StoreRelativePath", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.SessionId != null)
            {
                writer.WriteProperty(obj.SessionId, "SessionId", JsonWriterExtensions.WriteGuidValue);
            }

            if (obj.ModifiedDate != null)
            {
                writer.WriteProperty(obj.ModifiedDate, "ModifiedDate", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.FileSize != null)
            {
                writer.WriteProperty(obj.FileSize, "FileSize", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ExpectedRanges != null)
            {
                writer.WriteEnumerableProperty(obj.ExpectedRanges, "ExpectedRanges", UploadChunkRangeConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
