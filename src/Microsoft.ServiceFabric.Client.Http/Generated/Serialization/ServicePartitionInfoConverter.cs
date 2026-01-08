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
    /// Converter for <see cref="ServicePartitionInfo" />.
    /// </summary>
    internal class ServicePartitionInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServicePartitionInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServicePartitionInfo GetFromJsonProperties(JsonReader reader)
        {
            ServicePartitionInfo obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ServiceKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ServiceKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Stateful", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatefulServicePartitionInfoConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Stateless", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatelessServicePartitionInfoConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, ServicePartitionInfo obj)
        {
            var kind = obj.ServiceKind;
            if (kind.Equals(ServiceKind.Stateful))
            {
                StatefulServicePartitionInfoConverter.Serialize(writer, (StatefulServicePartitionInfo)obj);
            }
            else if (kind.Equals(ServiceKind.Stateless))
            {
                StatelessServicePartitionInfoConverter.Serialize(writer, (StatelessServicePartitionInfo)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServiceKind.");
            }
        }
    }
}
