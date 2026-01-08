// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifying this parameter adds support for reliable collections
    /// </summary>
    public partial class ReliableCollectionsRef
    {
        /// <summary>
        /// Initializes a new instance of the ReliableCollectionsRef class.
        /// </summary>
        /// <param name="name">Name of ReliableCollection resource. Right now it's not used and you can use any string.</param>
        /// <param name="doNotPersistState">False (the default) if ReliableCollections state is persisted to disk as usual.
        /// True if you do not want to persist state, in which case replication is still enabled and you can use
        /// ReliableCollections as distributed cache.</param>
        public ReliableCollectionsRef(
            string name,
            bool? doNotPersistState = default(bool?))
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
            this.DoNotPersistState = doNotPersistState;
        }

        /// <summary>
        /// Gets name of ReliableCollection resource. Right now it's not used and you can use any string.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets false (the default) if ReliableCollections state is persisted to disk as usual. True if you do not want to
        /// persist state, in which case replication is still enabled and you can use ReliableCollections as distributed cache.
        /// </summary>
        public bool? DoNotPersistState { get; }
    }
}
