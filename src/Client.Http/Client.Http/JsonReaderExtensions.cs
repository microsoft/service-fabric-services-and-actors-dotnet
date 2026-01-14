// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http
{
    static class JsonReaderExtensions
    {
        internal static void MoveToContent(this JsonReader reader)
        {
            while ((reader.TokenType == JsonToken.Comment || reader.TokenType == JsonToken.None) && reader.Read())
            {
            }
        }

        internal static void ReadStartObject(this JsonReader reader)
        {
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonReaderException($"Unexpected JsonToken {reader.TokenType}.");
            reader.Read();
        }

        internal static void ReadEndObject(this JsonReader reader)
        {
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonReaderException($"Unexpected JsonToken {reader.TokenType}.");
            reader.Read();
        }

        internal static void ReadStartArray(this JsonReader reader)
        {
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonReaderException($"Unexpected JsonToken {reader.TokenType}.");
            reader.Read();
        }

        internal static void ReadEndArray(this JsonReader reader)
        {
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.EndArray)
                throw new JsonReaderException($"Unexpected JsonToken {reader.TokenType}.");
            reader.Read();
        }

        internal static string ReadPropertyName(this JsonReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName)
                throw new JsonReaderException($"Error reading Property NameDescription from Json, unexpected JsonToken {reader.TokenType}.");
            string propName = reader.Value?.ToString();
            reader.Read();
            return propName;
        }

        internal static string ReadValueAsString(this JsonReader reader)
        {
            string value = null;
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    value = (string)reader.Value;
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                case JsonToken.Date:
                    var formattable = reader.Value as IFormattable;
                    if (formattable != null)
                        value = formattable.ToString();
                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;
                default:
                    throw new JsonReaderException($"Error reading string. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static bool? ReadValueAsBool(this JsonReader reader)
        {
            bool? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    value = Convert.ToBoolean(reader.Value);
                    break;
                case JsonToken.String:
                    value = ParseAsType((string)reader.Value, bool.Parse);
                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;
                default:
                    throw new JsonReaderException($"Error reading boolean. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static int? ReadValueAsInt(this JsonReader reader)
        {
            int? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    value = Convert.ToInt32(reader.Value);
                    break;

                case JsonToken.String:
                    value = ParseAsTypeWithInvariantCulture((string)reader.Value, int.Parse);
                    break;

                case JsonToken.Null:
                    // value is initialized to null
                    break;

                default:
                    throw new JsonReaderException($"Error reading integer. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static byte ReadValueAsByte(this JsonReader reader)
        {
            // byte is int in json.
            int value;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    value = Convert.ToInt32(reader.Value);
                    break;

                case JsonToken.String:
                    value = ParseAsTypeWithInvariantCulture((string)reader.Value, int.Parse);
                    break;

                default:
                    throw new JsonReaderException($"Error reading integer. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return (byte)value;
        }

        internal static long? ReadValueAsLong(this JsonReader reader)
        {
            long? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    value = Convert.ToInt64(reader.Value);
                    break;
                case JsonToken.String:
                    value = ParseAsTypeWithInvariantCulture((string)reader.Value, long.Parse);
                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;
                default:
                    throw new JsonReaderException($"Error reading integer. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static double? ReadValueAsDouble(this JsonReader reader)
        {
            double? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.Float:
                    value = Convert.ToDouble(reader.Value);
                    break;
                case JsonToken.String:
                    value = ParseAsTypeWithInvariantCulture((string)reader.Value, double.Parse);
                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;
                default:
                    throw new JsonReaderException($"Error reading integer. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static DateTime? ReadValueAsDateTime(this JsonReader reader)
        {
            // DateTime is a string in ISO8601 format
            DateTime? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.Date:
                    value = (DateTime)reader.Value;
                    break;
                case JsonToken.String:
                    var valueString = (string)reader.Value;
                    try
                    {
                        value = XmlConvert.ToDateTime(valueString, XmlDateTimeSerializationMode.Utc);
                    }
                    catch (Exception)
                    {
                        // TODO: try parsing with DateTime.Parse, Remove it once all apis return in ISO8601 format.
                        try
                        {
                            value = DateTime.Parse(valueString);
                        }
                        catch (Exception ex)
                        {
                            throw new JsonReaderException(
                                $"Error converting string to System.DateTime, string value to be converted is {valueString}.  DateTime values must be specified in string as per ISO8601",
                                ex);
                        }
                    }

                    break;

                case JsonToken.Null:
                    // value is initialized to null
                    break;

                default:
                    throw new JsonReaderException($"Error reading Date. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static TimeSpan? ReadValueAsTimeSpan(this JsonReader reader)
        {
            // TimeSpan is a string in ISO8601 format
            TimeSpan? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    var valueString = (string)reader.Value;
                    try
                    {
                        value = XmlConvert.ToTimeSpan(valueString);
                    }
                    catch (Exception)
                    {
                        // TODO: try parsing with DateTime.Parse, Remove it once all apis return in ISO8601 format.
                        try
                        {
                            value = TimeSpan.Parse(valueString);
                        }
                        catch (Exception ex)
                        {
                            throw new JsonReaderException(
                            $"Error converting string to System.TimeSpan, string value to be converted is {valueString}. Timespan values must be specified in string as per ISO8601.",
                            ex);
                        }
                    }

                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;

                default:
                    throw new JsonReaderException($"Error reading TimeSpan. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static Guid? ReadValueAsGuid(this JsonReader reader)
        {
            Guid? value = null;

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    value = ParseAsType((string)reader.Value, Guid.Parse);
                    break;
                case JsonToken.Null:
                    // value is initialized to null
                    break;
                default:
                    throw new JsonReaderException($"Error reading Date. Unexpected token: {reader.TokenType}.");
            }

            reader.Read();
            return value;
        }

        internal static void SkipPropertyValue(this JsonReader reader)
        {
            if (reader.TokenType.Equals(JsonToken.StartObject) || reader.TokenType.Equals(JsonToken.StartArray))
                reader.Skip();
            reader.Read();
        }

        internal static List<T> ReadList<T>(this JsonReader reader, Func<JsonReader, T> deserializerFunc)
        {
            // handle null.
            if (reader.TokenType == JsonToken.Null)
            {
                reader.Read();
                return null;
            }

            var value = new List<T>();
            reader.ReadStartArray();

            do
            {
                // handle empty array.
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                T item = deserializerFunc(reader);
                value.Add(item);
            }
            while (reader.TokenType != JsonToken.EndArray);

            reader.ReadEndArray();
            return value;
        }

        internal static Dictionary<string, T> ReadDictionary<T>(this JsonReader reader, Func<JsonReader, T> deserializerFunc)
        {
            // handle null.
            if (reader.TokenType == JsonToken.Null)
            {
                reader.Read();
                return null;
            }

            var dict = new Dictionary<string, T>();
            reader.ReadStartObject();

            do
            {
                // handle empty dictionary.
                if (reader.TokenType == JsonToken.EndObject)
                    break;

                // key is propertyName, read property value and move to next token.
                string key = reader.ReadPropertyName();
                T value = deserializerFunc(reader);
                dict.Add(key, value);
            }
            while (reader.TokenType != JsonToken.EndObject);

            reader.ReadEndObject();
            return dict;
        }

        internal static T Deserialize<T>(this JsonReader reader, Func<JsonReader, T> getFromJsonPropertiesFunc)
        {
            T obj = default;

            // handle null.
            if (reader.TokenType.Equals(JsonToken.Null))
            {
                reader.Read();
                return obj;
            }

            // Handle JsonReader created over stream of length 0.
            reader.MoveToContent();
            if (reader.TokenType.Equals(JsonToken.None))
                return obj;

            // handle Empty Json.
            reader.ReadStartObject();
            if (reader.TokenType.Equals(JsonToken.EndObject))
            {
                reader.ReadEndObject();
                return obj;
            }

            // not empty json, get value by reading properties.
            obj = getFromJsonPropertiesFunc.Invoke(reader);
            reader.ReadEndObject();
            return obj;
        }

        static T ParseAsType<T>(string value, Func<string, T> parseFunc)
        {
            T result;

            try
            {
                result = parseFunc(value);
            }
            catch (Exception ex)
            {
                throw new JsonReaderException($"Error converting string to {typeof(T)}, string value to be converted is {value}", ex);
            }

            return result;
        }

        static T ParseAsTypeWithInvariantCulture<T>(string value, Func<string, CultureInfo, T> parseFunc)
        {
            T result;

            try
            {
                result = parseFunc(value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new JsonReaderException($"Error converting string to {typeof(T)}, string value to be converted is {value}", ex);
            }

            return result;
        }
    }
}
