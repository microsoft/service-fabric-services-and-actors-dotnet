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
    /// Converts FabricName to NameDescription(as specified in swagger).
    /// </summary>
    internal class NameDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of NameDescription and returns it as FabricName.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FabricName Deserialize(JsonReader reader)
        {
            reader.ReadStartObject();
            var name = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.Ordinal) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);
            
            reader.ReadEndObject();
            return new FabricName(name);
        }

        /// <summary>
        /// Serializes the FabricName object to JSON representing NameDescription as defined in Swagger.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FabricName obj)
        {
            writer.WriteStartObject();
            writer.WriteProperty(obj.ToString(), "Name", JsonWriterExtensions.WriteStringValue);
            writer.WriteEndObject();
        }
    }
}
