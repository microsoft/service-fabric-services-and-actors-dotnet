// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Exec command to run inside the container.
    /// </summary>
    public partial class ProbeExec
    {
        /// <summary>
        /// Initializes a new instance of the ProbeExec class.
        /// </summary>
        /// <param name="command">Comma separated command to run inside the container for example "sh, -c, echo hello
        /// world".</param>
        public ProbeExec(
            string command)
        {
            command.ThrowIfNull(nameof(command));
            this.Command = command;
        }

        /// <summary>
        /// Gets comma separated command to run inside the container for example "sh, -c, echo hello world".
        /// </summary>
        public string Command { get; }
    }
}
