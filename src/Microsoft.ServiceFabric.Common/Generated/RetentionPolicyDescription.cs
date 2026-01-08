// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the retention policy configured.
    /// </summary>
    public abstract partial class RetentionPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the RetentionPolicyDescription class.
        /// </summary>
        /// <param name="retentionPolicyType">The type of retention policy. Currently only "Basic" retention policy is
        /// supported.
        /// </param>
        protected RetentionPolicyDescription(
            RetentionPolicyType? retentionPolicyType)
        {
            retentionPolicyType.ThrowIfNull(nameof(retentionPolicyType));
            this.RetentionPolicyType = retentionPolicyType;
        }

        /// <summary>
        /// Gets the type of retention policy. Currently only "Basic" retention policy is supported.
        /// </summary>
        public RetentionPolicyType? RetentionPolicyType { get; }
    }
}
