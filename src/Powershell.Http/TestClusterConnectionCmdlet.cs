// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Client;

    /// <summary>
    /// Cmdlet to test connection to Service Fabric cluster. 
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "SFClusterConnection")]
    public class TestClusterConnectionCmdlet : CommonCmdletBase
    {
        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            var client = (IServiceFabricClient)this.SessionState.PSVariable.GetValue(Constants.ClusterConnectionVariableName);
            this.WriteObject(client != null);
        }
    }
}
