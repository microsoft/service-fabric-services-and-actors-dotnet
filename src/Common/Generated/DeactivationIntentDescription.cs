// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the intent or reason for deactivating the node. DeactivationDescription is an optional field.
    /// </summary>
    public partial class DeactivationIntentDescription
    {
        /// <summary>
        /// Initializes a new instance of the DeactivationIntentDescription class.
        /// </summary>
        /// <param name="deactivationIntent">Describes the intent or reason for deactivating the node. The possible values are
        /// following.
        /// . Possible values include: 'Pause', 'Restart', 'RemoveData', 'RemoveNode'</param>
        /// <param name="deactivationDescription">Describes the reason or more information about node deactivation. Maximum 
        /// number of characters allowed is 200.</param>
        public DeactivationIntentDescription(
            DeactivationIntent? deactivationIntent = default(DeactivationIntent?),
            string deactivationDescription = default(string))
        {
            this.DeactivationIntent = deactivationIntent;
            this.DeactivationDescription = deactivationDescription;
        }

        /// <summary>
        /// Gets the intent or reason for deactivating the node. The possible values are following.
        /// . Possible values include: 'Pause', 'Restart', 'RemoveData', 'RemoveNode'
        /// </summary>
        public DeactivationIntent? DeactivationIntent { get; }

        /// <summary>
        /// Gets the reason or more information about node deactivation. Maximum  number of characters allowed is 200.
        /// </summary>
        public string DeactivationDescription { get; }
    }
}
