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
    /// Converter for <see cref="ApplicationHealthPolicy" />.
    /// </summary>
    internal class ApplicationHealthPolicyConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthPolicy Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthPolicy GetFromJsonProperties(JsonReader reader)
        {
            var considerWarningAsError = default(bool?);
            var maxPercentUnhealthyDeployedApplications = default(int?);
            var defaultServiceTypeHealthPolicy = default(ServiceTypeHealthPolicy);
            var serviceTypeHealthPolicyMap = default(IEnumerable<ServiceTypeHealthPolicyMapItem>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ConsiderWarningAsError", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    considerWarningAsError = reader.ReadValueAsBool();
                }
                else if (string.Compare("MaxPercentUnhealthyDeployedApplications", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyDeployedApplications = reader.ReadValueAsInt();
                }
                else if (string.Compare("DefaultServiceTypeHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultServiceTypeHealthPolicy = ServiceTypeHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceTypeHealthPolicyMap", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeHealthPolicyMap = reader.ReadList(ServiceTypeHealthPolicyMapItemConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationHealthPolicy(
                considerWarningAsError: considerWarningAsError,
                maxPercentUnhealthyDeployedApplications: maxPercentUnhealthyDeployedApplications,
                defaultServiceTypeHealthPolicy: defaultServiceTypeHealthPolicy,
                serviceTypeHealthPolicyMap: serviceTypeHealthPolicyMap);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationHealthPolicy obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ConsiderWarningAsError != null)
            {
                writer.WriteProperty(obj.ConsiderWarningAsError, "ConsiderWarningAsError", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.MaxPercentUnhealthyDeployedApplications != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyDeployedApplications, "MaxPercentUnhealthyDeployedApplications", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.DefaultServiceTypeHealthPolicy != null)
            {
                writer.WriteProperty(obj.DefaultServiceTypeHealthPolicy, "DefaultServiceTypeHealthPolicy", ServiceTypeHealthPolicyConverter.Serialize);
            }

            if (obj.ServiceTypeHealthPolicyMap != null)
            {
                writer.WriteEnumerableProperty(obj.ServiceTypeHealthPolicyMap, "ServiceTypeHealthPolicyMap", ServiceTypeHealthPolicyMapItemConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
