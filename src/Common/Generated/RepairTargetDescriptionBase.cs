// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the entities targeted by a repair action.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public abstract partial class RepairTargetDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the RepairTargetDescriptionBase class.
        /// </summary>
        /// <param name="kind">Specifies the kind of the repair target. This type supports the Service Fabric platform; it is
        /// not meant to be used directly from your code.'</param>
        protected RepairTargetDescriptionBase(
            RepairTargetKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets specifies the kind of the repair target. This type supports the Service Fabric platform; it is not meant to be
        /// used directly from your code.'
        /// </summary>
        public RepairTargetKind? Kind { get; }
    }
}
