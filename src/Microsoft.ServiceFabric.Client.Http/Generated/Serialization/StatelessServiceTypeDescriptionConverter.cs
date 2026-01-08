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
    /// Converter for <see cref="StatelessServiceTypeDescription" />.
    /// </summary>
    internal class StatelessServiceTypeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceTypeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceTypeDescription GetFromJsonProperties(JsonReader reader)
        {
            var isStateful = default(bool?);
            var serviceTypeName = default(string);
            var placementConstraints = default(string);
            var loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>);
            var servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>);
            var extensions = default(IEnumerable<ServiceTypeExtensionDescription>);
            var useImplicitHost = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("IsStateful", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isStateful = reader.ReadValueAsBool();
                }
                else if (string.Compare("ServiceTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("PlacementConstraints", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    placementConstraints = reader.ReadValueAsString();
                }
                else if (string.Compare("LoadMetrics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    loadMetrics = reader.ReadList(ServiceLoadMetricDescriptionConverter.Deserialize);
                }
                else if (string.Compare("ServicePlacementPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePlacementPolicies = reader.ReadList(ServicePlacementPolicyDescriptionConverter.Deserialize);
                }
                else if (string.Compare("Extensions", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    extensions = reader.ReadList(ServiceTypeExtensionDescriptionConverter.Deserialize);
                }
                else if (string.Compare("UseImplicitHost", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    useImplicitHost = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatelessServiceTypeDescription(
                isStateful: isStateful,
                serviceTypeName: serviceTypeName,
                placementConstraints: placementConstraints,
                loadMetrics: loadMetrics,
                servicePlacementPolicies: servicePlacementPolicies,
                extensions: extensions,
                useImplicitHost: useImplicitHost);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatelessServiceTypeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ServiceKindConverter.Serialize);
            if (obj.IsStateful != null)
            {
                writer.WriteProperty(obj.IsStateful, "IsStateful", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ServiceTypeName != null)
            {
                writer.WriteProperty(obj.ServiceTypeName, "ServiceTypeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PlacementConstraints != null)
            {
                writer.WriteProperty(obj.PlacementConstraints, "PlacementConstraints", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LoadMetrics != null)
            {
                writer.WriteEnumerableProperty(obj.LoadMetrics, "LoadMetrics", ServiceLoadMetricDescriptionConverter.Serialize);
            }

            if (obj.ServicePlacementPolicies != null)
            {
                writer.WriteEnumerableProperty(obj.ServicePlacementPolicies, "ServicePlacementPolicies", ServicePlacementPolicyDescriptionConverter.Serialize);
            }

            if (obj.Extensions != null)
            {
                writer.WriteEnumerableProperty(obj.Extensions, "Extensions", ServiceTypeExtensionDescriptionConverter.Serialize);
            }

            if (obj.UseImplicitHost != null)
            {
                writer.WriteProperty(obj.UseImplicitHost, "UseImplicitHost", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
