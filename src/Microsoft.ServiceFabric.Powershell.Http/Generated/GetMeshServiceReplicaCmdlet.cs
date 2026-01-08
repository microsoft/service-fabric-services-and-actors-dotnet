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
    /// Lists all the replicas of a service.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFMeshServiceReplica", DefaultParameterSetName = "List")]
    public partial class GetMeshServiceReplicaCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationResourceName. The identity of the application.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "List")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "Get")]
        public string ApplicationResourceName { get; set; }

        /// <summary>
        /// Gets or sets ServiceResourceName. The identity of the service.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "List")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "Get")]
        public string ServiceResourceName { get; set; }

        /// <summary>
        /// Gets or sets ReplicaName. Service Fabric replica name.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "Get")]
        public string ReplicaName { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("List"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.MeshServiceReplicas.ListAsync(
                        applicationResourceName: this.ApplicationResourceName,
                        serviceResourceName: this.ServiceResourceName,
                        cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                    if (result == null)
                    {
                        break;
                    }

                    var count = 0;
                    foreach (var item in result.Data)
                    {
                        count++;
                        this.WriteObject(this.FormatOutput(item));
                    }

                    continuationToken = result.ContinuationToken;
                    this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
                }
                while (continuationToken.Next);
            }
            else if (this.ParameterSetName.Equals("Get"))
            {
                var result = this.ServiceFabricClient.MeshServiceReplicas.GetAsync(
                    applicationResourceName: this.ApplicationResourceName,
                    serviceResourceName: this.ServiceResourceName,
                    replicaName: this.ReplicaName,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            var outputResult = output as ServiceReplicaDescription;

            var result = new PSObject();

            result.Properties.Add(new PSNoteProperty("ReplicaName", outputResult.ReplicaName));
            result.Properties.Add(new PSNoteProperty("OperatingSystem", outputResult.OsType));

            return result;
        }
    }
}
