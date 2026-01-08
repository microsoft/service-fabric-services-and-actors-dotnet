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
    /// Converter for <see cref="DeployedStatelessServiceInstanceDetailInfo" />.
    /// </summary>
    internal class DeployedStatelessServiceInstanceDetailInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatelessServiceInstanceDetailInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatelessServiceInstanceDetailInfo GetFromJsonProperties(JsonReader reader)
        {
            var serviceName = default(ServiceName);
            var partitionId = default(PartitionId);
            var currentServiceOperation = default(ServiceOperationName?);
            var currentServiceOperationStartTimeUtc = default(DateTime?);
            var reportedLoad = default(IEnumerable<LoadMetricReportInfo>);
            var instanceId = default(ReplicaId);
            var deployedServiceReplicaQueryResult = default(DeployedStatelessServiceInstanceInfo);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = ServiceNameConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("CurrentServiceOperation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentServiceOperation = ServiceOperationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("CurrentServiceOperationStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentServiceOperationStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ReportedLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reportedLoad = reader.ReadList(LoadMetricReportInfoConverter.Deserialize);
                }
                else if (string.Compare("InstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceId = ReplicaIdConverter.Deserialize(reader);
                }
                else if (string.Compare("DeployedServiceReplicaQueryResult", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deployedServiceReplicaQueryResult = DeployedStatelessServiceInstanceInfoConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedStatelessServiceInstanceDetailInfo(
                serviceName: serviceName,
                partitionId: partitionId,
                currentServiceOperation: currentServiceOperation,
                currentServiceOperationStartTimeUtc: currentServiceOperationStartTimeUtc,
                reportedLoad: reportedLoad,
                instanceId: instanceId,
                deployedServiceReplicaQueryResult: deployedServiceReplicaQueryResult);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedStatelessServiceInstanceDetailInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.CurrentServiceOperation, "CurrentServiceOperation", ServiceOperationNameConverter.Serialize);
            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", ServiceNameConverter.Serialize);
            }

            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.CurrentServiceOperationStartTimeUtc != null)
            {
                writer.WriteProperty(obj.CurrentServiceOperationStartTimeUtc, "CurrentServiceOperationStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ReportedLoad != null)
            {
                writer.WriteEnumerableProperty(obj.ReportedLoad, "ReportedLoad", LoadMetricReportInfoConverter.Serialize);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", ReplicaIdConverter.Serialize);
            }

            if (obj.DeployedServiceReplicaQueryResult != null)
            {
                writer.WriteProperty(obj.DeployedServiceReplicaQueryResult, "DeployedServiceReplicaQueryResult", DeployedStatelessServiceInstanceInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
