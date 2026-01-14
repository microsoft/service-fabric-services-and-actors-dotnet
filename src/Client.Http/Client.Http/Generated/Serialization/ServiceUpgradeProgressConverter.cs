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
    /// Converter for <see cref="ServiceUpgradeProgress" />.
    /// </summary>
    internal class ServiceUpgradeProgressConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceUpgradeProgress Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceUpgradeProgress GetFromJsonProperties(JsonReader reader)
        {
            var serviceName = default(string);
            var completedReplicaCount = default(string);
            var pendingReplicaCount = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("CompletedReplicaCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    completedReplicaCount = reader.ReadValueAsString();
                }
                else if (string.Compare("PendingReplicaCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pendingReplicaCount = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceUpgradeProgress(
                serviceName: serviceName,
                completedReplicaCount: completedReplicaCount,
                pendingReplicaCount: pendingReplicaCount);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceUpgradeProgress obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CompletedReplicaCount != null)
            {
                writer.WriteProperty(obj.CompletedReplicaCount, "CompletedReplicaCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PendingReplicaCount != null)
            {
                writer.WriteProperty(obj.PendingReplicaCount, "PendingReplicaCount", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
