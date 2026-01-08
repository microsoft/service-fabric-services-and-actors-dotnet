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
    /// Converter for <see cref="CodePackageEntryPointStatistics" />.
    /// </summary>
    internal class CodePackageEntryPointStatisticsConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CodePackageEntryPointStatistics Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CodePackageEntryPointStatistics GetFromJsonProperties(JsonReader reader)
        {
            var lastExitCode = default(string);
            var lastActivationTime = default(DateTime?);
            var lastExitTime = default(DateTime?);
            var lastSuccessfulActivationTime = default(DateTime?);
            var lastSuccessfulExitTime = default(DateTime?);
            var activationCount = default(string);
            var activationFailureCount = default(string);
            var continuousActivationFailureCount = default(string);
            var exitCount = default(string);
            var exitFailureCount = default(string);
            var continuousExitFailureCount = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("LastExitCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastExitCode = reader.ReadValueAsString();
                }
                else if (string.Compare("LastActivationTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastActivationTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastExitTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastExitTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastSuccessfulActivationTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastSuccessfulActivationTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastSuccessfulExitTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastSuccessfulExitTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ActivationCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    activationCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ActivationFailureCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    activationFailureCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ContinuousActivationFailureCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continuousActivationFailureCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ExitCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ExitFailureCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitFailureCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ContinuousExitFailureCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continuousExitFailureCount = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CodePackageEntryPointStatistics(
                lastExitCode: lastExitCode,
                lastActivationTime: lastActivationTime,
                lastExitTime: lastExitTime,
                lastSuccessfulActivationTime: lastSuccessfulActivationTime,
                lastSuccessfulExitTime: lastSuccessfulExitTime,
                activationCount: activationCount,
                activationFailureCount: activationFailureCount,
                continuousActivationFailureCount: continuousActivationFailureCount,
                exitCount: exitCount,
                exitFailureCount: exitFailureCount,
                continuousExitFailureCount: continuousExitFailureCount);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CodePackageEntryPointStatistics obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.LastExitCode != null)
            {
                writer.WriteProperty(obj.LastExitCode, "LastExitCode", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastActivationTime != null)
            {
                writer.WriteProperty(obj.LastActivationTime, "LastActivationTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastExitTime != null)
            {
                writer.WriteProperty(obj.LastExitTime, "LastExitTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastSuccessfulActivationTime != null)
            {
                writer.WriteProperty(obj.LastSuccessfulActivationTime, "LastSuccessfulActivationTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastSuccessfulExitTime != null)
            {
                writer.WriteProperty(obj.LastSuccessfulExitTime, "LastSuccessfulExitTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ActivationCount != null)
            {
                writer.WriteProperty(obj.ActivationCount, "ActivationCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ActivationFailureCount != null)
            {
                writer.WriteProperty(obj.ActivationFailureCount, "ActivationFailureCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContinuousActivationFailureCount != null)
            {
                writer.WriteProperty(obj.ContinuousActivationFailureCount, "ContinuousActivationFailureCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ExitCount != null)
            {
                writer.WriteProperty(obj.ExitCount, "ExitCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ExitFailureCount != null)
            {
                writer.WriteProperty(obj.ExitFailureCount, "ExitFailureCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContinuousExitFailureCount != null)
            {
                writer.WriteProperty(obj.ContinuousExitFailureCount, "ContinuousExitFailureCount", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
