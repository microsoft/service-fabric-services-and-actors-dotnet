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
    /// Converter for <see cref="ServiceHostUpgradeImpact" />.
    /// </summary>
    internal class ServiceHostUpgradeImpactConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceHostUpgradeImpact? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceHostUpgradeImpact);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceHostUpgradeImpact.Invalid;
            }
            else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceHostUpgradeImpact.None;
            }
            else if (string.Compare(value, "ServiceHostRestart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceHostUpgradeImpact.ServiceHostRestart;
            }
            else if (string.Compare(value, "UnexpectedServiceHostRestart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceHostUpgradeImpact.UnexpectedServiceHostRestart;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceHostUpgradeImpact? value)
        {
            switch (value)
            {
                case ServiceHostUpgradeImpact.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServiceHostUpgradeImpact.None:
                    writer.WriteStringValue("None");
                    break;
                case ServiceHostUpgradeImpact.ServiceHostRestart:
                    writer.WriteStringValue("ServiceHostRestart");
                    break;
                case ServiceHostUpgradeImpact.UnexpectedServiceHostRestart:
                    writer.WriteStringValue("UnexpectedServiceHostRestart");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceHostUpgradeImpact");
            }
        }
    }
}
