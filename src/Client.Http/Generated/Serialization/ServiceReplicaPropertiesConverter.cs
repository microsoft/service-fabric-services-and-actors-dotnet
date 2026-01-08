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
    /// Converter for <see cref="ServiceReplicaProperties" />.
    /// </summary>
    internal class ServiceReplicaPropertiesConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceReplicaProperties Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceReplicaProperties GetFromJsonProperties(JsonReader reader)
        {
            var osType = default(OperatingSystemType?);
            var codePackages = default(IEnumerable<ContainerCodePackageProperties>);
            var networkRefs = default(IEnumerable<NetworkRef>);
            var diagnostics = default(DiagnosticsRef);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("osType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    osType = OperatingSystemTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("codePackages", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackages = reader.ReadList(ContainerCodePackagePropertiesConverter.Deserialize);
                }
                else if (string.Compare("networkRefs", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    networkRefs = reader.ReadList(NetworkRefConverter.Deserialize);
                }
                else if (string.Compare("diagnostics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    diagnostics = DiagnosticsRefConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceReplicaProperties(
                osType: osType,
                codePackages: codePackages,
                networkRefs: networkRefs,
                diagnostics: diagnostics);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceReplicaProperties obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.OsType, "osType", OperatingSystemTypeConverter.Serialize);
            writer.WriteEnumerableProperty(obj.CodePackages, "codePackages", ContainerCodePackagePropertiesConverter.Serialize);
            if (obj.NetworkRefs != null)
            {
                writer.WriteEnumerableProperty(obj.NetworkRefs, "networkRefs", NetworkRefConverter.Serialize);
            }

            if (obj.Diagnostics != null)
            {
                writer.WriteProperty(obj.Diagnostics, "diagnostics", DiagnosticsRefConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
