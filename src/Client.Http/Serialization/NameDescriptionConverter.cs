// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    /// <summary>
    /// Converts FabricName to NameDescription(as specified in swagger).
    /// </summary>
    static class NameDescriptionConverter
    {
        internal static FabricName Deserialize(JsonReader reader)
        {
            reader.ReadStartObject();
            string name = default;

            do
            {
                string propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.Ordinal) == 0)
                    name = reader.ReadValueAsString();
                else
                    reader.SkipPropertyValue();
            }
            while (reader.TokenType != JsonToken.EndObject);
            
            reader.ReadEndObject();
            return new FabricName(name);
        }

        internal static void Serialize(JsonWriter writer, FabricName obj)
        {
            writer.WriteStartObject();
            writer.WriteProperty(obj.ToString(), "Name", JsonWriterExtensions.WriteStringValue);
            writer.WriteEndObject();
        }
    }
}
