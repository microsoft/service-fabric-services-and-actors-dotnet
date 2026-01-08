// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Converter for ApplicationParameters
    /// </summary>
    internal class ApplicationParametersConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object. ApplicationParameters are represented as array of key, value pair.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
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
                {
                    break;
                }

                var item = DeserializeFunc(reader);
                parameters.Add(item.Item1, item.Item2);
            }
            while (reader.TokenType != JsonToken.EndArray);

            reader.ReadEndArray();

            return parameters;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="applicationParameters">Application parameters to serialize to JSON.</param>
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

        /// <summary>
        /// Function to deserialize ApplicationParameter from json.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        private static Tuple<string, string> DeserializeFunc(JsonReader reader)
        {
            reader.ReadStartObject();

            var key = default(string);
            var value = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Key", propName, StringComparison.Ordinal) == 0)
                {
                    key = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.Ordinal) == 0)
                {
                    value = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            reader.ReadEndObject();
            return new Tuple<string, string>(key, value);
        }
    }
}
