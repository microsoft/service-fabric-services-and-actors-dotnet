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
    /// Converter for <see cref="ApplicationLoadMetricInformation" />.
    /// </summary>
    internal class ApplicationLoadMetricInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationLoadMetricInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationLoadMetricInformation GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var reservationCapacity = default(long?);
            var applicationCapacity = default(long?);
            var applicationLoad = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("ReservationCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reservationCapacity = reader.ReadValueAsLong();
                }
                else if (string.Compare("ApplicationCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationCapacity = reader.ReadValueAsLong();
                }
                else if (string.Compare("ApplicationLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationLoad = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationLoadMetricInformation(
                name: name,
                reservationCapacity: reservationCapacity,
                applicationCapacity: applicationCapacity,
                applicationLoad: applicationLoad);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationLoadMetricInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ReservationCapacity != null)
            {
                writer.WriteProperty(obj.ReservationCapacity, "ReservationCapacity", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ApplicationCapacity != null)
            {
                writer.WriteProperty(obj.ApplicationCapacity, "ApplicationCapacity", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ApplicationLoad != null)
            {
                writer.WriteProperty(obj.ApplicationLoad, "ApplicationLoad", JsonWriterExtensions.WriteLongValue);
            }

            writer.WriteEndObject();
        }
    }
}
