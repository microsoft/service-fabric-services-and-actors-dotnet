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
    /// Converter for <see cref="ApplicationUpgradeUpdateDescription" />.
    /// </summary>
    internal class ApplicationUpgradeUpdateDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationUpgradeUpdateDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationUpgradeUpdateDescription GetFromJsonProperties(JsonReader reader)
        {
            var name = default(ApplicationName);
            var upgradeKind = default(UpgradeKind?);
            var applicationHealthPolicy = default(ApplicationHealthPolicy);
            var updateDescription = default(RollingUpgradeUpdateDescription);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = ApplicationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeKind = UpgradeKindConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicy = ApplicationHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("UpdateDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    updateDescription = RollingUpgradeUpdateDescriptionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationUpgradeUpdateDescription(
                name: name,
                upgradeKind: upgradeKind,
                applicationHealthPolicy: applicationHealthPolicy,
                updateDescription: updateDescription);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationUpgradeUpdateDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeKindConverter.Serialize);
            writer.WriteProperty(obj.Name, "Name", ApplicationNameConverter.Serialize);
            if (obj.ApplicationHealthPolicy != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicy, "ApplicationHealthPolicy", ApplicationHealthPolicyConverter.Serialize);
            }

            if (obj.UpdateDescription != null)
            {
                writer.WriteProperty(obj.UpdateDescription, "UpdateDescription", RollingUpgradeUpdateDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
