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
    /// Converter for <see cref="PartitionLoadInformation" />.
    /// </summary>
    internal class PartitionLoadInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionLoadInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionLoadInformation GetFromJsonProperties(JsonReader reader)
        {
            var partitionId = default(PartitionId);
            var primaryLoadMetricReports = default(IEnumerable<LoadMetricReport>);
            var secondaryLoadMetricReports = default(IEnumerable<LoadMetricReport>);
            var auxiliaryLoadMetricReports = default(IEnumerable<LoadMetricReport>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("PrimaryLoadMetricReports", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryLoadMetricReports = reader.ReadList(LoadMetricReportConverter.Deserialize);
                }
                else if (string.Compare("SecondaryLoadMetricReports", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryLoadMetricReports = reader.ReadList(LoadMetricReportConverter.Deserialize);
                }
                else if (string.Compare("AuxiliaryLoadMetricReports", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryLoadMetricReports = reader.ReadList(LoadMetricReportConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionLoadInformation(
                partitionId: partitionId,
                primaryLoadMetricReports: primaryLoadMetricReports,
                secondaryLoadMetricReports: secondaryLoadMetricReports,
                auxiliaryLoadMetricReports: auxiliaryLoadMetricReports);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionLoadInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.PrimaryLoadMetricReports != null)
            {
                writer.WriteEnumerableProperty(obj.PrimaryLoadMetricReports, "PrimaryLoadMetricReports", LoadMetricReportConverter.Serialize);
            }

            if (obj.SecondaryLoadMetricReports != null)
            {
                writer.WriteEnumerableProperty(obj.SecondaryLoadMetricReports, "SecondaryLoadMetricReports", LoadMetricReportConverter.Serialize);
            }

            if (obj.AuxiliaryLoadMetricReports != null)
            {
                writer.WriteEnumerableProperty(obj.AuxiliaryLoadMetricReports, "AuxiliaryLoadMetricReports", LoadMetricReportConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
