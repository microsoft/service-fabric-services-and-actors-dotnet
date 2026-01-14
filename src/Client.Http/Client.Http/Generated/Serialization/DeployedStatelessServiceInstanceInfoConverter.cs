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
    /// Converter for <see cref="DeployedStatelessServiceInstanceInfo" />.
    /// </summary>
    internal class DeployedStatelessServiceInstanceInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatelessServiceInstanceInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedStatelessServiceInstanceInfo GetFromJsonProperties(JsonReader reader)
        {
            var serviceName = default(ServiceName);
            var serviceTypeName = default(string);
            var serviceManifestName = default(string);
            var codePackageName = default(string);
            var partitionId = default(PartitionId);
            var replicaStatus = default(ReplicaStatus?);
            var address = default(string);
            var servicePackageActivationId = default(string);
            var hostProcessId = default(string);
            var instanceId = default(ReplicaId);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = ServiceNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("CodePackageName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageName = reader.ReadValueAsString();
                }
                else if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("ReplicaStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaStatus = ReplicaStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("Address", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    address = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageActivationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationId = reader.ReadValueAsString();
                }
                else if (string.Compare("HostProcessId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hostProcessId = reader.ReadValueAsString();
                }
                else if (string.Compare("InstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceId = ReplicaIdConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedStatelessServiceInstanceInfo(
                serviceName: serviceName,
                serviceTypeName: serviceTypeName,
                serviceManifestName: serviceManifestName,
                codePackageName: codePackageName,
                partitionId: partitionId,
                replicaStatus: replicaStatus,
                address: address,
                servicePackageActivationId: servicePackageActivationId,
                hostProcessId: hostProcessId,
                instanceId: instanceId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedStatelessServiceInstanceInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.ReplicaStatus, "ReplicaStatus", ReplicaStatusConverter.Serialize);
            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", ServiceNameConverter.Serialize);
            }

            if (obj.ServiceTypeName != null)
            {
                writer.WriteProperty(obj.ServiceTypeName, "ServiceTypeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceManifestName != null)
            {
                writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CodePackageName != null)
            {
                writer.WriteProperty(obj.CodePackageName, "CodePackageName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.Address != null)
            {
                writer.WriteProperty(obj.Address, "Address", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServicePackageActivationId != null)
            {
                writer.WriteProperty(obj.ServicePackageActivationId, "ServicePackageActivationId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HostProcessId != null)
            {
                writer.WriteProperty(obj.HostProcessId, "HostProcessId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", ReplicaIdConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
