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
    /// Converter for <see cref="CodePackageEntryPoint" />.
    /// </summary>
    internal class CodePackageEntryPointConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CodePackageEntryPoint Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CodePackageEntryPoint GetFromJsonProperties(JsonReader reader)
        {
            var entryPointLocation = default(string);
            var processId = default(string);
            var runAsUserName = default(string);
            var codePackageEntryPointStatistics = default(CodePackageEntryPointStatistics);
            var status = default(EntryPointStatus?);
            var nextActivationTime = default(DateTime?);
            var instanceId = default(string);
            var containerId = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EntryPointLocation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    entryPointLocation = reader.ReadValueAsString();
                }
                else if (string.Compare("ProcessId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    processId = reader.ReadValueAsString();
                }
                else if (string.Compare("RunAsUserName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    runAsUserName = reader.ReadValueAsString();
                }
                else if (string.Compare("CodePackageEntryPointStatistics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageEntryPointStatistics = CodePackageEntryPointStatisticsConverter.Deserialize(reader);
                }
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = EntryPointStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("NextActivationTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nextActivationTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("InstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceId = reader.ReadValueAsString();
                }
                else if (string.Compare("ContainerId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    containerId = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CodePackageEntryPoint(
                entryPointLocation: entryPointLocation,
                processId: processId,
                runAsUserName: runAsUserName,
                codePackageEntryPointStatistics: codePackageEntryPointStatistics,
                status: status,
                nextActivationTime: nextActivationTime,
                instanceId: instanceId,
                containerId: containerId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CodePackageEntryPoint obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", EntryPointStatusConverter.Serialize);
            if (obj.EntryPointLocation != null)
            {
                writer.WriteProperty(obj.EntryPointLocation, "EntryPointLocation", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ProcessId != null)
            {
                writer.WriteProperty(obj.ProcessId, "ProcessId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.RunAsUserName != null)
            {
                writer.WriteProperty(obj.RunAsUserName, "RunAsUserName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CodePackageEntryPointStatistics != null)
            {
                writer.WriteProperty(obj.CodePackageEntryPointStatistics, "CodePackageEntryPointStatistics", CodePackageEntryPointStatisticsConverter.Serialize);
            }

            if (obj.NextActivationTime != null)
            {
                writer.WriteProperty(obj.NextActivationTime, "NextActivationTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContainerId != null)
            {
                writer.WriteProperty(obj.ContainerId, "ContainerId", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
