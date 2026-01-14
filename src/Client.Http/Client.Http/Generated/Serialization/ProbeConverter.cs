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
    /// Converter for <see cref="Probe" />.
    /// </summary>
    internal class ProbeConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static Probe Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static Probe GetFromJsonProperties(JsonReader reader)
        {
            var initialDelaySeconds = default(int?);
            var periodSeconds = default(int?);
            var timeoutSeconds = default(int?);
            var successThreshold = default(int?);
            var failureThreshold = default(int?);
            var exec = default(ProbeExec);
            var httpGet = default(ProbeHttpGet);
            var tcpSocket = default(ProbeTcpSocket);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("initialDelaySeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    initialDelaySeconds = reader.ReadValueAsInt();
                }
                else if (string.Compare("periodSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    periodSeconds = reader.ReadValueAsInt();
                }
                else if (string.Compare("timeoutSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeoutSeconds = reader.ReadValueAsInt();
                }
                else if (string.Compare("successThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    successThreshold = reader.ReadValueAsInt();
                }
                else if (string.Compare("failureThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureThreshold = reader.ReadValueAsInt();
                }
                else if (string.Compare("exec", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exec = ProbeExecConverter.Deserialize(reader);
                }
                else if (string.Compare("httpGet", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    httpGet = ProbeHttpGetConverter.Deserialize(reader);
                }
                else if (string.Compare("tcpSocket", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tcpSocket = ProbeTcpSocketConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new Probe(
                initialDelaySeconds: initialDelaySeconds,
                periodSeconds: periodSeconds,
                timeoutSeconds: timeoutSeconds,
                successThreshold: successThreshold,
                failureThreshold: failureThreshold,
                exec: exec,
                httpGet: httpGet,
                tcpSocket: tcpSocket);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, Probe obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.InitialDelaySeconds != null)
            {
                writer.WriteProperty(obj.InitialDelaySeconds, "initialDelaySeconds", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.PeriodSeconds != null)
            {
                writer.WriteProperty(obj.PeriodSeconds, "periodSeconds", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.TimeoutSeconds != null)
            {
                writer.WriteProperty(obj.TimeoutSeconds, "timeoutSeconds", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.SuccessThreshold != null)
            {
                writer.WriteProperty(obj.SuccessThreshold, "successThreshold", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.FailureThreshold != null)
            {
                writer.WriteProperty(obj.FailureThreshold, "failureThreshold", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.Exec != null)
            {
                writer.WriteProperty(obj.Exec, "exec", ProbeExecConverter.Serialize);
            }

            if (obj.HttpGet != null)
            {
                writer.WriteProperty(obj.HttpGet, "httpGet", ProbeHttpGetConverter.Serialize);
            }

            if (obj.TcpSocket != null)
            {
                writer.WriteProperty(obj.TcpSocket, "tcpSocket", ProbeTcpSocketConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
