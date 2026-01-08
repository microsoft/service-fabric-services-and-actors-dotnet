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
    /// Converter for <see cref="ServiceTypeHealthPolicy" />.
    /// </summary>
    internal class ServiceTypeHealthPolicyConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTypeHealthPolicy Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTypeHealthPolicy GetFromJsonProperties(JsonReader reader)
        {
            var maxPercentUnhealthyPartitionsPerService = default(int?);
            var maxPercentUnhealthyReplicasPerPartition = default(int?);
            var maxPercentUnhealthyServices = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MaxPercentUnhealthyPartitionsPerService", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyPartitionsPerService = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentUnhealthyReplicasPerPartition", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyReplicasPerPartition = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentUnhealthyServices", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyServices = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceTypeHealthPolicy(
                maxPercentUnhealthyPartitionsPerService: maxPercentUnhealthyPartitionsPerService,
                maxPercentUnhealthyReplicasPerPartition: maxPercentUnhealthyReplicasPerPartition,
                maxPercentUnhealthyServices: maxPercentUnhealthyServices);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceTypeHealthPolicy obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.MaxPercentUnhealthyPartitionsPerService != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyPartitionsPerService, "MaxPercentUnhealthyPartitionsPerService", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentUnhealthyReplicasPerPartition != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyReplicasPerPartition, "MaxPercentUnhealthyReplicasPerPartition", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentUnhealthyServices != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyServices, "MaxPercentUnhealthyServices", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
