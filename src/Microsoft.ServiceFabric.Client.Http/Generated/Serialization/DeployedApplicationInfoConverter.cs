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
    /// Converter for <see cref="DeployedApplicationInfo" />.
    /// </summary>
    internal class DeployedApplicationInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedApplicationInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedApplicationInfo GetFromJsonProperties(JsonReader reader)
        {
            var id = default(string);
            var name = default(ApplicationName);
            var typeName = default(string);
            var status = default(DeployedApplicationStatus?);
            var workDirectory = default(string);
            var logDirectory = default(string);
            var tempDirectory = default(string);
            var healthState = default(HealthState?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Id", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    id = reader.ReadValueAsString();
                }
                else if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = ApplicationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("TypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    typeName = reader.ReadValueAsString();
                }
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = DeployedApplicationStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("WorkDirectory", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    workDirectory = reader.ReadValueAsString();
                }
                else if (string.Compare("LogDirectory", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    logDirectory = reader.ReadValueAsString();
                }
                else if (string.Compare("TempDirectory", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tempDirectory = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedApplicationInfo(
                id: id,
                name: name,
                typeName: typeName,
                status: status,
                workDirectory: workDirectory,
                logDirectory: logDirectory,
                tempDirectory: tempDirectory,
                healthState: healthState);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedApplicationInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", DeployedApplicationStatusConverter.Serialize);
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.Id != null)
            {
                writer.WriteProperty(obj.Id, "Id", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", ApplicationNameConverter.Serialize);
            }

            if (obj.TypeName != null)
            {
                writer.WriteProperty(obj.TypeName, "TypeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.WorkDirectory != null)
            {
                writer.WriteProperty(obj.WorkDirectory, "WorkDirectory", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LogDirectory != null)
            {
                writer.WriteProperty(obj.LogDirectory, "LogDirectory", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TempDirectory != null)
            {
                writer.WriteProperty(obj.TempDirectory, "TempDirectory", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
