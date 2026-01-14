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
    /// Converter for <see cref="DeployedCodePackageInfo" />.
    /// </summary>
    internal class DeployedCodePackageInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedCodePackageInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedCodePackageInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var version = default(string);
            var serviceManifestName = default(string);
            var servicePackageActivationId = default(string);
            var hostType = default(HostType?);
            var hostIsolationMode = default(HostIsolationMode?);
            var status = default(DeploymentStatus?);
            var runFrequencyInterval = default(string);
            var setupEntryPoint = default(CodePackageEntryPoint);
            var mainEntryPoint = default(CodePackageEntryPoint);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageActivationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationId = reader.ReadValueAsString();
                }
                else if (string.Compare("HostType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hostType = HostTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("HostIsolationMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hostIsolationMode = HostIsolationModeConverter.Deserialize(reader);
                }
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = DeploymentStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("RunFrequencyInterval", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    runFrequencyInterval = reader.ReadValueAsString();
                }
                else if (string.Compare("SetupEntryPoint", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    setupEntryPoint = CodePackageEntryPointConverter.Deserialize(reader);
                }
                else if (string.Compare("MainEntryPoint", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mainEntryPoint = CodePackageEntryPointConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedCodePackageInfo(
                name: name,
                version: version,
                serviceManifestName: serviceManifestName,
                servicePackageActivationId: servicePackageActivationId,
                hostType: hostType,
                hostIsolationMode: hostIsolationMode,
                status: status,
                runFrequencyInterval: runFrequencyInterval,
                setupEntryPoint: setupEntryPoint,
                mainEntryPoint: mainEntryPoint);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedCodePackageInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HostType, "HostType", HostTypeConverter.Serialize);
            writer.WriteProperty(obj.HostIsolationMode, "HostIsolationMode", HostIsolationModeConverter.Serialize);
            writer.WriteProperty(obj.Status, "Status", DeploymentStatusConverter.Serialize);
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Version != null)
            {
                writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceManifestName != null)
            {
                writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServicePackageActivationId != null)
            {
                writer.WriteProperty(obj.ServicePackageActivationId, "ServicePackageActivationId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.RunFrequencyInterval != null)
            {
                writer.WriteProperty(obj.RunFrequencyInterval, "RunFrequencyInterval", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.SetupEntryPoint != null)
            {
                writer.WriteProperty(obj.SetupEntryPoint, "SetupEntryPoint", CodePackageEntryPointConverter.Serialize);
            }

            if (obj.MainEntryPoint != null)
            {
                writer.WriteProperty(obj.MainEntryPoint, "MainEntryPoint", CodePackageEntryPointConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
