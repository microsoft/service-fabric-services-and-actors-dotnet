// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http
{
    static class JsonWriterExtensions
    {
        internal static void WriteDateTimeValue(this JsonWriter writer, DateTime? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                // write in ISO8601 foramt.
                writer.WriteValue(XmlConvert.ToString(value.Value, XmlDateTimeSerializationMode.Utc));
        }

        internal static void WriteTimeSpanValue(this JsonWriter writer, TimeSpan? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                // write in ISO8601 foramt.
                writer.WriteValue(XmlConvert.ToString(value.Value));
        }

        internal static void WriteStringValue(this JsonWriter writer, string value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }

        internal static void WriteIntValue(this JsonWriter writer, int? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }

        internal static void WriteLongValue(this JsonWriter writer, long? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }

        internal static void WriteDoubleValue(this JsonWriter writer, double? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }

        internal static void WriteBoolValue(this JsonWriter writer, bool? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }

        internal static void WriteGuidValue(this JsonWriter writer, Guid? value)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.ToString());
        }

        internal static void WriteByteValue(this JsonWriter writer, byte value)
        {
            // byte is int in json.
            writer.WriteValue(value);
        }

        internal static void WriteProperty<T>(this JsonWriter writer, T obj, string propertyName, Action<JsonWriter, T> serializeFunc)
        {
            writer.WritePropertyName(propertyName);
            if (obj == null)
                writer.WriteNull();
            else
                serializeFunc.Invoke(writer, obj);
        }

        internal static void WriteEnumerableProperty<T>(this JsonWriter writer, IEnumerable<T> sequence, string propertyName, Action<JsonWriter, T> serializeFunc)
        {
            writer.WritePropertyName(propertyName);

            if (sequence == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();

                foreach (var item in sequence)
                {
                    if (item == null)
                        writer.WriteNull();
                    else
                        serializeFunc.Invoke(writer, item);
                }

                writer.WriteEndArray();
            }
        }

        internal static void WriteDictionaryProperty<T>(this JsonWriter writer, IReadOnlyDictionary<string, T> collection, string propertyName, Action<JsonWriter, T> serializeFunc)
        {
            writer.WritePropertyName(propertyName);
            if (collection == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();

                foreach (var item in collection)
                {
                    writer.WritePropertyName(item.Key);
                    if (item.Value == null)
                        writer.WriteNull();
                    else
                        serializeFunc.Invoke(writer, item.Value);
                }

                writer.WriteEndObject();
            }
        }

        internal static void WriteEnumerableValue<T>(this JsonWriter writer, IEnumerable<T> sequence, Action<JsonWriter, T> serializeFunc)
        {
            if (sequence != null)
            {
                writer.WriteStartArray();

                foreach (T item in sequence)
                {
                    if (item == null)
                        writer.WriteNull();
                    else
                        serializeFunc.Invoke(writer, item);
                }

                writer.WriteEndArray();
            }
        }
    }
}
