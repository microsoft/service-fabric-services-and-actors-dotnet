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
    /// Converter for <see cref="DeployedStatefulServiceReplicaDetailInfo" />.
    /// </summary>
    internal class DeployedStatefulServiceReplicaDetailInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatefulServiceReplicaDetailInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatefulServiceReplicaDetailInfo GetFromJsonProperties(JsonReader reader)
        {
            var serviceName = default(ServiceName);
            var partitionId = default(PartitionId);
            var currentServiceOperation = default(ServiceOperationName?);
            var currentServiceOperationStartTimeUtc = default(DateTime?);
            var reportedLoad = default(IEnumerable<LoadMetricReportInfo>);
            var replicaId = default(ReplicaId);
            var currentReplicatorOperation = default(ReplicatorOperationName?);
            var readStatus = default(PartitionAccessStatus?);
            var writeStatus = default(PartitionAccessStatus?);
            var replicatorStatus = default(ReplicatorStatus);
            var replicaStatus = default(KeyValueStoreReplicaStatus);
            var deployedServiceReplicaQueryResult = default(DeployedStatefulServiceReplicaInfo);

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
                else if (string.Compare("ReplicaId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaId = ReplicaIdConverter.Deserialize(reader);
                }
                else if (string.Compare("CurrentReplicatorOperation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentReplicatorOperation = ReplicatorOperationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ReadStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    readStatus = PartitionAccessStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("WriteStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    writeStatus = PartitionAccessStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("ReplicatorStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicatorStatus = ReplicatorStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("ReplicaStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaStatus = KeyValueStoreReplicaStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("DeployedServiceReplicaQueryResult", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deployedServiceReplicaQueryResult = DeployedStatefulServiceReplicaInfoConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedStatefulServiceReplicaDetailInfo(
                serviceName: serviceName,
                partitionId: partitionId,
                currentServiceOperation: currentServiceOperation,
                currentServiceOperationStartTimeUtc: currentServiceOperationStartTimeUtc,
                reportedLoad: reportedLoad,
                replicaId: replicaId,
                currentReplicatorOperation: currentReplicatorOperation,
                readStatus: readStatus,
                writeStatus: writeStatus,
                replicatorStatus: replicatorStatus,
                replicaStatus: replicaStatus,
                deployedServiceReplicaQueryResult: deployedServiceReplicaQueryResult);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedStatefulServiceReplicaDetailInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.CurrentServiceOperation, "CurrentServiceOperation", ServiceOperationNameConverter.Serialize);
            writer.WriteProperty(obj.CurrentReplicatorOperation, "CurrentReplicatorOperation", ReplicatorOperationNameConverter.Serialize);
            writer.WriteProperty(obj.ReadStatus, "ReadStatus", PartitionAccessStatusConverter.Serialize);
            writer.WriteProperty(obj.WriteStatus, "WriteStatus", PartitionAccessStatusConverter.Serialize);
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

            if (obj.ReplicaId != null)
            {
                writer.WriteProperty(obj.ReplicaId, "ReplicaId", ReplicaIdConverter.Serialize);
            }

            if (obj.ReplicatorStatus != null)
            {
                writer.WriteProperty(obj.ReplicatorStatus, "ReplicatorStatus", ReplicatorStatusConverter.Serialize);
            }

            if (obj.ReplicaStatus != null)
            {
                writer.WriteProperty(obj.ReplicaStatus, "ReplicaStatus", KeyValueStoreReplicaStatusConverter.Serialize);
            }

            if (obj.DeployedServiceReplicaQueryResult != null)
            {
                writer.WriteProperty(obj.DeployedServiceReplicaQueryResult, "DeployedServiceReplicaQueryResult", DeployedStatefulServiceReplicaInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
