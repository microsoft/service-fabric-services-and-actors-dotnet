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
    /// Converter for <see cref="PartitionSchemeDescription" />.
    /// </summary>
    internal class PartitionSchemeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionSchemeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionSchemeDescription GetFromJsonProperties(JsonReader reader)
        {
            PartitionSchemeDescription obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("PartitionScheme", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is PartitionScheme.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Named", StringComparison.OrdinalIgnoreCase))
            {
                obj = NamedPartitionSchemeDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Singleton", StringComparison.OrdinalIgnoreCase))
            {
                obj = SingletonPartitionSchemeDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("UniformInt64Range", StringComparison.OrdinalIgnoreCase))
            {
                obj = UniformInt64RangePartitionSchemeDescriptionConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown PartitionScheme.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionSchemeDescription obj)
        {
            var kind = obj.PartitionScheme;
            if (kind.Equals(PartitionScheme.Named))
            {
                NamedPartitionSchemeDescriptionConverter.Serialize(writer, (NamedPartitionSchemeDescription)obj);
            }
            else if (kind.Equals(PartitionScheme.Singleton))
            {
                SingletonPartitionSchemeDescriptionConverter.Serialize(writer, (SingletonPartitionSchemeDescription)obj);
            }
            else if (kind.Equals(PartitionScheme.UniformInt64Range))
            {
                UniformInt64RangePartitionSchemeDescriptionConverter.Serialize(writer, (UniformInt64RangePartitionSchemeDescription)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown PartitionScheme.");
            }
        }
    }
}
