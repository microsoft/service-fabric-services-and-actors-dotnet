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
    /// Converter for <see cref="AverageServiceLoadScalingTrigger" />.
    /// </summary>
    internal class AverageServiceLoadScalingTriggerConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AverageServiceLoadScalingTrigger Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AverageServiceLoadScalingTrigger GetFromJsonProperties(JsonReader reader)
        {
            var metricName = default(string);
            var lowerLoadThreshold = default(string);
            var upperLoadThreshold = default(string);
            var scaleIntervalInSeconds = default(long?);
            var useOnlyPrimaryLoad = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MetricName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metricName = reader.ReadValueAsString();
                }
                else if (string.Compare("LowerLoadThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lowerLoadThreshold = reader.ReadValueAsString();
                }
                else if (string.Compare("UpperLoadThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upperLoadThreshold = reader.ReadValueAsString();
                }
                else if (string.Compare("ScaleIntervalInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scaleIntervalInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("UseOnlyPrimaryLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    useOnlyPrimaryLoad = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AverageServiceLoadScalingTrigger(
                metricName: metricName,
                lowerLoadThreshold: lowerLoadThreshold,
                upperLoadThreshold: upperLoadThreshold,
                scaleIntervalInSeconds: scaleIntervalInSeconds,
                useOnlyPrimaryLoad: useOnlyPrimaryLoad);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AverageServiceLoadScalingTrigger obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ScalingTriggerKindConverter.Serialize);
            writer.WriteProperty(obj.MetricName, "MetricName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.LowerLoadThreshold, "LowerLoadThreshold", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.UpperLoadThreshold, "UpperLoadThreshold", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ScaleIntervalInSeconds, "ScaleIntervalInSeconds", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.UseOnlyPrimaryLoad, "UseOnlyPrimaryLoad", JsonWriterExtensions.WriteBoolValue);
            writer.WriteEndObject();
        }
    }
}
