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
    /// Converter for <see cref="ServicePlacementPolicyDescription" />.
    /// </summary>
    internal class ServicePlacementPolicyDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServicePlacementPolicyDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServicePlacementPolicyDescription GetFromJsonProperties(JsonReader reader)
        {
            ServicePlacementPolicyDescription obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Type", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Type.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("InvalidDomain", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementInvalidDomainPolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NonPartiallyPlaceService", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementNonPartiallyPlaceServicePolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("AllowMultipleStatelessInstancesOnNode", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PreferPrimaryDomain", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementPreferPrimaryDomainPolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("RequireDomain", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementRequiredDomainPolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("RequireDomainDistribution", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicePlacementRequireDomainDistributionPolicyDescriptionConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown Type.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServicePlacementPolicyDescription obj)
        {
            var kind = obj.Type;
            if (kind.Equals(ServicePlacementPolicyType.InvalidDomain))
            {
                ServicePlacementInvalidDomainPolicyDescriptionConverter.Serialize(writer, (ServicePlacementInvalidDomainPolicyDescription)obj);
            }
            else if (kind.Equals(ServicePlacementPolicyType.NonPartiallyPlaceService))
            {
                ServicePlacementNonPartiallyPlaceServicePolicyDescriptionConverter.Serialize(writer, (ServicePlacementNonPartiallyPlaceServicePolicyDescription)obj);
            }
            else if (kind.Equals(ServicePlacementPolicyType.AllowMultipleStatelessInstancesOnNode))
            {
                ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescriptionConverter.Serialize(writer, (ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescription)obj);
            }
            else if (kind.Equals(ServicePlacementPolicyType.PreferPrimaryDomain))
            {
                ServicePlacementPreferPrimaryDomainPolicyDescriptionConverter.Serialize(writer, (ServicePlacementPreferPrimaryDomainPolicyDescription)obj);
            }
            else if (kind.Equals(ServicePlacementPolicyType.RequireDomain))
            {
                ServicePlacementRequiredDomainPolicyDescriptionConverter.Serialize(writer, (ServicePlacementRequiredDomainPolicyDescription)obj);
            }
            else if (kind.Equals(ServicePlacementPolicyType.RequireDomainDistribution))
            {
                ServicePlacementRequireDomainDistributionPolicyDescriptionConverter.Serialize(writer, (ServicePlacementRequireDomainDistributionPolicyDescription)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Type.");
            }
        }
    }
}
