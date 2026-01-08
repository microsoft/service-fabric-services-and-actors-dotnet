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
    /// Converter for <see cref="PartitionInformation" />.
    /// </summary>
    internal class PartitionInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionInformation GetFromJsonProperties(JsonReader reader)
        {
            PartitionInformation obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ServicePartitionKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ServicePartitionKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Int64Range", StringComparison.OrdinalIgnoreCase))
            {
                obj = Int64RangePartitionInformationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Named", StringComparison.OrdinalIgnoreCase))
            {
                obj = NamedPartitionInformationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Singleton", StringComparison.OrdinalIgnoreCase))
            {
                obj = SingletonPartitionInformationConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServicePartitionKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionInformation obj)
        {
            var kind = obj.ServicePartitionKind;
            if (kind.Equals(ServicePartitionKind.Int64Range))
            {
                Int64RangePartitionInformationConverter.Serialize(writer, (Int64RangePartitionInformation)obj);
            }
            else if (kind.Equals(ServicePartitionKind.Named))
            {
                NamedPartitionInformationConverter.Serialize(writer, (NamedPartitionInformation)obj);
            }
            else if (kind.Equals(ServicePartitionKind.Singleton))
            {
                SingletonPartitionInformationConverter.Serialize(writer, (SingletonPartitionInformation)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ServicePartitionKind.");
            }
        }
    }
}
