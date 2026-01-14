// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class ApplicationParametersConverter
    {
        internal static IReadOnlyDictionary<string, string> Deserialize(JsonReader reader)
        {
            // ApplicationParameters are represented as array of key value pair.
            // eg: [{"Key": "Key1", "Value": "Value1"}, {"Key": "Key2", "Value": "Value2"}]

            // Read the json array and return it as Dictionary.
            if (reader.TokenType == JsonToken.Null)
            {
                reader.Read();
                return null;
            }

            var parameters = new Dictionary<string, string>();
            reader.ReadStartArray();

            do
            {
                // handle empty array.
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                Tuple<string, string> item = DeserializeFunc(reader);
                parameters.Add(item.Item1, item.Item2);
            }
            while (reader.TokenType != JsonToken.EndArray);

            reader.ReadEndArray();

            return parameters;
        }

        internal static void Serialize(JsonWriter writer, IReadOnlyDictionary<string, string> applicationParameters)
        {
            // ApplicationParameters are represented as array of key value pair.
            // eg: [{"Key": "Key1", "Value": "Value1"}, {"Key": "Key2", "Value": "Value2"}]
            // write the dictionary as json array.
            if (applicationParameters == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartArray();

                foreach (var param in applicationParameters)
                {
                    writer.WriteStartObject();
                    writer.WriteProperty(param.Key, "Key", JsonWriterExtensions.WriteStringValue);
                    writer.WriteProperty(param.Value, "Value", JsonWriterExtensions.WriteStringValue);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
        }

        static Tuple<string, string> DeserializeFunc(JsonReader reader)
        {
            reader.ReadStartObject();

            string key = default;
            string value = default;

            do
            {
                string propName = reader.ReadPropertyName();
                if (string.Compare("Key", propName, StringComparison.Ordinal) == 0)
                    key = reader.ReadValueAsString();
                else if (string.Compare("Value", propName, StringComparison.Ordinal) == 0)
                    value = reader.ReadValueAsString();
                else
                    reader.SkipPropertyValue();
            }
            while (reader.TokenType != JsonToken.EndObject);

            reader.ReadEndObject();
            return new Tuple<string, string>(key, value);
        }
    }
}
