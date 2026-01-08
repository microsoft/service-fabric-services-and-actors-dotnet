// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Adds the list of tags on the specified node.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "SFNodeTags")]
    public partial class AddNodeTagsCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets NodeTags. Description for adding list of node tags.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public IEnumerable<string> NodeTags { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            this.ServiceFabricClient.Nodes.AddNodeTagsAsync(
                nodeName: this.NodeName,
                nodeTags: this.NodeTags,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
