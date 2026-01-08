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
    /// Converter for <see cref="ApplicationLoadInfo" />.
    /// </summary>
    internal class ApplicationLoadInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationLoadInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationLoadInfo GetFromJsonProperties(JsonReader reader)
        {
            var id = default(string);
            var minimumNodes = default(long?);
            var maximumNodes = default(long?);
            var nodeCount = default(long?);
            var applicationLoadMetricInformation = default(IEnumerable<ApplicationLoadMetricInformation>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Id", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    id = reader.ReadValueAsString();
                }
                else if (string.Compare("MinimumNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minimumNodes = reader.ReadValueAsLong();
                }
                else if (string.Compare("MaximumNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maximumNodes = reader.ReadValueAsLong();
                }
                else if (string.Compare("NodeCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeCount = reader.ReadValueAsLong();
                }
                else if (string.Compare("ApplicationLoadMetricInformation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationLoadMetricInformation = reader.ReadList(ApplicationLoadMetricInformationConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationLoadInfo(
                id: id,
                minimumNodes: minimumNodes,
                maximumNodes: maximumNodes,
                nodeCount: nodeCount,
                applicationLoadMetricInformation: applicationLoadMetricInformation);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationLoadInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Id != null)
            {
                writer.WriteProperty(obj.Id, "Id", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MinimumNodes != null)
            {
                writer.WriteProperty(obj.MinimumNodes, "MinimumNodes", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.MaximumNodes != null)
            {
                writer.WriteProperty(obj.MaximumNodes, "MaximumNodes", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.NodeCount != null)
            {
                writer.WriteProperty(obj.NodeCount, "NodeCount", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ApplicationLoadMetricInformation != null)
            {
                writer.WriteEnumerableProperty(obj.ApplicationLoadMetricInformation, "ApplicationLoadMetricInformation", ApplicationLoadMetricInformationConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
