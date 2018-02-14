// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace FabActUtil
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class ToolContext
    {
        public ToolContext()
        {
            this.ActorTypes = new List<ActorTypeInformation>();
        }

        public ToolArguments Arguments { get; set; }

        public Assembly InputAssembly { get; set; }

        public IList<ActorTypeInformation> ActorTypes { get; set; }
    }
}
