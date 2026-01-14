// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Container logs.
    /// </summary>
    public partial class ContainerLogs
    {
        /// <summary>
        /// Initializes a new instance of the ContainerLogs class.
        /// </summary>
        /// <param name="content">Container logs.</param>
        public ContainerLogs(
            string content = default(string))
        {
            this.Content = content;
        }

        /// <summary>
        /// Gets container logs.
        /// </summary>
        public string Content { get; }
    }
}
