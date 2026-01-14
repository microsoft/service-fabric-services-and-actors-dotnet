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
    /// Converter for <see cref="DeployedServiceReplicaDetailInfo" />.
    /// </summary>
    internal class DeployedServiceReplicaDetailInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedServiceReplicaDetailInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedServiceReplicaDetailInfo GetFromJsonProperties(JsonReader reader)
        {
            DeployedServiceReplicaDetailInfo obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ServiceKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ServiceKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Stateful", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedStatefulServiceReplicaDetailInfoConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Stateless", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedStatelessServiceInstanceDetailInfoConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServiceKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedServiceReplicaDetailInfo obj)
        {
            var kind = obj.ServiceKind;
            if (kind.Equals(ServiceKind.Stateful))
            {
                DeployedStatefulServiceReplicaDetailInfoConverter.Serialize(writer, (DeployedStatefulServiceReplicaDetailInfo)obj);
            }
            else if (kind.Equals(ServiceKind.Stateless))
            {
                DeployedStatelessServiceInstanceDetailInfoConverter.Serialize(writer, (DeployedStatelessServiceInstanceDetailInfo)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServiceKind.");
            }
        }
    }
}
