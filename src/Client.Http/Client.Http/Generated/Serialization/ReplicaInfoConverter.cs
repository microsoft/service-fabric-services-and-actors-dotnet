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
    /// Converter for <see cref="ReplicaInfo" />.
    /// </summary>
    internal class ReplicaInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaInfo GetFromJsonProperties(JsonReader reader)
        {
            ReplicaInfo obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ServiceKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ServiceKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Stateful", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatefulServiceReplicaInfoConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Stateless", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatelessServiceInstanceInfoConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, ReplicaInfo obj)
        {
            var kind = obj.ServiceKind;
            if (kind.Equals(ServiceKind.Stateful))
            {
                StatefulServiceReplicaInfoConverter.Serialize(writer, (StatefulServiceReplicaInfo)obj);
            }
            else if (kind.Equals(ServiceKind.Stateless))
            {
                StatelessServiceInstanceInfoConverter.Serialize(writer, (StatelessServiceInstanceInfo)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServiceKind.");
            }
        }
    }
}
