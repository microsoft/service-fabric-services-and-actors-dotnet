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
    /// Converter for <see cref="ApplicationStatus" />.
    /// </summary>
    internal class ApplicationStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ApplicationStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ApplicationStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Invalid;
            }
            else if (string.Compare(value, "Ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Ready;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Upgrading;
            }
            else if (string.Compare(value, "Creating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Creating;
            }
            else if (string.Compare(value, "Deleting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Deleting;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationStatus.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ApplicationStatus? value)
        {
            switch (value)
            {
                case ApplicationStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ApplicationStatus.Ready:
                    writer.WriteStringValue("Ready");
                    break;
                case ApplicationStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case ApplicationStatus.Creating:
                    writer.WriteStringValue("Creating");
                    break;
                case ApplicationStatus.Deleting:
                    writer.WriteStringValue("Deleting");
                    break;
                case ApplicationStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ApplicationStatus");
            }
        }
    }
}
