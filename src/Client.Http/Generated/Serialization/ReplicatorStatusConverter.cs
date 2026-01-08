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
    /// Converter for <see cref="ReplicatorStatus" />.
    /// </summary>
    internal class ReplicatorStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicatorStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicatorStatus GetFromJsonProperties(JsonReader reader)
        {
            ReplicatorStatus obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Primary", StringComparison.OrdinalIgnoreCase))
            {
                obj = PrimaryReplicatorStatusConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ActiveSecondary", StringComparison.OrdinalIgnoreCase))
            {
                obj = SecondaryActiveReplicatorStatusConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("IdleSecondary", StringComparison.OrdinalIgnoreCase))
            {
                obj = SecondaryIdleReplicatorStatusConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, ReplicatorStatus obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(ReplicaRole.Primary))
            {
                PrimaryReplicatorStatusConverter.Serialize(writer, (PrimaryReplicatorStatus)obj);
            }
            else if (kind.Equals(ReplicaRole.ActiveSecondary))
            {
                SecondaryActiveReplicatorStatusConverter.Serialize(writer, (SecondaryActiveReplicatorStatus)obj);
            }
            else if (kind.Equals(ReplicaRole.IdleSecondary))
            {
                SecondaryIdleReplicatorStatusConverter.Serialize(writer, (SecondaryIdleReplicatorStatus)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
