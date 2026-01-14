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
    /// Converter for <see cref="PackageSharingPolicyScope" />.
    /// </summary>
    internal class PackageSharingPolicyScopeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static PackageSharingPolicyScope? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(PackageSharingPolicyScope);

            if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PackageSharingPolicyScope.None;
            }
            else if (string.Compare(value, "All", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PackageSharingPolicyScope.All;
            }
            else if (string.Compare(value, "Code", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PackageSharingPolicyScope.Code;
            }
            else if (string.Compare(value, "Config", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PackageSharingPolicyScope.Config;
            }
            else if (string.Compare(value, "Data", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PackageSharingPolicyScope.Data;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, PackageSharingPolicyScope? value)
        {
            switch (value)
            {
                case PackageSharingPolicyScope.None:
                    writer.WriteStringValue("None");
                    break;
                case PackageSharingPolicyScope.All:
                    writer.WriteStringValue("All");
                    break;
                case PackageSharingPolicyScope.Code:
                    writer.WriteStringValue("Code");
                    break;
                case PackageSharingPolicyScope.Config:
                    writer.WriteStringValue("Config");
                    break;
                case PackageSharingPolicyScope.Data:
                    writer.WriteStringValue("Data");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type PackageSharingPolicyScope");
            }
        }
    }
}
