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
    /// Submits a property batch.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Submit, "SFPropertyBatch")]
    public partial class SubmitPropertyBatchCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NameId. The Service Fabric name, without the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string NameId { get; set; }

        /// <summary>
        /// Gets or sets Operations. A list of the property batch operations to be executed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public IEnumerable<PropertyBatchOperation> Operations { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var propertyBatchDescriptionList = new PropertyBatchDescriptionList(
            operations: this.Operations);

            var result = this.ServiceFabricClient.Properties.SubmitPropertyBatchAsync(
                nameId: this.NameId,
                propertyBatchDescriptionList: propertyBatchDescriptionList,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
