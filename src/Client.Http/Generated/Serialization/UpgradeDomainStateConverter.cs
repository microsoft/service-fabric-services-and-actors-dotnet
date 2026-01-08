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
    /// Converter for <see cref="UpgradeDomainState" />.
    /// </summary>
    internal class UpgradeDomainStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static UpgradeDomainState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(UpgradeDomainState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeDomainState.Invalid;
            }
            else if (string.Compare(value, "Pending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeDomainState.Pending;
            }
            else if (string.Compare(value, "InProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeDomainState.InProgress;
            }
            else if (string.Compare(value, "Completed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeDomainState.Completed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, UpgradeDomainState? value)
        {
            switch (value)
            {
                case UpgradeDomainState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case UpgradeDomainState.Pending:
                    writer.WriteStringValue("Pending");
                    break;
                case UpgradeDomainState.InProgress:
                    writer.WriteStringValue("InProgress");
                    break;
                case UpgradeDomainState.Completed:
                    writer.WriteStringValue("Completed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type UpgradeDomainState");
            }
        }
    }
}
