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
    /// Converter for <see cref="PartitionMetricLoadDescription" />.
    /// </summary>
    internal class PartitionMetricLoadDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionMetricLoadDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionMetricLoadDescription GetFromJsonProperties(JsonReader reader)
        {
            var partitionId = default(PartitionId);
            var primaryReplicaLoadEntries = default(IEnumerable<MetricLoadDescription>);
            var secondaryReplicasOrInstancesLoadEntries = default(IEnumerable<MetricLoadDescription>);
            var secondaryReplicaOrInstanceLoadEntriesPerNode = default(IEnumerable<ReplicaMetricLoadDescription>);
            var auxiliaryReplicasLoadEntries = default(IEnumerable<MetricLoadDescription>);
            var auxiliaryReplicaLoadEntriesPerNode = default(IEnumerable<ReplicaMetricLoadDescription>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("PrimaryReplicaLoadEntries", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryReplicaLoadEntries = reader.ReadList(MetricLoadDescriptionConverter.Deserialize);
                }
                else if (string.Compare("SecondaryReplicasOrInstancesLoadEntries", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryReplicasOrInstancesLoadEntries = reader.ReadList(MetricLoadDescriptionConverter.Deserialize);
                }
                else if (string.Compare("SecondaryReplicaOrInstanceLoadEntriesPerNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryReplicaOrInstanceLoadEntriesPerNode = reader.ReadList(ReplicaMetricLoadDescriptionConverter.Deserialize);
                }
                else if (string.Compare("AuxiliaryReplicasLoadEntries", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryReplicasLoadEntries = reader.ReadList(MetricLoadDescriptionConverter.Deserialize);
                }
                else if (string.Compare("AuxiliaryReplicaLoadEntriesPerNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryReplicaLoadEntriesPerNode = reader.ReadList(ReplicaMetricLoadDescriptionConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionMetricLoadDescription(
                partitionId: partitionId,
                primaryReplicaLoadEntries: primaryReplicaLoadEntries,
                secondaryReplicasOrInstancesLoadEntries: secondaryReplicasOrInstancesLoadEntries,
                secondaryReplicaOrInstanceLoadEntriesPerNode: secondaryReplicaOrInstanceLoadEntriesPerNode,
                auxiliaryReplicasLoadEntries: auxiliaryReplicasLoadEntries,
                auxiliaryReplicaLoadEntriesPerNode: auxiliaryReplicaLoadEntriesPerNode);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionMetricLoadDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.PrimaryReplicaLoadEntries != null)
            {
                writer.WriteEnumerableProperty(obj.PrimaryReplicaLoadEntries, "PrimaryReplicaLoadEntries", MetricLoadDescriptionConverter.Serialize);
            }

            if (obj.SecondaryReplicasOrInstancesLoadEntries != null)
            {
                writer.WriteEnumerableProperty(obj.SecondaryReplicasOrInstancesLoadEntries, "SecondaryReplicasOrInstancesLoadEntries", MetricLoadDescriptionConverter.Serialize);
            }

            if (obj.SecondaryReplicaOrInstanceLoadEntriesPerNode != null)
            {
                writer.WriteEnumerableProperty(obj.SecondaryReplicaOrInstanceLoadEntriesPerNode, "SecondaryReplicaOrInstanceLoadEntriesPerNode", ReplicaMetricLoadDescriptionConverter.Serialize);
            }

            if (obj.AuxiliaryReplicasLoadEntries != null)
            {
                writer.WriteEnumerableProperty(obj.AuxiliaryReplicasLoadEntries, "AuxiliaryReplicasLoadEntries", MetricLoadDescriptionConverter.Serialize);
            }

            if (obj.AuxiliaryReplicaLoadEntriesPerNode != null)
            {
                writer.WriteEnumerableProperty(obj.AuxiliaryReplicaLoadEntriesPerNode, "AuxiliaryReplicaLoadEntriesPerNode", ReplicaMetricLoadDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
