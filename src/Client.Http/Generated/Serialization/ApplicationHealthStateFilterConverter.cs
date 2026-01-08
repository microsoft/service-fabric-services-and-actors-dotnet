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
    /// Converter for <see cref="ApplicationHealthStateFilter" />.
    /// </summary>
    internal class ApplicationHealthStateFilterConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthStateFilter Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthStateFilter GetFromJsonProperties(JsonReader reader)
        {
            var applicationNameFilter = default(string);
            var applicationTypeNameFilter = default(string);
            var healthStateFilter = default(int?);
            var serviceFilters = default(IEnumerable<ServiceHealthStateFilter>);
            var deployedApplicationFilters = default(IEnumerable<DeployedApplicationHealthStateFilter>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ApplicationNameFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationNameFilter = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeNameFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeNameFilter = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthStateFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStateFilter = reader.ReadValueAsInt();
                }
                else if (string.Compare("ServiceFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceFilters = reader.ReadList(ServiceHealthStateFilterConverter.Deserialize);
                }
                else if (string.Compare("DeployedApplicationFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deployedApplicationFilters = reader.ReadList(DeployedApplicationHealthStateFilterConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationHealthStateFilter(
                applicationNameFilter: applicationNameFilter,
                applicationTypeNameFilter: applicationTypeNameFilter,
                healthStateFilter: healthStateFilter,
                serviceFilters: serviceFilters,
                deployedApplicationFilters: deployedApplicationFilters);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationHealthStateFilter obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ApplicationNameFilter != null)
            {
                writer.WriteProperty(obj.ApplicationNameFilter, "ApplicationNameFilter", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ApplicationTypeNameFilter != null)
            {
                writer.WriteProperty(obj.ApplicationTypeNameFilter, "ApplicationTypeNameFilter", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HealthStateFilter != null)
            {
                writer.WriteProperty(obj.HealthStateFilter, "HealthStateFilter", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ServiceFilters != null)
            {
                writer.WriteEnumerableProperty(obj.ServiceFilters, "ServiceFilters", ServiceHealthStateFilterConverter.Serialize);
            }

            if (obj.DeployedApplicationFilters != null)
            {
                writer.WriteEnumerableProperty(obj.DeployedApplicationFilters, "DeployedApplicationFilters", DeployedApplicationHealthStateFilterConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
