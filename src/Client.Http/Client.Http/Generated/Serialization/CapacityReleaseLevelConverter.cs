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
    /// Converter for <see cref="CapacityReleaseLevel" />.
    /// </summary>
    internal class CapacityReleaseLevelConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static CapacityReleaseLevel? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(CapacityReleaseLevel);

            if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseLevel.None;
            }
            else if (string.Compare(value, "Minor", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseLevel.Minor;
            }
            else if (string.Compare(value, "Major", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseLevel.Major;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, CapacityReleaseLevel? value)
        {
            switch (value)
            {
                case CapacityReleaseLevel.None:
                    writer.WriteStringValue("None");
                    break;
                case CapacityReleaseLevel.Minor:
                    writer.WriteStringValue("Minor");
                    break;
                case CapacityReleaseLevel.Major:
                    writer.WriteStringValue("Major");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type CapacityReleaseLevel");
            }
        }
    }
}
