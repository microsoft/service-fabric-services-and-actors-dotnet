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
    /// Converter for <see cref="ServiceCreatedEvent" />.
    /// </summary>
    internal class ServiceCreatedEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceCreatedEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceCreatedEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var serviceId = default(string);
            var serviceTypeName = default(string);
            var applicationName = default(string);
            var applicationTypeName = default(string);
            var serviceInstance = default(long?);
            var isStateful = default(bool?);
            var partitionCount = default(int?);
            var targetReplicaSetSize = default(int?);
            var minReplicaSetSize = default(int?);
            var servicePackageVersion = default(string);
            var partitionId = default(PartitionId);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EventInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    eventInstanceId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("Category", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    category = reader.ReadValueAsString();
                }
                else if (string.Compare("TimeStamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("HasCorrelatedEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasCorrelatedEvents = reader.ReadValueAsBool();
                }
                else if (string.Compare("ServiceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceId = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceInstance", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceInstance = reader.ReadValueAsLong();
                }
                else if (string.Compare("IsStateful", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isStateful = reader.ReadValueAsBool();
                }
                else if (string.Compare("PartitionCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("TargetReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetReplicaSetSize = reader.ReadValueAsInt();
                }
                else if (string.Compare("MinReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minReplicaSetSize = reader.ReadValueAsInt();
                }
                else if (string.Compare("ServicePackageVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceCreatedEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                serviceId: serviceId,
                serviceTypeName: serviceTypeName,
                applicationName: applicationName,
                applicationTypeName: applicationTypeName,
                serviceInstance: serviceInstance,
                isStateful: isStateful,
                partitionCount: partitionCount,
                targetReplicaSetSize: targetReplicaSetSize,
                minReplicaSetSize: minReplicaSetSize,
                servicePackageVersion: servicePackageVersion,
                partitionId: partitionId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceCreatedEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.ServiceId, "ServiceId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServiceTypeName, "ServiceTypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationName, "ApplicationName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationTypeName, "ApplicationTypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServiceInstance, "ServiceInstance", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.IsStateful, "IsStateful", JsonWriterExtensions.WriteBoolValue);
            writer.WriteProperty(obj.PartitionCount, "PartitionCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.TargetReplicaSetSize, "TargetReplicaSetSize", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.MinReplicaSetSize, "MinReplicaSetSize", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.ServicePackageVersion, "ServicePackageVersion", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
