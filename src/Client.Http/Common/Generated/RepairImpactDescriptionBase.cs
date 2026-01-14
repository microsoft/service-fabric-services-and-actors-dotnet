// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the expected impact of executing a repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public abstract partial class RepairImpactDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the RepairImpactDescriptionBase class.
        /// </summary>
        /// <param name="kind">Specifies the kind of the impact. This type supports the Service Fabric platform; it is not
        /// meant to be used directly from your code.'</param>
        protected RepairImpactDescriptionBase(
            RepairImpactKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets specifies the kind of the impact. This type supports the Service Fabric platform; it is not meant to be used
        /// directly from your code.'
        /// </summary>
        public RepairImpactKind? Kind { get; }
    }
}
