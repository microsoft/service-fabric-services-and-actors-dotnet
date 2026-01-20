// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the named partition scheme of the service.
    /// </summary>
    public partial class NamedRepartitionSchemeDescription : RepartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the NamedRepartitionSchemeDescription class.
        /// </summary>
        /// <param name="namesToAdd">Dynamic array for the names of the partitions to add.</param>
        /// <param name="namesToRemove">Dynamic array for the names of the partitions to remove.</param>
        public NamedRepartitionSchemeDescription(
            IEnumerable<string> namesToAdd = default(IEnumerable<string>),
            IEnumerable<string> namesToRemove = default(IEnumerable<string>))
            : base(
                Common.RepartitionScheme.Named)
        {
            this.NamesToAdd = namesToAdd;
            this.NamesToRemove = namesToRemove;
        }

        /// <summary>
        /// Gets dynamic array for the names of the partitions to add.
        /// </summary>
        public IEnumerable<string> NamesToAdd { get; }

        /// <summary>
        /// Gets dynamic array for the names of the partitions to remove.
        /// </summary>
        public IEnumerable<string> NamesToRemove { get; }
    }
}
