// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    internal class ServiceCodeBuilderNames : CodeBuilderNames
    {
        public ServiceCodeBuilderNames()
            : base("service")
        {
        }

        public override string GetDataContractNamespace()
        {
            return Constants.ServiceCommunicationNamespace;
        }
    }
}
