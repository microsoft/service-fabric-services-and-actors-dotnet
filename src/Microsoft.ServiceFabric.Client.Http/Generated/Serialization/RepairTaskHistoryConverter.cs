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
    /// Converter for <see cref="RepairTaskHistory" />.
    /// </summary>
    internal class RepairTaskHistoryConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTaskHistory Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTaskHistory GetFromJsonProperties(JsonReader reader)
        {
            var createdUtcTimestamp = default(DateTime?);
            var claimedUtcTimestamp = default(DateTime?);
            var preparingUtcTimestamp = default(DateTime?);
            var approvedUtcTimestamp = default(DateTime?);
            var executingUtcTimestamp = default(DateTime?);
            var restoringUtcTimestamp = default(DateTime?);
            var completedUtcTimestamp = default(DateTime?);
            var preparingHealthCheckStartUtcTimestamp = default(DateTime?);
            var preparingHealthCheckEndUtcTimestamp = default(DateTime?);
            var restoringHealthCheckStartUtcTimestamp = default(DateTime?);
            var restoringHealthCheckEndUtcTimestamp = default(DateTime?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("CreatedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    createdUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ClaimedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    claimedUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("PreparingUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    preparingUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ApprovedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    approvedUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ExecutingUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    executingUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("RestoringUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoringUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("CompletedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    completedUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("PreparingHealthCheckStartUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    preparingHealthCheckStartUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("PreparingHealthCheckEndUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    preparingHealthCheckEndUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("RestoringHealthCheckStartUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoringHealthCheckStartUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("RestoringHealthCheckEndUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoringHealthCheckEndUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RepairTaskHistory(
                createdUtcTimestamp: createdUtcTimestamp,
                claimedUtcTimestamp: claimedUtcTimestamp,
                preparingUtcTimestamp: preparingUtcTimestamp,
                approvedUtcTimestamp: approvedUtcTimestamp,
                executingUtcTimestamp: executingUtcTimestamp,
                restoringUtcTimestamp: restoringUtcTimestamp,
                completedUtcTimestamp: completedUtcTimestamp,
                preparingHealthCheckStartUtcTimestamp: preparingHealthCheckStartUtcTimestamp,
                preparingHealthCheckEndUtcTimestamp: preparingHealthCheckEndUtcTimestamp,
                restoringHealthCheckStartUtcTimestamp: restoringHealthCheckStartUtcTimestamp,
                restoringHealthCheckEndUtcTimestamp: restoringHealthCheckEndUtcTimestamp);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RepairTaskHistory obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.CreatedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.CreatedUtcTimestamp, "CreatedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ClaimedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.ClaimedUtcTimestamp, "ClaimedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.PreparingUtcTimestamp != null)
            {
                writer.WriteProperty(obj.PreparingUtcTimestamp, "PreparingUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ApprovedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.ApprovedUtcTimestamp, "ApprovedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ExecutingUtcTimestamp != null)
            {
                writer.WriteProperty(obj.ExecutingUtcTimestamp, "ExecutingUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.RestoringUtcTimestamp != null)
            {
                writer.WriteProperty(obj.RestoringUtcTimestamp, "RestoringUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.CompletedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.CompletedUtcTimestamp, "CompletedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.PreparingHealthCheckStartUtcTimestamp != null)
            {
                writer.WriteProperty(obj.PreparingHealthCheckStartUtcTimestamp, "PreparingHealthCheckStartUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.PreparingHealthCheckEndUtcTimestamp != null)
            {
                writer.WriteProperty(obj.PreparingHealthCheckEndUtcTimestamp, "PreparingHealthCheckEndUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.RestoringHealthCheckStartUtcTimestamp != null)
            {
                writer.WriteProperty(obj.RestoringHealthCheckStartUtcTimestamp, "RestoringHealthCheckStartUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.RestoringHealthCheckEndUtcTimestamp != null)
            {
                writer.WriteProperty(obj.RestoringHealthCheckEndUtcTimestamp, "RestoringHealthCheckEndUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            writer.WriteEndObject();
        }
    }
}
