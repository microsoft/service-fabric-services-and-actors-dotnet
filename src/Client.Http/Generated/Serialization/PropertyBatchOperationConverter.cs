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
    /// Converter for <see cref="PropertyBatchOperation" />.
    /// </summary>
    internal class PropertyBatchOperationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyBatchOperation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyBatchOperation GetFromJsonProperties(JsonReader reader)
        {
            PropertyBatchOperation obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("CheckExists", StringComparison.OrdinalIgnoreCase))
            {
                obj = CheckExistsPropertyBatchOperationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("CheckSequence", StringComparison.OrdinalIgnoreCase))
            {
                obj = CheckSequencePropertyBatchOperationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("CheckValue", StringComparison.OrdinalIgnoreCase))
            {
                obj = CheckValuePropertyBatchOperationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeletePropertyBatchOperationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Get", StringComparison.OrdinalIgnoreCase))
            {
                obj = GetPropertyBatchOperationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Put", StringComparison.OrdinalIgnoreCase))
            {
                obj = PutPropertyBatchOperationConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, PropertyBatchOperation obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(PropertyBatchOperationKind.CheckExists))
            {
                CheckExistsPropertyBatchOperationConverter.Serialize(writer, (CheckExistsPropertyBatchOperation)obj);
            }
            else if (kind.Equals(PropertyBatchOperationKind.CheckSequence))
            {
                CheckSequencePropertyBatchOperationConverter.Serialize(writer, (CheckSequencePropertyBatchOperation)obj);
            }
            else if (kind.Equals(PropertyBatchOperationKind.CheckValue))
            {
                CheckValuePropertyBatchOperationConverter.Serialize(writer, (CheckValuePropertyBatchOperation)obj);
            }
            else if (kind.Equals(PropertyBatchOperationKind.Delete))
            {
                DeletePropertyBatchOperationConverter.Serialize(writer, (DeletePropertyBatchOperation)obj);
            }
            else if (kind.Equals(PropertyBatchOperationKind.Get))
            {
                GetPropertyBatchOperationConverter.Serialize(writer, (GetPropertyBatchOperation)obj);
            }
            else if (kind.Equals(PropertyBatchOperationKind.Put))
            {
                PutPropertyBatchOperationConverter.Serialize(writer, (PutPropertyBatchOperation)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
