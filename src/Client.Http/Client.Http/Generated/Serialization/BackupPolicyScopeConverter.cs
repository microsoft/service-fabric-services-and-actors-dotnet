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
    /// Converter for <see cref="BackupPolicyScope" />.
    /// </summary>
    internal class BackupPolicyScopeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupPolicyScope? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupPolicyScope);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupPolicyScope.Invalid;
            }
            else if (string.Compare(value, "Partition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupPolicyScope.Partition;
            }
            else if (string.Compare(value, "Service", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupPolicyScope.Service;
            }
            else if (string.Compare(value, "Application", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupPolicyScope.Application;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupPolicyScope? value)
        {
            switch (value)
            {
                case BackupPolicyScope.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupPolicyScope.Partition:
                    writer.WriteStringValue("Partition");
                    break;
                case BackupPolicyScope.Service:
                    writer.WriteStringValue("Service");
                    break;
                case BackupPolicyScope.Application:
                    writer.WriteStringValue("Application");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupPolicyScope");
            }
        }
    }
}
