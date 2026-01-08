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
    /// Converter for <see cref="ChaosEvent" />.
    /// </summary>
    internal class ChaosEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosEvent GetFromJsonProperties(JsonReader reader)
        {
            ChaosEvent obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("ExecutingFaults", StringComparison.OrdinalIgnoreCase))
            {
                obj = ExecutingFaultsChaosEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Started", StringComparison.OrdinalIgnoreCase))
            {
                obj = StartedChaosEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Stopped", StringComparison.OrdinalIgnoreCase))
            {
                obj = StoppedChaosEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("TestError", StringComparison.OrdinalIgnoreCase))
            {
                obj = TestErrorChaosEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ValidationFailed", StringComparison.OrdinalIgnoreCase))
            {
                obj = ValidationFailedChaosEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Waiting", StringComparison.OrdinalIgnoreCase))
            {
                obj = WaitingChaosEventConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, ChaosEvent obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(ChaosEventKind.ExecutingFaults))
            {
                ExecutingFaultsChaosEventConverter.Serialize(writer, (ExecutingFaultsChaosEvent)obj);
            }
            else if (kind.Equals(ChaosEventKind.Started))
            {
                StartedChaosEventConverter.Serialize(writer, (StartedChaosEvent)obj);
            }
            else if (kind.Equals(ChaosEventKind.Stopped))
            {
                StoppedChaosEventConverter.Serialize(writer, (StoppedChaosEvent)obj);
            }
            else if (kind.Equals(ChaosEventKind.TestError))
            {
                TestErrorChaosEventConverter.Serialize(writer, (TestErrorChaosEvent)obj);
            }
            else if (kind.Equals(ChaosEventKind.ValidationFailed))
            {
                ValidationFailedChaosEventConverter.Serialize(writer, (ValidationFailedChaosEvent)obj);
            }
            else if (kind.Equals(ChaosEventKind.Waiting))
            {
                WaitingChaosEventConverter.Serialize(writer, (WaitingChaosEvent)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
