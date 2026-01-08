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
    /// Converter for <see cref="PropertyValue" />.
    /// </summary>
    internal class PropertyValueConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyValue Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyValue GetFromJsonProperties(JsonReader reader)
        {
            PropertyValue obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Binary", StringComparison.OrdinalIgnoreCase))
            {
                obj = BinaryPropertyValueConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Int64", StringComparison.OrdinalIgnoreCase))
            {
                obj = Int64PropertyValueConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Double", StringComparison.OrdinalIgnoreCase))
            {
                obj = DoublePropertyValueConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("String", StringComparison.OrdinalIgnoreCase))
            {
                obj = StringPropertyValueConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Guid", StringComparison.OrdinalIgnoreCase))
            {
                obj = GuidPropertyValueConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PropertyValue obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(PropertyValueKind.Binary))
            {
                BinaryPropertyValueConverter.Serialize(writer, (BinaryPropertyValue)obj);
            }
            else if (kind.Equals(PropertyValueKind.Int64))
            {
                Int64PropertyValueConverter.Serialize(writer, (Int64PropertyValue)obj);
            }
            else if (kind.Equals(PropertyValueKind.Double))
            {
                DoublePropertyValueConverter.Serialize(writer, (DoublePropertyValue)obj);
            }
            else if (kind.Equals(PropertyValueKind.String))
            {
                StringPropertyValueConverter.Serialize(writer, (StringPropertyValue)obj);
            }
            else if (kind.Equals(PropertyValueKind.Guid))
            {
                GuidPropertyValueConverter.Serialize(writer, (GuidPropertyValue)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
