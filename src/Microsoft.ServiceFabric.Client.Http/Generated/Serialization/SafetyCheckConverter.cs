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
    /// Converter for <see cref="SafetyCheck" />.
    /// </summary>
    internal class SafetyCheckConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static SafetyCheck Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static SafetyCheck GetFromJsonProperties(JsonReader reader)
        {
            SafetyCheck obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("EnsureAvailability", StringComparison.OrdinalIgnoreCase))
            {
                obj = EnsureAvailabilitySafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("EnsurePartitionQuorum", StringComparison.OrdinalIgnoreCase))
            {
                obj = EnsurePartitionQuorumSafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("EnsureSeedNodeQuorum", StringComparison.OrdinalIgnoreCase))
            {
                obj = SeedNodeSafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("WaitForInbuildReplica", StringComparison.OrdinalIgnoreCase))
            {
                obj = WaitForInbuildReplicaSafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("WaitForPrimaryPlacement", StringComparison.OrdinalIgnoreCase))
            {
                obj = WaitForPrimaryPlacementSafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("WaitForPrimarySwap", StringComparison.OrdinalIgnoreCase))
            {
                obj = WaitForPrimarySwapSafetyCheckConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("WaitForReconfiguration", StringComparison.OrdinalIgnoreCase))
            {
                obj = WaitForReconfigurationSafetyCheckConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, SafetyCheck obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(SafetyCheckKind.EnsureAvailability))
            {
                EnsureAvailabilitySafetyCheckConverter.Serialize(writer, (EnsureAvailabilitySafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.EnsurePartitionQuorum))
            {
                EnsurePartitionQuorumSafetyCheckConverter.Serialize(writer, (EnsurePartitionQuorumSafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.EnsureSeedNodeQuorum))
            {
                SeedNodeSafetyCheckConverter.Serialize(writer, (SeedNodeSafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.WaitForInbuildReplica))
            {
                WaitForInbuildReplicaSafetyCheckConverter.Serialize(writer, (WaitForInbuildReplicaSafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.WaitForPrimaryPlacement))
            {
                WaitForPrimaryPlacementSafetyCheckConverter.Serialize(writer, (WaitForPrimaryPlacementSafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.WaitForPrimarySwap))
            {
                WaitForPrimarySwapSafetyCheckConverter.Serialize(writer, (WaitForPrimarySwapSafetyCheck)obj);
            }
            else if (kind.Equals(SafetyCheckKind.WaitForReconfiguration))
            {
                WaitForReconfigurationSafetyCheckConverter.Serialize(writer, (WaitForReconfigurationSafetyCheck)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
