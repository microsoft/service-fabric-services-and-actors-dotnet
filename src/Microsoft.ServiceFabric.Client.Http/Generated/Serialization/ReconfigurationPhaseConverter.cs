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
    /// Converter for <see cref="ReconfigurationPhase" />.
    /// </summary>
    internal class ReconfigurationPhaseConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ReconfigurationPhase? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ReconfigurationPhase);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Unknown;
            }
            else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.None;
            }
            else if (string.Compare(value, "Phase0", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Phase0;
            }
            else if (string.Compare(value, "Phase1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Phase1;
            }
            else if (string.Compare(value, "Phase2", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Phase2;
            }
            else if (string.Compare(value, "Phase3", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Phase3;
            }
            else if (string.Compare(value, "Phase4", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.Phase4;
            }
            else if (string.Compare(value, "AbortPhaseZero", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationPhase.AbortPhaseZero;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ReconfigurationPhase? value)
        {
            switch (value)
            {
                case ReconfigurationPhase.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ReconfigurationPhase.None:
                    writer.WriteStringValue("None");
                    break;
                case ReconfigurationPhase.Phase0:
                    writer.WriteStringValue("Phase0");
                    break;
                case ReconfigurationPhase.Phase1:
                    writer.WriteStringValue("Phase1");
                    break;
                case ReconfigurationPhase.Phase2:
                    writer.WriteStringValue("Phase2");
                    break;
                case ReconfigurationPhase.Phase3:
                    writer.WriteStringValue("Phase3");
                    break;
                case ReconfigurationPhase.Phase4:
                    writer.WriteStringValue("Phase4");
                    break;
                case ReconfigurationPhase.AbortPhaseZero:
                    writer.WriteStringValue("AbortPhaseZero");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ReconfigurationPhase");
            }
        }
    }
}
