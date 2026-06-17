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
    /// Converter for <see cref="ImpactApprovalStatus" />.
    /// </summary>
    internal class ImpactApprovalStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ImpactApprovalStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ImpactApprovalStatus);

            if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactApprovalStatus.None;
            }
            else if (string.Compare(value, "Nominal", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactApprovalStatus.Nominal;
            }
            else if (string.Compare(value, "WaitingForApproval", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactApprovalStatus.WaitingForApproval;
            }
            else if (string.Compare(value, "Approved", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactApprovalStatus.Approved;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ImpactApprovalStatus? value)
        {
            switch (value)
            {
                case ImpactApprovalStatus.None:
                    writer.WriteStringValue("None");
                    break;
                case ImpactApprovalStatus.Nominal:
                    writer.WriteStringValue("Nominal");
                    break;
                case ImpactApprovalStatus.WaitingForApproval:
                    writer.WriteStringValue("WaitingForApproval");
                    break;
                case ImpactApprovalStatus.Approved:
                    writer.WriteStringValue("Approved");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ImpactApprovalStatus");
            }
        }
    }
}
