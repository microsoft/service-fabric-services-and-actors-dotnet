// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the type of registration or provision requested, and if the operation needs to be asynchronous or not.
    /// Supported types of provision operations are from either image store or external store.
    /// </summary>
    public abstract partial class ProvisionApplicationTypeDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the ProvisionApplicationTypeDescriptionBase class.
        /// </summary>
        /// <param name="kind">The kind of application type registration or provision requested. The application package can be
        /// registered or provisioned either from the image store or from an external store. Following are the kinds of the
        /// application type provision.</param>
        /// <param name="async">Indicates whether or not provisioning should occur asynchronously. When set to true, the
        /// provision operation returns when the request is accepted by the system, and the provision operation continues
        /// without any timeout limit. The default value is false. For large application packages, we recommend setting the
        /// value to true.</param>
        protected ProvisionApplicationTypeDescriptionBase(
            ProvisionApplicationTypeKind? kind,
            bool? async = false)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.Async = async;
        }

        /// <summary>
        /// Gets indicates whether or not provisioning should occur asynchronously. When set to true, the provision operation
        /// returns when the request is accepted by the system, and the provision operation continues without any timeout
        /// limit. The default value is false. For large application packages, we recommend setting the value to true.
        /// </summary>
        public bool? Async { get; }

        /// <summary>
        /// Gets the kind of application type registration or provision requested. The application package can be registered or
        /// provisioned either from the image store or from an external store. Following are the kinds of the application type
        /// provision.
        /// </summary>
        public ProvisionApplicationTypeKind? Kind { get; }
    }
}
