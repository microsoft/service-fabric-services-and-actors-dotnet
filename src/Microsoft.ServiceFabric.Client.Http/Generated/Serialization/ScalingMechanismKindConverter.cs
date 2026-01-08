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
    /// Converter for <see cref="ScalingMechanismKind" />.
    /// </summary>
    internal class ScalingMechanismKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ScalingMechanismKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ScalingMechanismKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ScalingMechanismKind.Invalid;
            }
            else if (string.Compare(value, "PartitionInstanceCount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ScalingMechanismKind.PartitionInstanceCount;
            }
            else if (string.Compare(value, "AddRemoveIncrementalNamedPartition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ScalingMechanismKind.AddRemoveIncrementalNamedPartition;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ScalingMechanismKind? value)
        {
            switch (value)
            {
                case ScalingMechanismKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ScalingMechanismKind.PartitionInstanceCount:
                    writer.WriteStringValue("PartitionInstanceCount");
                    break;
                case ScalingMechanismKind.AddRemoveIncrementalNamedPartition:
                    writer.WriteStringValue("AddRemoveIncrementalNamedPartition");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ScalingMechanismKind");
            }
        }
    }
}
