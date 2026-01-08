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
    /// Converter for <see cref="DeployServicePackageToNodeDescription" />.
    /// </summary>
    internal class DeployServicePackageToNodeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployServicePackageToNodeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployServicePackageToNodeDescription GetFromJsonProperties(JsonReader reader)
        {
            var serviceManifestName = default(string);
            var applicationTypeName = default(string);
            var applicationTypeVersion = default(string);
            var nodeName = default(NodeName);
            var packageSharingPolicy = default(IEnumerable<PackageSharingPolicyInfo>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("PackageSharingPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    packageSharingPolicy = reader.ReadList(PackageSharingPolicyInfoConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployServicePackageToNodeDescription(
                serviceManifestName: serviceManifestName,
                applicationTypeName: applicationTypeName,
                applicationTypeVersion: applicationTypeVersion,
                nodeName: nodeName,
                packageSharingPolicy: packageSharingPolicy);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployServicePackageToNodeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationTypeName, "ApplicationTypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationTypeVersion, "ApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.NodeName, "NodeName", NodeNameConverter.Serialize);
            if (obj.PackageSharingPolicy != null)
            {
                writer.WriteEnumerableProperty(obj.PackageSharingPolicy, "PackageSharingPolicy", PackageSharingPolicyInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
