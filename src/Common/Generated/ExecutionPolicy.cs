// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The execution policy of the service
    /// </summary>
    public abstract partial class ExecutionPolicy
    {
        /// <summary>
        /// Initializes a new instance of the ExecutionPolicy class.
        /// </summary>
        /// <param name="type">Enumerates the execution policy types for services.</param>
        protected ExecutionPolicy(
            ExecutionPolicyType? type)
        {
            type.ThrowIfNull(nameof(type));
            this.Type = type;
        }

        /// <summary>
        /// Gets enumerates the execution policy types for services.
        /// </summary>
        public ExecutionPolicyType? Type { get; }
    }
}
