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
    /// Converter for <see cref="ExecutionPolicy" />.
    /// </summary>
    internal class ExecutionPolicyConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ExecutionPolicy Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ExecutionPolicy GetFromJsonProperties(JsonReader reader)
        {
            ExecutionPolicy obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("type", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is type.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Default", StringComparison.OrdinalIgnoreCase))
            {
                obj = DefaultExecutionPolicyConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("RunToCompletion", StringComparison.OrdinalIgnoreCase))
            {
                obj = RunToCompletionExecutionPolicyConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown type.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ExecutionPolicy obj)
        {
            var kind = obj.Type;
            if (kind.Equals(ExecutionPolicyType.Default))
            {
                DefaultExecutionPolicyConverter.Serialize(writer, (DefaultExecutionPolicy)obj);
            }
            else if (kind.Equals(ExecutionPolicyType.RunToCompletion))
            {
                RunToCompletionExecutionPolicyConverter.Serialize(writer, (RunToCompletionExecutionPolicy)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown type.");
            }
        }
    }
}
