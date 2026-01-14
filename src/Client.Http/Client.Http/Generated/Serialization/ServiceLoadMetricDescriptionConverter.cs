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
    /// Converter for <see cref="ServiceLoadMetricDescription" />.
    /// </summary>
    internal class ServiceLoadMetricDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceLoadMetricDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceLoadMetricDescription GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var weight = default(ServiceLoadMetricWeight?);
            var primaryDefaultLoad = default(int?);
            var secondaryDefaultLoad = default(int?);
            var auxiliaryDefaultLoad = default(int?);
            var maximumLoad = default(int?);
            var defaultLoad = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Weight", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    weight = ServiceLoadMetricWeightConverter.Deserialize(reader);
                }
                else if (string.Compare("PrimaryDefaultLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryDefaultLoad = reader.ReadValueAsInt();
                }
                else if (string.Compare("SecondaryDefaultLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryDefaultLoad = reader.ReadValueAsInt();
                }
                else if (string.Compare("AuxiliaryDefaultLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryDefaultLoad = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaximumLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maximumLoad = reader.ReadValueAsInt();
                }
                else if (string.Compare("DefaultLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultLoad = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceLoadMetricDescription(
                name: name,
                weight: weight,
                primaryDefaultLoad: primaryDefaultLoad,
                secondaryDefaultLoad: secondaryDefaultLoad,
                auxiliaryDefaultLoad: auxiliaryDefaultLoad,
                maximumLoad: maximumLoad,
                defaultLoad: defaultLoad);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceLoadMetricDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Weight, "Weight", ServiceLoadMetricWeightConverter.Serialize);
            if (obj.PrimaryDefaultLoad != null)
            {
                writer.WriteProperty(obj.PrimaryDefaultLoad, "PrimaryDefaultLoad", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.SecondaryDefaultLoad != null)
            {
                writer.WriteProperty(obj.SecondaryDefaultLoad, "SecondaryDefaultLoad", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.AuxiliaryDefaultLoad != null)
            {
                writer.WriteProperty(obj.AuxiliaryDefaultLoad, "AuxiliaryDefaultLoad", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaximumLoad != null)
            {
                writer.WriteProperty(obj.MaximumLoad, "MaximumLoad", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.DefaultLoad != null)
            {
                writer.WriteProperty(obj.DefaultLoad, "DefaultLoad", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
