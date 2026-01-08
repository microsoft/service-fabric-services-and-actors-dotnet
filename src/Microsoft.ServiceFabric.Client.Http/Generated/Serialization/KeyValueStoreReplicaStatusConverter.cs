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
    /// Converter for <see cref="KeyValueStoreReplicaStatus" />.
    /// </summary>
    internal class KeyValueStoreReplicaStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreReplicaStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreReplicaStatus GetFromJsonProperties(JsonReader reader)
        {
            var databaseRowCountEstimate = default(string);
            var databaseLogicalSizeEstimate = default(string);
            var copyNotificationCurrentKeyFilter = default(string);
            var copyNotificationCurrentProgress = default(string);
            var statusDetails = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DatabaseRowCountEstimate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    databaseRowCountEstimate = reader.ReadValueAsString();
                }
                else if (string.Compare("DatabaseLogicalSizeEstimate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    databaseLogicalSizeEstimate = reader.ReadValueAsString();
                }
                else if (string.Compare("CopyNotificationCurrentKeyFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyNotificationCurrentKeyFilter = reader.ReadValueAsString();
                }
                else if (string.Compare("CopyNotificationCurrentProgress", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyNotificationCurrentProgress = reader.ReadValueAsString();
                }
                else if (string.Compare("StatusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    statusDetails = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new KeyValueStoreReplicaStatus(
                databaseRowCountEstimate: databaseRowCountEstimate,
                databaseLogicalSizeEstimate: databaseLogicalSizeEstimate,
                copyNotificationCurrentKeyFilter: copyNotificationCurrentKeyFilter,
                copyNotificationCurrentProgress: copyNotificationCurrentProgress,
                statusDetails: statusDetails);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, KeyValueStoreReplicaStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ReplicaKindConverter.Serialize);
            if (obj.DatabaseRowCountEstimate != null)
            {
                writer.WriteProperty(obj.DatabaseRowCountEstimate, "DatabaseRowCountEstimate", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.DatabaseLogicalSizeEstimate != null)
            {
                writer.WriteProperty(obj.DatabaseLogicalSizeEstimate, "DatabaseLogicalSizeEstimate", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CopyNotificationCurrentKeyFilter != null)
            {
                writer.WriteProperty(obj.CopyNotificationCurrentKeyFilter, "CopyNotificationCurrentKeyFilter", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CopyNotificationCurrentProgress != null)
            {
                writer.WriteProperty(obj.CopyNotificationCurrentProgress, "CopyNotificationCurrentProgress", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.StatusDetails != null)
            {
                writer.WriteProperty(obj.StatusDetails, "StatusDetails", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
