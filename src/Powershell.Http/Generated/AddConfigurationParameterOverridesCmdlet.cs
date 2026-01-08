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
    /// Adds the list of configuration overrides on the specified node.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "SFConfigurationParameterOverrides")]
    public partial class AddConfigurationParameterOverridesCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ConfigParameterOverrideList. Description for adding list of configuration overrides.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public IEnumerable<ConfigParameterOverride> ConfigParameterOverrideList { get; set; }

        /// <summary>
        /// Gets or sets Force. Force adding configuration overrides on specified nodes.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? Force { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            this.ServiceFabricClient.Nodes.AddConfigurationParameterOverridesAsync(
                nodeName: this.NodeName,
                configParameterOverrideList: this.ConfigParameterOverrideList,
                force: this.Force,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
