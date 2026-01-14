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
    /// Converter for <see cref="ServicePlacementPolicyType" />.
    /// </summary>
    internal class ServicePlacementPolicyTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServicePlacementPolicyType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServicePlacementPolicyType);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.Invalid;
            }
            else if (string.Compare(value, "InvalidDomain", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.InvalidDomain;
            }
            else if (string.Compare(value, "RequireDomain", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.RequireDomain;
            }
            else if (string.Compare(value, "PreferPrimaryDomain", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.PreferPrimaryDomain;
            }
            else if (string.Compare(value, "RequireDomainDistribution", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.RequireDomainDistribution;
            }
            else if (string.Compare(value, "NonPartiallyPlaceService", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.NonPartiallyPlaceService;
            }
            else if (string.Compare(value, "AllowMultipleStatelessInstancesOnNode", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePlacementPolicyType.AllowMultipleStatelessInstancesOnNode;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServicePlacementPolicyType? value)
        {
            switch (value)
            {
                case ServicePlacementPolicyType.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServicePlacementPolicyType.InvalidDomain:
                    writer.WriteStringValue("InvalidDomain");
                    break;
                case ServicePlacementPolicyType.RequireDomain:
                    writer.WriteStringValue("RequireDomain");
                    break;
                case ServicePlacementPolicyType.PreferPrimaryDomain:
                    writer.WriteStringValue("PreferPrimaryDomain");
                    break;
                case ServicePlacementPolicyType.RequireDomainDistribution:
                    writer.WriteStringValue("RequireDomainDistribution");
                    break;
                case ServicePlacementPolicyType.NonPartiallyPlaceService:
                    writer.WriteStringValue("NonPartiallyPlaceService");
                    break;
                case ServicePlacementPolicyType.AllowMultipleStatelessInstancesOnNode:
                    writer.WriteStringValue("AllowMultipleStatelessInstancesOnNode");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServicePlacementPolicyType");
            }
        }
    }
}
