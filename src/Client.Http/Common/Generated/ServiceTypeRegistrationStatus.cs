// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServiceTypeRegistrationStatus.
    /// </summary>
    public enum ServiceTypeRegistrationStatus
    {
        /// <summary>
        /// Indicates the registration status is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the service type is disabled on this node. A type gets disabled when there are too many failures of
        /// the code package hosting the service type. If the service type is disabled, new replicas of that service type will
        /// not be placed on the node until it is enabled again. The service type is enabled again after the process hosting it
        /// comes up and re-registers the type or a preconfigured time interval has passed. The value is 1.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that the service type is enabled on this node. Replicas of this service type can be placed on this node
        /// when the code package registers the service type. The value is 2.
        /// </summary>
        Enabled,

        /// <summary>
        /// Indicates that the service type is enabled and registered on the node by a code package. Replicas of this service
        /// type can now be placed on this node. The value is 3.
        /// </summary>
        Registered,
    }
}
